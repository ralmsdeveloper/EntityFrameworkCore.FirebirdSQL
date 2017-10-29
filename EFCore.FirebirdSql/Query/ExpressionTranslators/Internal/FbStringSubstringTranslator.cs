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
using System.Reflection;
using EntityFrameworkCore.FirebirdSql.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{
    public class FbStringSubstringTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo = typeof(string).GetRuntimeMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (!_methodInfo.Equals(methodCallExpression.Method))
                return null;

            var from = methodCallExpression.Arguments[0].NodeType == ExpressionType.Constant
                ? (Expression)Expression.Constant((int)((ConstantExpression)methodCallExpression.Arguments[0]).Value + 1)
                : Expression.Add(methodCallExpression.Arguments[0], Expression.Constant(1));

            return new FbSubStringExpression(
                methodCallExpression.Object,
                from,
                methodCallExpression.Arguments[1]);
        }
    }
}
