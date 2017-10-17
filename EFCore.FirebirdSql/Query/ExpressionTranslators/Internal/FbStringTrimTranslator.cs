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
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{
    public class FbStringTrimTranslator : IMethodCallTranslator
    {
        private static readonly MethodInfo MethodInfoWithoutArgs
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), new Type[] { });

        private static readonly MethodInfo MethodInfoWithCharArrayArg
            = typeof(string).GetRuntimeMethod(nameof(string.Trim), new[] { typeof(char[]) });

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            if (MethodInfoWithoutArgs.Equals(methodCallExpression.Method)
                || MethodInfoWithCharArrayArg.Equals(methodCallExpression.Method)
                && ((methodCallExpression.Arguments[0] as ConstantExpression)?.Value as Array)?.Length == 0)
            {
                var sqlArguments = new[] { methodCallExpression.Object };

                return new SqlFunctionExpression(
                    "TRIM",
                    methodCallExpression.Type,
                    sqlArguments);
            }

            return null;
        }
    }
}
