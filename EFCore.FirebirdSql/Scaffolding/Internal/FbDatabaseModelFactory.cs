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
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using FbClient = FirebirdSql.Data;
using System.Diagnostics;
using EntityFrameworkCore.FirebirdSql.Internal;

namespace EntityFrameworkCore.FirebirdSql.Scaffolding.Internal
{
    public class FbDatabaseModelFactory : IDatabaseModelFactory
    {
        private static Version _version;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> _logger;

        public FbDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
            => _logger = logger;

        public DatabaseModel Create(string connectionString, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            using (var connection = new FbConnection(connectionString))
            {
                return Create(connection, tables, schemas);
            }
        }

        public DatabaseModel Create(DbConnection connection, IEnumerable<string> tables, IEnumerable<string> schemas)
        {
            var databaseModel = new DatabaseModel();

            var connectionStartedOpen = connection.State == ConnectionState.Open;
            if (!connectionStartedOpen)
            {
                connection.Open();
            }

            _version = FbClient.Services.FbServerProperties.ParseServerVersion(connection.ServerVersion);

            databaseModel.DefaultSchema = null;
            foreach (var table in GetTables(connection, tables))
            {
                table.Database = databaseModel;
                databaseModel.Tables.Add(table);
            }

            return databaseModel;
        }

        private IEnumerable<DatabaseTable> GetTables(DbConnection connection, IEnumerable<string> tables)
        {
            var tablesToSelect = new HashSet<string>(tables.ToList(), StringComparer.OrdinalIgnoreCase);
            var selectedTables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    SELECT RDB$RELATION_NAME FROM
                    RDB$RELATIONS t
                    WHERE t.RDB$RELATION_NAME <> '{HistoryRepository.DefaultTableName}'
                    AND RDB$VIEW_BLR IS NULL AND (RDB$SYSTEM_FLAG IS NULL OR RDB$SYSTEM_FLAG = 0);";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        if (!AllowsTable(tablesToSelect, selectedTables, name))
                        {
                            continue;
                        }

                        _logger.TableFound(name);

                        var table = new DatabaseTable { Name = name };

                        foreach (var column in GetColumns(connection, name))
                        {
                            column.Table = table;
                            table.Columns.Add(column);
                        }

                        var primaryKey = GetPrimaryKey(connection, name, table.Columns);
                        if (primaryKey != null)
                        {
                            primaryKey.Table = table;
                            if (!primaryKey.Columns.Any())
                            {
                                // Insert in Logger - refactor - v2.2
                                Console.WriteLine($"PK '{primaryKey.Name}' on table '{table.Name}' has no columns!");
                            }
                            else
                            {
                                table.PrimaryKey = primaryKey;
                            }
                        }

                        foreach (var index in GetIndexes(connection, name, table.Columns))
                        {
                            index.Table = table;
                            if (!index.Columns.Any())
                            {
                                // Insert in Logger - refactor - v2.2
                                Console.WriteLine($"index '{index.Name}' on table '{table.Name}' has no columns!");
                            }
                            table.Indexes.Add(index);  
                        }

                        yield return table;
                    }
                }
            }

            foreach (var table in tablesToSelect.Except(selectedTables, StringComparer.OrdinalIgnoreCase))
            {
                _logger.MissingTableWarning(table);
            }
        }

        private bool AllowsTable(
            HashSet<string> tables,
            HashSet<string> selectedTables,
            string name)
        {
            if (tables.Count == 0)
            {
                return true;
            }

            if (tables.Contains(name))
            {
                selectedTables.Add(name);
                return true;
            }

            return false;
        }

        private IEnumerable<DatabaseColumn> GetColumns(DbConnection connection, string table)
        {
            var _columnsOfTable = $@"
SELECT
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
    {FieldIsIdentity} IDENTITY
FROM RDB$RELATION_FIELDS RF
JOIN RDB$FIELDS F ON (F.RDB$FIELD_NAME = RF.RDB$FIELD_SOURCE)
LEFT OUTER JOIN RDB$CHARACTER_SETS CH ON (CH.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID)
LEFT OUTER JOIN RDB$COLLATIONS DCO ON ((DCO.RDB$COLLATION_ID = F.RDB$COLLATION_ID) AND (DCO.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID))
WHERE (COALESCE(RF.RDB$SYSTEM_FLAG, 0) = 0) AND RF.RDB$RELATION_NAME='{table}'
ORDER BY RF.RDB$FIELD_POSITION";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = _columnsOfTable;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var columnName = reader["FIELD_NAME"].ToString().Trim();
                        var dataType = reader["FIELD_TYPE"].ToString().Trim();
                        var notNull = reader["FIELD_NULL"].ToString().Trim().Equals("NOT NULL", StringComparison.OrdinalIgnoreCase);
                        var defaultValue = reader["FIELD_DEFAULT"].ToString().Trim();
                        var isIdentity = int.Parse(reader["IDENTITY"].ToString()) == 1;
                        var description = reader["FIELD_DESCRIPTION"].ToString().Trim();

                        _logger.ColumnFound(table, columnName, dataType, notNull, defaultValue);

                        var column = new DatabaseColumn
                        {
                            ComputedColumnSql = null,
                            Name = columnName,
                            StoreType = dataType,
                            IsNullable = !notNull,
                            DefaultValueSql = string.IsNullOrWhiteSpace(defaultValue)
                                ? null
                                : defaultValue,
                            ValueGenerated = isIdentity
                                ? Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd
                                : default
                        };

                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            column.AddAnnotation("Description", description.Replace(Environment.NewLine, "; "));
                        }

                        yield return column;
                    }
                }
            }
        }

        private DatabasePrimaryKey GetPrimaryKey(
            DbConnection connection,
            string table,
            IList<DatabaseColumn> columns)
        {
            var primaryKeys = $@"
SELECT I.RDB$INDEX_NAME AS INDEX_NAME,SG.RDB$FIELD_NAME AS FIELD_NAME
FROM RDB$INDICES I
    LEFT JOIN RDB$INDEX_SEGMENTS SG ON I.RDB$INDEX_NAME = SG.RDB$INDEX_NAME
    LEFT JOIN RDB$RELATION_CONSTRAINTS RC ON RC.RDB$INDEX_NAME = I.RDB$INDEX_NAME
WHERE RC.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY' AND I.RDB$RELATION_NAME = '{table}'";

            var primaryKey = new DatabasePrimaryKey();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = primaryKeys;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["INDEX_NAME"].ToString().Trim();
                        var columnName = reader["FIELD_NAME"].ToString().Trim();

                        if (string.IsNullOrWhiteSpace(primaryKey.Name))
                        {
                            primaryKey.Name = name;
                            _logger.PrimaryKeyFound(name, table);
                        }

                        var column = columns.FirstOrDefault(c => c.Name == columnName) ??
                            columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                        Debug.Assert(column != null, "column is null.");

                        primaryKey.Columns.Add(column);
                    }
                }
            }
            return primaryKey;
        }

        private IEnumerable<DatabaseIndex> GetIndexes(
            DbConnection connection,
            string table,
            IList<DatabaseColumn> columns)
        {
            var indexes = $@"
SELECT
    I.RDB$INDEX_NAME, COALESCE(I.RDB$UNIQUE_FLAG, 0) AS ISUNIQUE,
    I.RDB$RELATION_NAME, List(SG.RDB$FIELD_NAME) as ""RDB$FIELD_NAME"" FROM  RDB$INDICES I
    LEFT JOIN RDB$INDEX_SEGMENTS SG ON I.RDB$INDEX_NAME = SG.RDB$INDEX_NAME
    LEFT JOIN RDB$RELATION_CONSTRAINTS RC ON RC.RDB$INDEX_NAME = I.RDB$INDEX_NAME AND RC.RDB$CONSTRAINT_TYPE = NULL
WHERE I.RDB$RELATION_NAME = '{table}'
GROUP BY I.RDB$INDEX_NAME, ISUNIQUE, I.RDB$RELATION_NAME";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = indexes;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var indexName = reader["RDB$INDEX_NAME"].ToString().Trim();
                        var columnName = reader["RDB$FIELD_NAME"].ToString().Trim();
                        var isUnique = reader["ISUNIQUE"].ToString().Equals("1");

                        if (string.IsNullOrWhiteSpace(columnName))
                        {
                            // ignore indices without a column specified (i.e. with COMPUTED BY)
#pragma warning disable CS1030 // diretiva de #aviso
#warning Analyze this for 2.2
#pragma warning restore CS1030 // diretiva de #aviso
                            continue;
                        }

                        var index = new DatabaseIndex
                        {
                            Name = indexName,
                            IsUnique = !isUnique,
                        };

                        _logger.IndexFound(index.Name, table, index.IsUnique);

                        foreach (var n in columnName.Trim().Split(','))
                        {
                            var name = n.Trim();
                            var column = columns.FirstOrDefault(c => c.Name == name)
                                ?? columns.FirstOrDefault(c => c.Name.Equals(name, StringComparison.Ordinal));

                            Debug.Assert(column != null, $"column '{name}' parsed for index '{indexName}' from '{columnName}' in table {table} is null.");

                            index.Columns.Add(column);
                        }

                        yield return index;
                    }
                }
            }
        }

        private IEnumerable<DatabaseForeignKey> GetForeignKeys(
            DbConnection connection,
            DatabaseTable table,
            IList<DatabaseTable> tables)
        {
            var foreignKeys = $@"
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
            detail_relation_constraints.rdb$constraint_type = 'FOREIGN KEY' AND detail_relation_constraints.RDB$RELATION_NAME = '{table.Name}'
    ) FORAIGN
WHERE
    rc.rdb$constraint_type = 'PRIMARY KEY' AND FORAIGN.REFERENCE_TABLE_NAME = rc.rdb$relation_name";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = foreignKeys;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DatabaseForeignKey foreignKey = null;
                        var invalid = false;

                        try
                        {
                            var constaintName = reader["RDB$CONSTRAINT_NAME"].ToString().Trim();
                            var principalTableName = reader["RDB$RELATION_NAME"].ToString().Trim();
                            var onDelete = reader["RDB$DELETE_RULE"].ToString().Trim();

                            foreignKey = new DatabaseForeignKey
                            {
                                Name = constaintName,
                                PrincipalTable = tables.FirstOrDefault(t => t.Name == principalTableName) ??
                                tables.FirstOrDefault(t => t.Name.Equals(principalTableName, StringComparison.OrdinalIgnoreCase)),
                                OnDelete = ConvertToReferentialAction(onDelete),
                                Table = table
                            };

                            _logger.ForeignKeyFound(table.Name, constaintName, principalTableName, onDelete);

                            if (foreignKey.PrincipalTable == null)
                            {
                                _logger.ForeignKeyReferencesMissingTableWarning(constaintName);
                                continue;
                            }

                            foreach (var pair in reader.GetString(3).Split(','))
                            {

                                var columnName = pair.Split('$')[0];
                                var column = table.Columns.FirstOrDefault(c => c.Name == columnName)
                                    ??  table.Columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                                Debug.Assert(column != null, "column is null.");

                                var principalColumnName = pair.Split('$')[1];
                                var principalColumn = foreignKey.PrincipalTable.Columns.FirstOrDefault(c => c.Name == principalColumnName);

                                if (principalColumn == null)
                                {
                                    invalid = true;
                                    _logger.ForeignKeyPrincipalColumnMissingWarning(
                                        constaintName,
                                        table.Name,
                                        principalColumnName,
                                        principalTableName);
                                    break;
                                }

                                foreignKey.Columns.Add(column);
                                foreignKey.PrincipalColumns.Add(principalColumn);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"failed to get foreign keys for table {table.Name}", ex);
                        }
                        if (!invalid)
                        {
                            yield return foreignKey;
                        }
                    }
                }
            }

        }

        private static string FieldIsIdentity
            => (_version.Major >= 3 ? "COALESCE(RF.RDB$IDENTITY_TYPE, 0)" : "0");

        private static string EscapeLiteral(string s) => $"N'{s}'";

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
