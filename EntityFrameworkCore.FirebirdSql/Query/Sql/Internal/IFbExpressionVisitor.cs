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

using System.Linq.Expressions;
using EntityFrameworkCore.FirebirdSql.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;

namespace EntityFrameworkCore.FirebirdSql.Query.Sql.Internal
{
    public interface IFbExpressionVisitor
    {
		Expression VisitExtract(FbExtractExpression extractExpression);
		Expression VisitSubString(FbSubStringExpression sbStringExpression);
    }
}
