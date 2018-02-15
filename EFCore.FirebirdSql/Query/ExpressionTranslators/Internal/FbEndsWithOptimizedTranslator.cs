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
    public class FbEndsWithOptimizedTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo _methodInfo  = typeof(string).GetRuntimeMethod(nameof(string.EndsWith), new[] { typeof(string) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (Equals(methodCallExpression.Method, _methodInfo))
            {
                var patternExpression = methodCallExpression.Arguments[0];
                var patternConstantExpression = patternExpression as ConstantExpression;

                var endsWithExpression = new NullCompensatedExpression(
                    Expression.Equal(
                        new SqlFunctionExpression(
                            "RIGHT", 
                            methodCallExpression.Object.Type,
                            new[]
                            {
                                methodCallExpression.Object,
                                new SqlFunctionExpression("CHARACTER_LENGTH", typeof(int), new[] { patternExpression })
                            }),
                        patternExpression));

                return patternConstantExpression != null
                    ? (string)patternConstantExpression.Value == string.Empty
                        ? (Expression)Expression.Constant(true)
                        : endsWithExpression
                    : Expression.OrElse(
                        endsWithExpression,
                        Expression.Equal(patternExpression, Expression.Constant(string.Empty)));
            } 
            return null;
        }
    }
}
