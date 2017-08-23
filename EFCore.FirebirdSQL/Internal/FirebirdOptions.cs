/*                 
 *     EntityFrameworkCore.FirebirdSqlSQL  - Congratulations EFCore Team
 *              https://www.FirebirdSqlsql.org/en/net-provider/ 
 *     Permission to use, copy, modify, and distribute this software and its
 *     documentation for any purpose, without fee, and without a written
 *     agreement is hereby granted, provided that the above copyright notice
 *     and this paragraph and the following two paragraphs appear in all copies. 
 * 
 *     The contents of this file are subject to the Initial
 *     Developer's Public License Version 1.0 (the "License");
 *     you may not use this file except in compliance with the
 *     License. You may obtain a copy of the License at
 *     http://www.FirebirdSqlsql.org/index.php?op=doc&id=idpl
 *
 *     Software distributed under the License is distributed on
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations under the License.
 *
 *              Copyright (c) 2017 Rafael Almeida
 *         Made In Sergipe-Brasil - ralms@ralms.net 
 *                  All Rights Reserved.
 */


using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using FirebirdSql.Data.FirebirdClient;

namespace Microsoft.EntityFrameworkCore.Internal
{
    public class FirebirdSqlOptions : IFirebirdSqlOptions
    {

        private FbOptionsExtension _relationalOptions; 
        private readonly Lazy<FbConnectionSettings> _lazyConnectionSettings;

        public FirebirdSqlOptions()
        {
            _lazyConnectionSettings = new Lazy<FbConnectionSettings>(() =>
            {
                if (_relationalOptions.Connection != null)
                    return FbConnectionSettings.GetSettings(_relationalOptions.Connection);
                return FbConnectionSettings.GetSettings(_relationalOptions.ConnectionString);
            });
        }

        public virtual void Initialize(IDbContextOptions options)
        {
            _relationalOptions = options.FindExtension<FbOptionsExtension>() ?? new FbOptionsExtension();

        }

        public virtual void Validate(IDbContextOptions options)
        {
           //Removed Add Future!
        }

        public virtual FbConnectionSettings ConnectionSettings => _lazyConnectionSettings.Value;

        public virtual string GetCreateTable(ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            if (_relationalOptions.Connection != null)
                return GetCreateTable(_relationalOptions.Connection, sqlGenerationHelper, table, schema);
            return GetCreateTable(_relationalOptions.ConnectionString, sqlGenerationHelper, table, schema);
        }

        private static string GetCreateTable(string connectionString, ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            using (var connection = new FbConnection(connectionString))
            {
                connection.Open();
                return ExecuteCreateTable(connection, sqlGenerationHelper, table, schema);
            }
        }

        private static string GetCreateTable(DbConnection connection, ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            var opened = false;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                opened = true;
            }
            try
            {
                return ExecuteCreateTable(connection, sqlGenerationHelper, table, schema);
            }
            finally
            {
                if (opened)
                    connection.Close();
            }
        }

        private static string ExecuteCreateTable(DbConnection connection, ISqlGenerationHelper sqlGenerationHelper, string table, string schema)
        {
            using (var cmd = connection.CreateCommand())
            {
                var structTable = $@"select rf.rdb$relation_name as table_name, 
                                    rf.rdb$field_name as column_name,
                                    case f.rdb$field_type
                                        when 14 then 'CHAR'
                                        when 37 then 'VARCHAR'
                                        when 8 then 'INTEGER' 
                                    end as data_type,
                                    f.rdb$field_length,
                                    f.rdb$field_scale
                            from rdb$fields f
                                join rdb$relation_fields rf on rf.rdb$field_source = f.rdb$field_name
                            where rf.rdb$relation_name = '{table}'";

                cmd.CommandText = structTable;
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return reader.GetFieldValue<string>(1);
                }
            }
            return null;
        }

    }
}
