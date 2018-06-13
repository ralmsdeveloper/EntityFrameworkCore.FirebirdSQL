/*
 *          Copyright (c) 2017-2018 Rafael Almeida (ralms@ralms.net)
 *                               Jiri Cincura   (juri@cincura.net)
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

using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionVisitors.Internal
{
    public class FbSqlTranslatingExpressionVisitor : SqlTranslatingExpressionVisitor
    {
        private static readonly HashSet<string> _dateTimeDataTypes
            = new HashSet<string>
            {
                "time",
                "date",
                "datetime",
                "timestamp"
            };

        public FbSqlTranslatingExpressionVisitor(
            SqlTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryModelVisitor queryModelVisitor,
            SelectExpression targetSelectExpression = null,
            Expression topLevelPredicate = null,
            bool inProjection = false)
            : base(dependencies, queryModelVisitor, targetSelectExpression, topLevelPredicate, inProjection)
        {
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            var visitedExpression = base.VisitBinary(binaryExpression);

            if (visitedExpression == null)
            {
                return null;
            }

            switch (visitedExpression.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                    return IsDateTimeBasedOperation(visitedExpression)
                        ? null
                        : visitedExpression;
            }

            return visitedExpression;
        }

        private static bool IsDateTimeBasedOperation(Expression expression)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                var typeMapping = InferTypeMappingFromColumn(binaryExpression.Left)
                                  ?? InferTypeMappingFromColumn(binaryExpression.Right);

                if (typeMapping != null
                    && _dateTimeDataTypes.Contains(typeMapping.StoreType))
                {
                    return true;
                }
            }

            return false;
        }

        private static RelationalTypeMapping InferTypeMappingFromColumn(Expression expression)
            => expression.FindProperty(expression.Type)?.FindRelationalMapping();
    }
}
