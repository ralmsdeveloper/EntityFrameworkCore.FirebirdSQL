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
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{
    public class FbDateTimeNowTranslator : IMemberTranslator
    {
        public virtual Expression Translate(MemberExpression memberExpression)
        {
            if (memberExpression.Expression == null
                && memberExpression.Member.DeclaringType == typeof(DateTime))
            {
                switch (memberExpression.Member.Name)
                {
                    case nameof(DateTime.Now):
                        return new SqlFragmentExpression("CURRENT_TIMESTAMP");
                    case nameof(DateTime.UtcNow):
                        return new SqlFragmentExpression("CURRENT_TIMESTAMP");
                    default:
                        throw new Exception(nameof(memberExpression));
                }
            }
            return null;
        }
    }
}
