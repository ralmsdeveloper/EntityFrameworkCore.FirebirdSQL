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
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{ 
    public class FbObjectToStringTranslator : IMethodCallTranslator
    {
        static readonly List<Type> _supportedTypes = new List<Type>
        {
            typeof(int),
            typeof(long),
            typeof(DateTime),
            typeof(Guid),
            typeof(bool),
            typeof(byte),
            typeof(byte[]),
            typeof(double),
            typeof(DateTimeOffset),
            typeof(char),
            typeof(short),
            typeof(float),
            typeof(decimal),
            typeof(TimeSpan),
            typeof(uint),
            typeof(ushort),
            typeof(ulong),
            typeof(sbyte),
        };

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
            => methodCallExpression.Method.Name == nameof(ToString) &&
               methodCallExpression.Arguments.Count == 0 &&
               methodCallExpression.Object != null &&
               _supportedTypes.Contains(methodCallExpression.Object.Type.UnwrapNullableType().UnwrapEnumType())
                ? new ExplicitCastExpression(methodCallExpression.Object, typeof(string))
                : null;
    }
}
