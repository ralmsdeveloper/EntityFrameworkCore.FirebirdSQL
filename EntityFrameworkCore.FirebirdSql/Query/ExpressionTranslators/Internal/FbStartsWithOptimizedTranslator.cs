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
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{

    public class FbStartsWithOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodStringOf
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(string) });

        private static readonly MethodInfo _methodCharOf
            = typeof(string).GetRuntimeMethod(nameof(string.StartsWith), new[] { typeof(char) });

        static readonly MethodInfo _concatCast
           = typeof(string).GetRuntimeMethod(nameof(string.Concat), new[] { typeof(string), typeof(string) });

        public virtual Expression Translate(MethodCallExpression methodStartCall)
        {
            if (!methodStartCall.Method.Equals(_methodStringOf) ||
                !methodStartCall.Method.Equals(_methodCharOf) ||
                methodStartCall.Object == null)
                return null;

            var constantPatternExpr = methodStartCall.Arguments[0] as ConstantExpression; 
            if (methodStartCall != null)
            {
                // Operation Simple With LIKE Sample (LIKE 'FIREBIRD%')
                return new LikeExpression(
                    methodStartCall.Object,
                    Expression.Constant(System.Text.RegularExpressions.Regex.Replace((string)constantPatternExpr?.Value, @"([%_\\'])", @"\$1") + '%')
                );
            } 

            var pattern = methodStartCall.Arguments[0];
            return Expression.AndAlso(
                new LikeExpression(methodStartCall.Object, Expression.Add(pattern, Expression.Constant("%"), _concatCast)),
                Expression.Equal(
                    new SqlFunctionExpression("LEFT", typeof(string), new[]
                    {
                        methodStartCall.Object,
                        new SqlFunctionExpression("CHARACTER_LENGTH", typeof(int), new[] { pattern }),
                    }),
                    pattern
                )
            );
              
        }
    }
}
