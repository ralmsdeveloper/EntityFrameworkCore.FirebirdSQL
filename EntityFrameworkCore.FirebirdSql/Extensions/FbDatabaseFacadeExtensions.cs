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
using System.Reflection;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     FirebirdSQL specific extension methods for <see cref="DbContext.Database" />.
    /// </summary>
    public static class FbDatabaseFacadeExtensions
    { 
        public static bool IsFirebird(this DatabaseFacade database)
            => database.ProviderName.Equals(
                typeof(FbOptionsExtension).GetTypeInfo().Assembly.GetName().Name, StringComparison.Ordinal);
    }
}
