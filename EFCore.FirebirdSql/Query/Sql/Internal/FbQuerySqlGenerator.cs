/*
 *          Copyright (c) 2017-2018 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSql
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using EntityFrameworkCore.FirebirdSql.Query.Expressions.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.ResultOperators;
using Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Parsing;

namespace EntityFrameworkCore.FirebirdSql.Query.Sql.Internal
{
    public class FbQuerySqlGenerator : DefaultQuerySqlGenerator, IFbExpressionVisitor
    {
        private const string _letters = "bcdfghijklmnopqrstuvwxyz";
        private static int _incrementLetter = 0;
        private readonly bool _isLegacyDialect; 
        private readonly RelationalQueryCompilationContext _queryCompilationContext;
        private readonly List<IQueryAnnotation> _queryAnnotations;

        protected override string TypedTrueLiteral { get; } ="1";
        protected override string TypedFalseLiteral { get; } = "0";
        protected override string AliasSeparator => " ";
        
        public FbQuerySqlGenerator(
            QuerySqlGeneratorDependencies dependencies,
            SelectExpression selectExpression,
            IFbOptions fBOptions)
            : base(dependencies, selectExpression)
        {
            _isLegacyDialect = fBOptions.IsLegacyDialect;
            _queryCompilationContext = CompileRQCC()(selectExpression);

            _queryAnnotations = _queryCompilationContext
               .QueryAnnotations
               .Where(p =>
                   p.GetType() == typeof(IncludeResultOperator)
                   || p.GetType() == typeof(WithLockResultOperator))
               .Distinct()
               .ToList();
        }

        private static Func<SelectExpression, RelationalQueryCompilationContext> CompileRQCC()
        {
            var fieldInfo = typeof(SelectExpression)
                .GetTypeInfo()
                .GetRuntimeFields()
                .Single(f => f.FieldType == typeof(RelationalQueryCompilationContext));

            var parameterExpression = Expression.Parameter(
                typeof(SelectExpression),
                "selectExpression");

            return Expression
                .Lambda<Func<SelectExpression, RelationalQueryCompilationContext>>(
                    Expression.Field(parameterExpression, fieldInfo),
                    parameterExpression)
                .Compile();
        }

        public override Expression VisitSelect(SelectExpression selectExpression)
        {
            var visitSelectExpression = base.VisitSelect(selectExpression);

            if (_queryAnnotations.Count == 1
                && _queryAnnotations[0] is WithLockResultOperator annotation)
            {
                Sql.Append(annotation.Hint);
            }

            return visitSelectExpression;
        }

        public override Expression VisitTable(TableExpression tableExpression)
        { 
            if (_isLegacyDialect)
            {
                if (tableExpression.Alias.IndexOf(".", StringComparison.OrdinalIgnoreCase) > -1
                    || _letters.IndexOf(tableExpression.Alias, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    var letter = _letters[_incrementLetter];
                    tableExpression.Alias = letter.ToString();

                    _incrementLetter++;
                    if (_incrementLetter >= _letters.Length)
                    {
                        _incrementLetter = 0;
                    }
                }
            }

            return base.VisitTable(tableExpression);
        }

        protected override void GeneratePseudoFromClause()
            => Sql.Append(" FROM RDB$DATABASE");

        protected override void GenerateTop(SelectExpression selectExpression)
        {
            if (selectExpression.Limit != null)
            {
                Sql.AppendLine().Append("FIRST ");
                Visit(selectExpression.Limit);
                Sql.AppendLine().Append(" ");
            }

            if (selectExpression.Offset != null)
            {
                Sql.Append("SKIP ");
                Visit(selectExpression.Offset);
                Sql.Append(" ");
            }
        }

        protected override void GenerateProjection(Expression projection)
        {
            var aliasedProjection = projection as AliasExpression;
            var expressionToProcess = aliasedProjection?.Expression ?? projection;
            var updatedExperssion = ExplicitCastToBool(expressionToProcess);

            expressionToProcess = aliasedProjection != null
                ? new AliasExpression(aliasedProjection.Alias, updatedExperssion)
                : updatedExperssion;

            base.GenerateProjection(expressionToProcess);
        }

        private Expression ExplicitCastToBool(Expression expression)
            => ((expression as BinaryExpression)?.NodeType == ExpressionType.Coalesce || expression.NodeType == ExpressionType.Constant)
                && expression.Type.UnwrapNullableType() == typeof(bool)
                ? new ExplicitCastExpression(expression, expression.Type)
                : expression;

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Add &&
                expression.Left.Type == typeof(string) &&
                expression.Right.Type == typeof(string))
            {
                Sql.Append("(");
                Visit(expression.Left);
                Sql.Append("||");
                var exp = Visit(expression.Right);
                Sql.Append(")");
                return exp;
            }

            if (expression.NodeType == ExpressionType.And &&
                expression.Left.Type == typeof(int) &&
                expression.Right.Type == typeof(int))
            {
                Sql.Append("(BIN_AND(");
                Visit(expression.Left);
                Sql.Append(",");
                var exp = Visit(expression.Right);
                Sql.Append("))");
                return exp;
            }

            if (expression.NodeType == ExpressionType.Or &&
                expression.Left.Type == typeof(int) &&
                expression.Right.Type == typeof(int))
            {
                Sql.Append("(BIN_OR (");
                Visit(expression.Left);
                Sql.Append(",");
                var exp = Visit(expression.Right);
                Sql.Append("))");
                return exp;
            }

            var expr = base.VisitBinary(expression);
            return expr;
        }

        public virtual Expression VisitSubString(FbSubStringExpression substringExpression)
        {
            Sql.Append("SUBSTRING(");
            Visit(substringExpression.ValueExpression);
            Sql.Append(" FROM ");
            Visit(substringExpression.FromExpression);
            if (substringExpression.ForExpression != null)
            {
                Sql.Append(" FOR ");
                Visit(substringExpression.ForExpression);
            }
            Sql.Append(")");
            return substringExpression;
        }

        public virtual Expression VisitExtract(FbExtractExpression extractExpression)
        {
            Sql.Append("EXTRACT(");
            Sql.Append(extractExpression.Part);
            Sql.Append(" FROM ");
            Visit(extractExpression.ValueExpression);
            Sql.Append(")");
            return extractExpression;
        }
     
        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
        }

        public override Expression VisitFromSql(FromSqlExpression fromSqlExpression)
        {
            Sql.AppendLine("(");

            using (Sql.Indent())
            {
                GenerateFromSql(fromSqlExpression.Sql, fromSqlExpression.Arguments, ParameterValues);
            }

            Sql.Append(") ")
                .Append(SqlGenerator.DelimitIdentifier(fromSqlExpression.Alias));

            return fromSqlExpression;
        }

        public override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
        {
            switch (sqlFunctionExpression.FunctionName)
            {
                case "EXTRACT":
                    {
                        Sql.Append(sqlFunctionExpression.FunctionName);
                        Sql.Append("(");
                        Visit(sqlFunctionExpression.Arguments[0]);
                        Sql.Append(" FROM ");
                        Visit(sqlFunctionExpression.Arguments[1]);
                        Sql.Append(")");

                        return sqlFunctionExpression;
                    }
                case "CAST":
                    {
                        Sql.Append(sqlFunctionExpression.FunctionName);
                        Sql.Append("(");
                        Visit(sqlFunctionExpression.Arguments[0]);
                        Sql.Append(" AS ");
                        Visit(sqlFunctionExpression.Arguments[1]);
                        Sql.Append(")");

                        return sqlFunctionExpression;
                    }
            }

            return base.VisitSqlFunction(sqlFunctionExpression);
        }
    }
}
