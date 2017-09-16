/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSQL
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
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class FbConnectionSettings  
    {
        public FbConnectionSettings(MigrationsSqlGeneratorDependencies dependencies,
            IFbOptions options) 
        {
           // _options = options;
        } 
        private static readonly ConcurrentDictionary<string, FbConnectionSettings> Settings
            = new ConcurrentDictionary<string, FbConnectionSettings>();

        private static FbConnectionStringBuilder SettingConnectionStringBuilder(FbConnectionStringBuilder csb)
        {
           
            return new FbConnectionStringBuilder
            {                
                Database = csb.Database,
                Port = csb.Port,
                DataSource = csb.DataSource
            };
        }

        public static FbConnectionSettings GetSettings(string connectionString)
        {
            var csb = new FbConnectionStringBuilder(connectionString);
            var settingsCsb = SettingConnectionStringBuilder(csb);
            return Settings.GetOrAdd(settingsCsb.ConnectionString, key =>
            {
                csb.Pooling = false;
                string serverVersion=string.Empty;
                //Error connection database not exists
                try
                {
                    using (var _connection = new FbConnection(csb.ConnectionString))
                    {
                        _connection.Open();
                        serverVersion = _connection.ServerVersion;
                    }
                }
                catch 
                { } 
                var version = new ServerVersion(serverVersion);
                return new FbConnectionSettings(settingsCsb, version);
            });
        }

        public static FbConnectionSettings GetSettings(DbConnection connection)
        {
            var csb = new FbConnectionStringBuilder(connection.ConnectionString);
            var settingsCsb = SettingConnectionStringBuilder(csb);
            return Settings.GetOrAdd(settingsCsb.ConnectionString, key =>
            {
                var open = false;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    open = true;
                }
                try
                {
                    var version = new ServerVersion(connection.ServerVersion);
                    var connectionSettings = new FbConnectionSettings(settingsCsb, version);
                    return connectionSettings;
                }
                finally
                {
                    if (open)
                        connection.Close();
                }
            });
        }

        public FbConnectionSettings(FbConnectionStringBuilder settingConnectionStringBuilder, ServerVersion serverVersion)
        => ServerVersion = serverVersion;  

        public readonly ServerVersion ServerVersion;

    }
}
