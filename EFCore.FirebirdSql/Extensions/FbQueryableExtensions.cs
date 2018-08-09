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

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore
{
    public static class FbQueryableExtensions
    {
        internal static readonly MethodInfo WithLockMethodInfo
            = typeof(FbQueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods(nameof(WithLock))
                .Single();

        public static IQueryable<TEntity> WithLock<TEntity>(
            this IQueryable<TEntity> source)
            where TEntity : class
        {
            return source.Provider.CreateQuery<TEntity>(
                Expression.Call(
                    null,
                    WithLockMethodInfo.MakeGenericMethod(typeof(TEntity)),
                    source.Expression));
        }
    }
}
