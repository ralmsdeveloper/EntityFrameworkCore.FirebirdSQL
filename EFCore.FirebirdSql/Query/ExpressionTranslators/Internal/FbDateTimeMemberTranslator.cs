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
    public class FbDateTimeMemberTranslator : IMemberTranslator
    {
        private static readonly Dictionary<string, string> _datePartMapping
          = new Dictionary<string, string>
          {
                { nameof(DateTime.Year), "year" },
                { nameof(DateTime.Month), "month" },
                { nameof(DateTime.DayOfYear), "dayofyear" },
                { nameof(DateTime.Day), "day" },
                { nameof(DateTime.Hour), "hour" },
                { nameof(DateTime.Minute), "minute" },
                { nameof(DateTime.Second), "second" },
                { nameof(DateTime.Millisecond), "millisecond" },
                { nameof(TimeSpan.TotalDays), "day" },
                { nameof(TimeSpan.Days), "day" },
                { nameof(TimeSpan.TotalHours), "hour" },
                { nameof(TimeSpan.Hours), "hour" },
                { nameof(TimeSpan.TotalMinutes), "minute" },
                { nameof(TimeSpan.Minutes), "minute" },
                { nameof(TimeSpan.TotalSeconds), "second" },
                { nameof(TimeSpan.Seconds), "second" },
                { nameof(TimeSpan.TotalMilliseconds), "millisecond" },
                { nameof(TimeSpan.Milliseconds), "millisecond" },
          };

        public virtual Expression Translate(MemberExpression memberExpression)
        {
            var declaringType = memberExpression.Member.DeclaringType;
            var memberName = memberExpression.Member.Name;

            if (declaringType == typeof(TimeSpan)
                && memberExpression.Expression.NodeType == ExpressionType.Subtract)
            {
                var binaryExpression = memberExpression.Expression as BinaryExpression;
                if (_datePartMapping.TryGetValue(memberName, out var datePart))
                {
                    return new SqlFunctionExpression(
                    functionName: "DATEDIFF",
                    returnType: memberExpression.Type,
                    arguments: new[]
                    {
                        new SqlFragmentExpression(datePart),
                        binaryExpression.Left,
                        binaryExpression.Right
                    });
                }
            }

            if (declaringType == typeof(DateTime)
                || declaringType == typeof(DateTimeOffset))
            {
                if (_datePartMapping.TryGetValue(memberName, out var datePart))
                {
                    return new SqlFunctionExpression(
                    functionName: "EXTRACT",
                    returnType: memberExpression.Type,
                    arguments: new[]
                    {
                        new SqlFragmentExpression(datePart),
                        memberExpression.Expression
                    });

                }

                switch (memberName)
                {
                    case nameof(DateTime.Now):
                    case nameof(DateTime.UtcNow):
                        return new ExplicitCastExpression(
                            new SqlFragmentExpression("'NOW'"),
                            memberExpression.Type);

                    case nameof(DateTime.Today):
                        return new SqlFunctionExpression(
                            "CAST",
                            memberExpression.Type,
                            arguments: new[]
                            {
                                new SqlFragmentExpression("'TODAY'"),
                                new SqlFragmentExpression("DATE")
                            });

                    case nameof(DateTime.TimeOfDay):
                        return new SqlFunctionExpression(
                            "CAST",
                            memberExpression.Type,
                            arguments: new[]
                            {
                                memberExpression.Expression,
                                new SqlFragmentExpression("TIME")
                            });
                }
            }

            return null;
        }
    }
}
