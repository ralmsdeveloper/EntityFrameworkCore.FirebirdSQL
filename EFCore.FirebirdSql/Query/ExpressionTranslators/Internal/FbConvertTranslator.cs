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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityFrameworkCore.FirebirdSql.Storage.Internal.Mapping;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{ 
    public class FbConvertTranslator : IMethodCallTranslator
    {
        static readonly Dictionary<string, string> TypeMapping = new Dictionary<string, string>
        {
            [nameof(Convert.ToByte)] = "SMALLINT",
            [nameof(Convert.ToDecimal)] = $"DECIMAL({FbTypeMapper.DefaultDecimalPrecision},{FbTypeMapper.DefaultDecimalScale})",
            [nameof(Convert.ToDouble)] = "DOUBLE PRECISION",
            [nameof(Convert.ToInt16)] = "SMALLINT",
            [nameof(Convert.ToInt32)] = "INTEGER",
            [nameof(Convert.ToInt64)] = "BIGINT",
            [nameof(Convert.ToString)] = $"VARCHAR({FbTypeMapper.VarcharMaxSize})"
        };

        static readonly HashSet<Type> SuportedTypes = new HashSet<Type>
        {
            typeof(bool),
            typeof(byte),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(long),
            typeof(short),
            typeof(string)
        };

        static readonly IEnumerable<MethodInfo> SupportedMethods
            = TypeMapping.Keys
                         .SelectMany(t => typeof(Convert).GetTypeInfo().GetDeclaredMethods(t)
                                                         .Where(m => m.GetParameters().Length == 1 && SuportedTypes.Contains(m.GetParameters().First().ParameterType)));

        public virtual Expression Translate(MethodCallExpression methodCallExpression)
            => SupportedMethods.Contains(methodCallExpression.Method)
                   ? new ExplicitCastExpression(methodCallExpression.Arguments[0], methodCallExpression.Type)
                   : null;
    }
}
