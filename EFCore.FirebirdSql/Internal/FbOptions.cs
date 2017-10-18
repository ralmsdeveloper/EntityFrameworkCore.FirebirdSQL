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
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Firebird = FirebirdSql.Data.FirebirdClient;
using Data = FirebirdSql.Data.Services;
using System.Collections.Concurrent;

namespace EntityFrameworkCore.FirebirdSql.Internal
{
    public class FbOptions : IFbOptions
    {
        public FbOptionsExtension Setting { get; private set; }
        public Version ServerVersion { get; private set; }
        public int ObjectLengthName { get; private set; }

        private static readonly ConcurrentDictionary<string, FbOptions> internalOptions
            = new ConcurrentDictionary<string, FbOptions>();

        public virtual void Initialize(IDbContextOptions options)
        {
            Setting = GetOptions(options);
            internalOptions.GetOrAdd(Setting.ConnectionString, key =>
            {
                try
                {
                    using (var _connection = new Firebird.FbConnection(Setting.ConnectionString))
                    {
                        _connection.Open();
                        ServerVersion = Data.FbServerProperties.ParseServerVersion(_connection.ServerVersion);
                        ObjectLengthName = ServerVersion?.Major == 3 ? 31 : 63;
                        _connection.Close();

                    }
                }
                catch
                {
                    //
                }
                return this;
            });
        }

        public virtual void Validate(IDbContextOptions options)
        {
            Setting = GetOptions(options);
        }

        private FbOptionsExtension GetOptions(IDbContextOptions options)
            => options.FindExtension<FbOptionsExtension>() ?? new FbOptionsExtension();
    }
}
