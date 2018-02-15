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

using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{ 
    public class FbStringIsNullOrWhiteSpaceTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo
            = typeof(string).GetRuntimeMethod(nameof(string.IsNullOrWhiteSpace), new[] { typeof(string) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method.Equals(_methodInfo))
            {
                var argument = methodCallExpression.Arguments[0];

                return Expression.MakeBinary(
                    ExpressionType.OrElse,
                    new IsNullExpression(argument),
                    Expression.Equal(new SqlFunctionExpression(
                                    "TRIM",
                                    typeof(string),
                                    new[] { argument }),
                        Expression.Constant("", typeof(string))));
            } 
            return null;
        }
    }
}
