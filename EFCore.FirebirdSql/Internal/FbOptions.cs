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

namespace EntityFrameworkCore.FirebirdSql.Internal
{
    public class FbOptions : IFbOptions
    {
        public FbOptionsExtension Settings { get; private set; }
        public Version ServerVersion { get; private set; }
        public bool IsLegacyDialect { get; private set; }
        public int ObjectLengthName => (ServerVersion ?? GetSettings(Settings.ConnectionString).ServerVersion).Major == 3 ? 31 : 63;

        public virtual void Initialize(IDbContextOptions options) => Settings = GetOptions(options);

        public virtual void Validate(IDbContextOptions options) => Settings = GetOptions(options);

        private FbOptionsExtension GetOptions(IDbContextOptions options)
            => options.FindExtension<FbOptionsExtension>() ?? new FbOptionsExtension();

        private FbOptions GetSettings(string connectionString)
        {
            if (ServerVersion != null)
            {
                return this;
            }

            try
            {
                using (var connection = new Firebird.FbConnection(connectionString))
                {
                    connection.Open();
                    ServerVersion = Data.FbServerProperties.ParseServerVersion(connection.ServerVersion);
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT MON$SQL_DIALECT FROM MON$DATABASE";
                        IsLegacyDialect = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return this;
        }
    }
}
