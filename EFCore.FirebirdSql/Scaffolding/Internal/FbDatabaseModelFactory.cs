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
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using FbClient = FirebirdSql.Data;

namespace EntityFrameworkCore.FirebirdSql.Scaffolding.Internal
{
    public class FbDatabaseModelFactory : IDatabaseModelFactory
    {
        FbConnection _connection;
        TableSelectionSet _tableSelectionSet;
        DatabaseModel _databaseModel;
        Dictionary<string, DatabaseTable> _tables;
        Dictionary<string, DatabaseColumn> _tableColumns;
        private string TableKey(DatabaseTable table) => TableKey(table.Name, table.Schema);
        private string TableKey(string name, string schema) => $"{name}";
        private string ColumnKey(DatabaseTable table, string columnName) => $"{TableKey(table)}.{columnName}";
        private static Version _version;
        #region Declaration Query
        private readonly string _getTablesQuery = @"SELECT
    RDB$RELATION_NAME
FROM
    RDB$RELATIONS
WHERE 
    RDB$VIEW_BLR IS NULL AND (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0)";

        private readonly string _columns = @"SELECT
    RF.RDB$RELATION_NAME,
    RF.RDB$FIELD_NAME FIELD_NAME,
    RF.RDB$FIELD_POSITION FIELD_POSITION,
    CASE F.RDB$FIELD_TYPE
    WHEN 7 THEN
        CASE F.RDB$FIELD_SUB_TYPE
        WHEN 0 THEN 'SMALLINT'
        WHEN 1 THEN 'NUMERIC(' || F.RDB$FIELD_PRECISION || ', ' || (-F.RDB$FIELD_SCALE) || ')'
        WHEN 2 THEN 'DECIMAL(' || F.RDB$FIELD_PRECISION || ', ' || (-F.RDB$FIELD_SCALE) || ')'
        END
    WHEN 8 THEN
        CASE F.RDB$FIELD_SUB_TYPE
        WHEN 0 THEN 'INTEGER'
        WHEN 1 THEN 'NUMERIC('  || F.RDB$FIELD_PRECISION || ', ' || (-F.RDB$FIELD_SCALE) || ')'
        WHEN 2 THEN 'DECIMAL(' || F.RDB$FIELD_PRECISION || ', ' || (-F.RDB$FIELD_SCALE) || ')'
        END
    WHEN 9 THEN 'QUAD'
    WHEN 10 THEN 'FLOAT'
    WHEN 12 THEN 'DATE'
    WHEN 13 THEN 'TIME'
    WHEN 14 THEN 'CHAR(' || (TRUNC(F.RDB$FIELD_LENGTH / CH.RDB$BYTES_PER_CHARACTER)) || ') ' || (CASE WHEN COALESCE(CH.RDB$CHARACTER_SET_NAME,'')<>'' THEN 'CHARACTER SET '||CH.RDB$CHARACTER_SET_NAME ELSE '' END)
    WHEN 16 THEN
        CASE F.RDB$FIELD_SUB_TYPE
        WHEN 0 THEN 'BIGINT'
        WHEN 1 THEN 'NUMERIC(' || F.RDB$FIELD_PRECISION || ', ' || (-F.RDB$FIELD_SCALE) || ')'
        WHEN 2 THEN 'DECIMAL(' || F.RDB$FIELD_PRECISION || ', ' || (-F.RDB$FIELD_SCALE) || ')'
        END
    WHEN 27 THEN 'DOUBLE PRECISION'
    WHEN 35 THEN 'TIMESTAMP'
    WHEN 37 THEN
        IIF (COALESCE(f.RDB$COMPUTED_SOURCE,'')<>'',
        'COMPUTED BY ' || CAST(f.RDB$COMPUTED_SOURCE AS VARCHAR(250)),
        'VARCHAR(' || (TRUNC(F.RDB$FIELD_LENGTH / CH.RDB$BYTES_PER_CHARACTER)) || ')')
    WHEN 261 THEN 'BLOB SUB_TYPE ' || (CASE WHEN F.RDB$FIELD_SUB_TYPE=0 THEN 'BINARY' ELSE 'TEXT' END)
    ELSE 'RDB$FIELD_TYPE: ' || F.RDB$FIELD_TYPE || '?'
    END FIELD_TYPE,
    IIF(COALESCE(RF.RDB$NULL_FLAG, 0) = 0, 'NULL', 'NOT NULL') FIELD_NULL,
    CH.RDB$CHARACTER_SET_NAME FIELD_CHARSET,
    DCO.RDB$COLLATION_NAME FIELD_COLLATION,
    COALESCE(RF.RDB$DEFAULT_SOURCE, F.RDB$DEFAULT_SOURCE) FIELD_DEFAULT,
    F.RDB$VALIDATION_SOURCE FIELD_CHECK,
    RF.RDB$DESCRIPTION FIELD_DESCRIPTION,
    {0} IDENTITY
FROM RDB$RELATION_FIELDS RF
JOIN RDB$FIELDS F ON (F.RDB$FIELD_NAME = RF.RDB$FIELD_SOURCE)
LEFT OUTER JOIN RDB$CHARACTER_SETS CH ON (CH.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID)
LEFT OUTER JOIN RDB$COLLATIONS DCO ON ((DCO.RDB$COLLATION_ID = F.RDB$COLLATION_ID) AND (DCO.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID))
WHERE (COALESCE(RF.RDB$SYSTEM_FLAG, 0) = 0) AND RF.RDB$RELATION_NAME='{1}'
ORDER BY RF.RDB$FIELD_POSITION";

        private readonly string _getPrimaryQuery = @"
SELECT I.RDB$INDEX_NAME AS INDEX_NAME,SG.RDB$FIELD_NAME AS FIELD_NAME
FROM RDB$INDICES I
    LEFT JOIN RDB$INDEX_SEGMENTS SG ON I.RDB$INDEX_NAME = SG.RDB$INDEX_NAME
    LEFT JOIN RDB$RELATION_CONSTRAINTS RC ON RC.RDB$INDEX_NAME = I.RDB$INDEX_NAME
WHERE RC.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY' AND I.RDB$RELATION_NAME = '{0}'";

        private readonly string _getIndexesQuery = @"
SELECT
    I.RDB$INDEX_NAME, COALESCE(I.RDB$UNIQUE_FLAG, 0) AS ISUNIQUE,
    I.RDB$RELATION_NAME, SG.RDB$FIELD_NAME FROM  RDB$INDICES I
    LEFT JOIN RDB$INDEX_SEGMENTS SG ON I.RDB$INDEX_NAME = SG.RDB$INDEX_NAME
    LEFT JOIN RDB$RELATION_CONSTRAINTS RC ON RC.RDB$INDEX_NAME = I.RDB$INDEX_NAME AND RC.RDB$CONSTRAINT_TYPE = NULL
WHERE I.RDB$RELATION_NAME = '{0}'  
GROUP BY I.RDB$INDEX_NAME, ISUNIQUE, I.RDB$RELATION_NAME, SG.RDB$FIELD_NAME";

        private readonly string _getConstraintsQuery = @"
SELECT
    FORAIGN.CONSTRAINT_NAME AS RDB$CONSTRAINT_NAME,
    FORAIGN.RELATION_NAME AS RDB$RELATION_NAME,
    FORAIGN.DELETE_RULE AS RDB$DELETE_RULE,
    trim(trim(FORAIGN.column_name)||'$'||sg.rdb$field_name) AS COLUMN_NAME
FROM
    rdb$indices ix
    left join rdb$index_segments sg on ix.rdb$index_name = sg.rdb$index_name
    left join rdb$relation_constraints rc on rc.rdb$index_name = ix.rdb$index_name,
        (
        SELECT
            detail_index_segments.RDB$FIELD_NAME AS column_name,
            detail_relation_constraints.RDB$CONSTRAINT_NAME AS CONSTRAINT_NAME,
            master_relation_constraints.RDB$RELATION_NAME AS RELATION_NAME,
            rdb$ref_constraints.RDB$DELETE_RULE AS DELETE_RULE,
            detail_relation_constraints.RDB$RELATION_NAME AS REFERENCE_TABLE_NAME
        FROM
            rdb$relation_constraints detail_relation_constraints
            JOIN rdb$index_segments detail_index_segments ON detail_relation_constraints.rdb$index_name = detail_index_segments.rdb$index_name 
            JOIN rdb$ref_constraints ON detail_relation_constraints.rdb$constraint_name = rdb$ref_constraints.rdb$constraint_name -- Master indeksas
            JOIN rdb$relation_constraints master_relation_constraints ON rdb$ref_constraints.rdb$const_name_uq = master_relation_constraints.rdb$constraint_name
            JOIN rdb$index_segments master_index_segments ON master_relation_constraints.rdb$index_name = master_index_segments.rdb$index_name 
        WHERE
            detail_relation_constraints.rdb$constraint_type = 'FOREIGN KEY' AND detail_relation_constraints.RDB$RELATION_NAME = '{0}'
            
    ) FORAIGN
WHERE
    rc.rdb$constraint_type = 'PRIMARY KEY' AND FORAIGN.REFERENCE_TABLE_NAME = rc.rdb$relation_name";
        #endregion

        public FbDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> loggerFactory)
        => Logger = loggerFactory;

        public virtual IDiagnosticsLogger<DbLoggerCategory.Scaffolding> Logger { get; }

        void ResetState()
        {
            _connection = null;
            _tableSelectionSet = null;
            _databaseModel = new DatabaseModel();
            _tables = new Dictionary<string, DatabaseTable>();
            _tableColumns = new Dictionary<string, DatabaseColumn>(StringComparer.OrdinalIgnoreCase);
        }

        public DatabaseModel Create(string connectionString, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            using (var connection = new FbConnection(connectionString))
            {
                return Create(connection, tables, schemas);
            }
        }

        public DatabaseModel Create(DbConnection connection, IEnumerable<string> tables, IEnumerable<string> schemas)
            => Create(connection, new TableSelectionSet(tables, schemas));

        public DatabaseModel Create(DbConnection connection, TableSelectionSet tableSelectionSet)
        {
            ResetState();
            _connection = (FbConnection)connection;
            if (_connection?.State != ConnectionState.Open)
                _connection?.Open();

            _version = FbClient.Services.FbServerProperties.ParseServerVersion(connection.ServerVersion);

            try
            {
                _tableSelectionSet = tableSelectionSet;
                _databaseModel.DatabaseName = _connection?.Database;
                _databaseModel.DefaultSchema = null;
                GetTables();
                GetColumns();
                GetPrimaryKeys();
                GetIndexes();
                GetConstraints();
                return _databaseModel;
            }
            finally
            {
                FbConnection.ClearPool(_connection);
                _connection?.Dispose();
            }
        }

        private void GetTables()
        {
            using (var command = new FbCommand(_getTablesQuery, _connection))
            {
                using (var rResult = command.ExecuteReader())
                {
                    while (rResult.Read())
                    {
                        var tableName = rResult["RDB$RELATION_NAME"].ToString().Trim();

                        var table = new DatabaseTable
                        {
                            Schema = null,
                            Name = tableName
                        };
                        Logger.Logger.LogDebug($"Creating => { tableName} Model");
                        if (_tableSelectionSet.Allows(table.Schema, table.Name))
                        {
                            _databaseModel.Tables.Add(table);
                            _tables[TableKey(table)] = table;
                        }
                    }
                }
            }
        }

        private void GetColumns()
        {
            foreach (var table in _tables)
            {
                using (var command = new FbCommand(string.Format(_columns, FieldIsIdentity, table.Key), _connection))
                {
                    using (var rResult = command.ExecuteReader())
                    {
                        while (rResult.Read())
                        {
                            var columnName = rResult["FIELD_NAME"].ToString().Trim();
                            var storeType = rResult["FIELD_TYPE"].ToString().Trim();
                            var isNullable = rResult["FIELD_NULL"].ToString().Trim();
                            var valueDefault = rResult["FIELD_DEFAULT"].ToString().Trim();
                            var isIdentity = int.Parse(rResult["IDENTITY"].ToString()) == 1;
                            var description = rResult["FIELD_DESCRIPTION"].ToString().Trim();

                            var column = new DatabaseColumn
                            {
                                Table = table.Value,
                                Name = columnName,
                                StoreType = storeType,
                                IsNullable = isNullable.Equals("NULL", StringComparison.OrdinalIgnoreCase),
                                DefaultValueSql
                                    = string.IsNullOrWhiteSpace(valueDefault)
                                        ? null
                                        : valueDefault,

                                ComputedColumnSql = null,
                                ValueGenerated
                                    = isIdentity
                                        ? Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd
                                        : default
                            };

                            if (!string.IsNullOrWhiteSpace(description))
                            {
                                column.AddAnnotation("Description", description.Replace(System.Environment.NewLine, "; "));
                            }

                            table.Value.Columns.Add(column);
                            Logger
                                .Logger
                                .LogDebug($"Creating Column: {column.Name.Trim()}, Type:{column.StoreType} to table => {column.Table.Name}");
                        }
                    }
                }
            }
        }

        private void GetPrimaryKeys()
        {
            foreach (var table in _tables)
            {
                DatabasePrimaryKey index = null;
                using (var command = new FbCommand(string.Format(_getPrimaryQuery, table.Key.Replace("\"", "")), _connection))
                {
                    using (var rResult = command.ExecuteReader())
                    {
                        while (rResult.Read())
                        {
                            var pkName = rResult["INDEX_NAME"].ToString().Trim();
                            var columnName = rResult["FIELD_NAME"].ToString().Trim();

                            if (index == null)
                            {
                                index = new DatabasePrimaryKey
                                {
                                    Table = table.Value,
                                    Name = pkName
                                };
                            }
                            var findColumn = table.Value.Columns
                                .FirstOrDefault(y => y.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                            if (findColumn != null)
                            {
                                index.Columns.Add(findColumn);
                            }
                        }
                    }
                }
                table.Value.PrimaryKey = index;
            }
        }

        private void GetIndexes()
        {
            foreach (var table in _tables)
            {
                DatabaseIndex index = null;
                using (var command = new FbCommand(string.Format(_getIndexesQuery, table.Key), _connection))
                {
                    using (var rResult = command.ExecuteReader())
                    {
                        while (rResult.Read())
                        {
                            try
                            {
                                var indexName = rResult["RDB$INDEX_NAME"].ToString().Trim();
                                var columnName = rResult["SG.RDB$FIELD_NAME"].ToString().Trim();
                                var isUnique = Convert.ToBoolean(rResult["ISUNIQUE"].ToString());

                                if (index == null)
                                {
                                    index = new DatabaseIndex
                                    {
                                        Table = table.Value,
                                        Name = indexName,
                                        IsUnique = !isUnique,
                                    };
                                }

                                foreach (var column in columnName.Trim().Split(','))
                                {
                                    var findColumn = table.Value.Columns
                                        .FirstOrDefault(y => y.Name.Equals(column.Trim(), StringComparison.OrdinalIgnoreCase));

                                    if (findColumn != null
                                        && !index.Columns.Where(i => i.Name == findColumn.Name).Any())
                                    {
                                        index.Columns.Add(findColumn);
                                    }
                                }

                                table.Value.Indexes.Add(index);

                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }

        private void GetConstraints()
        {
            var exceptions = new Dictionary<DatabaseForeignKey, Exception>();

            foreach (var table in _tables)
            {
                using (var command = new FbCommand(string.Format(_getConstraintsQuery, table.Key), _connection))
                {
                    using (var rResult = command.ExecuteReader())
                    {
                        while (rResult.Read())
                        {
                            if (_tables.ContainsKey($"{ rResult.GetString(1).ToString().Trim() }"))
                            {
                                var fkInfo = new DatabaseForeignKey
                                {
                                    Name = rResult["RDB$CONSTRAINT_NAME"].ToString().Trim(),
                                    OnDelete = ConvertToReferentialAction(rResult["RDB$DELETE_RULE"].ToString().Trim()),
                                    Table = table.Value,
                                    PrincipalTable = _tables[rResult["RDB$RELATION_NAME"].ToString().Trim()]
                                };

                                try
                                {
                                    foreach (var pair in rResult.GetString(3).Split(','))
                                    {
                                        fkInfo.Columns.Add(table.Value.Columns.Single(y => y.Name == pair.Split('$')[0]));
                                        fkInfo.PrincipalColumns.Add(fkInfo.PrincipalTable.Columns.Single(y => y.Name == pair.Split('$')[1]));
                                    }
                                    table.Value.ForeignKeys.Add(fkInfo);
                                }
                                catch (Exception e)
                                {
                                    exceptions.Add(fkInfo, e);
                                }
                            }
                            else
                            {
                                Logger.Logger.LogWarning($"Referenced table `{ rResult.GetString(1).ToString().Trim() }` is not in dictionary.");
                            }
                        }
                    }
                }
            }

            if (exceptions.Count > 0)
            {
                Logger.Logger.LogWarning($"Unable to reverse the following foraign keys ({exceptions.Count})");

                foreach (var exception in exceptions)
                {
                    Logger.Logger.LogWarning($"Table: {exception.Key.Table.Name} Referenced-Table: " +
                                             $"{exception.Key.PrincipalTable.Name} Constraint-Name: {exception.Key.Name}");
                }
                Logger.Logger.LogWarning($"---------------------------------------------------------");
            }
        }

        private static string FieldIsIdentity => (_version.Major >= 3 ? "COALESCE(RF.RDB$IDENTITY_TYPE, 0)" : "0");

        private static ReferentialAction? ConvertToReferentialAction(string onDeleteAction)
        {
            switch (onDeleteAction.ToUpperInvariant())
            {
                case "SET NULL":
                    return ReferentialAction.SetNull;
                case "SET DEFAUT":
                    return ReferentialAction.Restrict;
                case "CASCADE":
                    return ReferentialAction.Cascade;
                case "NO ACTION":
                    return ReferentialAction.NoAction;
                default:
                    return null;
            }
        }
    }
}
