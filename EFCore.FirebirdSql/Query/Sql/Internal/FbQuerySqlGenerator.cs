/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
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
using System.Linq.Expressions;
using EntityFrameworkCore.FirebirdSql.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Query.Sql.Internal
{
    public class FbQuerySqlGenerator : DefaultQuerySqlGenerator, IFbExpressionVisitor
    {
        protected override string TypedTrueLiteral => "1";
        protected override string TypedFalseLiteral => "0";

        public FbQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, SelectExpression selectExpression)
            : base(dependencies, selectExpression)
        { }

        public override Expression VisitSelect(SelectExpression selectExpression)
        {
            base.VisitSelect(selectExpression);
            if (selectExpression.Type == typeof(bool))
            {
                Sql.Append(" FROM RDB$DATABASE");
            }
            return selectExpression;
        }

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
            => (expression as BinaryExpression)?.NodeType == ExpressionType.Coalesce
                   && expression.Type.UnwrapNullableType() == typeof(bool)
                ? new ExplicitCastExpression(expression, expression.Type)
                : expression;

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (binaryExpression.NodeType == ExpressionType.Add &&
                binaryExpression.Left.Type == typeof(string) &&
                binaryExpression.Right.Type == typeof(string))
            {
                Sql.Append("(");
                Visit(binaryExpression.Left);
                Sql.Append("||");
                var exp = Visit(binaryExpression.Right);
                Sql.Append(")");
                return exp;
            }
            
            if (binaryExpression.NodeType == ExpressionType.And &&
                binaryExpression.Left.Type == typeof(System.Int32) &&
                binaryExpression.Right.Type == typeof(System.Int32))
            {
                Sql.Append("(BIN_AND(");
                Visit(binaryExpression.Left);
                Sql.Append(",");
                var exp = Visit(binaryExpression.Right);
                Sql.Append("))");
                return exp;
            }

            if (binaryExpression.NodeType == ExpressionType.Or &&
                binaryExpression.Left.Type == typeof(System.Int32) &&
                binaryExpression.Right.Type == typeof(System.Int32))
            {
                Sql.Append("(BIN_OR (");
                Visit(binaryExpression.Left);
                Sql.Append(",");
                var exp = Visit(binaryExpression.Right);
                Sql.Append("))");
                return exp;
            }

            var expr = base.VisitBinary(binaryExpression);
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
    }
}
