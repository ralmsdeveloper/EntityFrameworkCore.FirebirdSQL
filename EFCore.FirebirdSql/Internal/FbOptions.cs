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
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.FirebirdSql.Internal
{
    using Firebird = global::FirebirdSql.Data.FirebirdClient; 
    using Data = global::FirebirdSql.Data.Services;
    public class FbOptions : IFbOptions
    {
        private bool IsLegacy { get; set; }
        public FbOptionsExtension Settings { get; private set; }
        public Version ServerVersion { get; private set; }
        public bool IsLegacyDialect
        {
            get
            {
                if (ServerVersion == null && Settings != null)
                {
                    GetSettings(Settings.ConnectionString);
                }
                return IsLegacy;
            }
            set => IsLegacy = value;
        }
        public int ObjectLengthName => (ServerVersion ?? GetSettings(Settings.ConnectionString).ServerVersion).Major == 3 ? 31 : 63;

        public virtual void Initialize(IDbContextOptions options) => Settings = GetOptions(options);

        public virtual void Validate(IDbContextOptions options) => Settings = GetOptions(options);

        private FbOptionsExtension GetOptions(IDbContextOptions options)
            => options.FindExtension<FbOptionsExtension>() ?? new FbOptionsExtension();

        private FbOptions GetSettings(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = Settings.Connection.ConnectionString;
            }
            if (ServerVersion != null)
            {
                return this;
            }

            try
            {
                IsLagacyDataBase(connectionString);
            }
            catch(Exception)
            { 
            }
            return this;
        }

        private void IsLagacyDataBase(string connectionString)
        {
            try
            {
                using (var connection = new Firebird.FbConnection(connectionString))
                {
                    connection.Open();
                    ServerVersion = Data.FbServerProperties.ParseServerVersion(connection.ServerVersion);
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT MON$SQL_DIALECT FROM MON$DATABASE";
                        IsLegacy = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
                    }
                    connection.Close();
                }
            }
            finally
            {
            }
        }
    }
}
