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

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionVisitors.Internal
{
    public class FbSqlTranslatingExpressionVisitorFactory : SqlTranslatingExpressionVisitorFactory
    {
        public FbSqlTranslatingExpressionVisitorFactory(
            SqlTranslatingExpressionVisitorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override SqlTranslatingExpressionVisitor Create(
            RelationalQueryModelVisitor queryModelVisitor,
            SelectExpression targetSelectExpression = null,
            Expression topLevelPredicate = null,
            bool inProjection = false)
            => new FbSqlTranslatingExpressionVisitor(
                Dependencies, queryModelVisitor, targetSelectExpression, topLevelPredicate, inProjection);
    }
}
