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
using System.Data.Common;
using System.Data;

namespace EntityFrameworkCore.FirebirdSql.Internal
{
    public class FbOptions : IFbOptions
    {
        private Lazy<FbOptions> internaSettings;
        public FbOptionsExtension Setting { get; private set; }
        public Version ServerVersion { get; private set; }

        public int ObjectLengthName
            => (ServerVersion ?? GetSettings(Setting.ConnectionString).ServerVersion).Major == 3 ? 31 : 63;

        private static readonly ConcurrentDictionary<string, FbOptions> internalOptions
            = new ConcurrentDictionary<string, FbOptions>();

        public virtual void Initialize(IDbContextOptions options)
        {
            Setting = GetOptions(options);
            internaSettings = new Lazy<FbOptions>(()
                => Setting.Connection != null
                        ? GetSettings(Setting.Connection)
                        : GetSettings(Setting.ConnectionString));
        }

        public virtual void Validate(IDbContextOptions options)
        {
            Setting = GetOptions(options);
        }

        private FbOptionsExtension GetOptions(IDbContextOptions options)
            => options.FindExtension<FbOptionsExtension>() ?? new FbOptionsExtension();

        private FbOptions GetSettings(string connectionString)
        {
            if (ServerVersion != null)
                return this;

            var csb = new Firebird.FbConnectionStringBuilder(connectionString);

            return internalOptions.GetOrAdd(csb.ConnectionString, key =>
            {
                try
                {
                    using (var _connection = new Firebird.FbConnection(csb.ConnectionString))
                    {
                        _connection.Open();
                        ServerVersion = Data.FbServerProperties.ParseServerVersion(_connection.ServerVersion);
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

        private FbOptions GetSettings(DbConnection connection)
        {
            if (ServerVersion != null)
                return this;

            var csb = new Firebird.FbConnectionStringBuilder(connection.ConnectionString);
            return internalOptions.GetOrAdd(csb.ConnectionString, key =>
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                try
                {
                    ServerVersion = Data.FbServerProperties.ParseServerVersion(connection.ServerVersion);
                    return this;
                }
                finally
                {
                    if (connection?.State == ConnectionState.Open)
                        connection.Close();
                }
            });
        }
    }
}
