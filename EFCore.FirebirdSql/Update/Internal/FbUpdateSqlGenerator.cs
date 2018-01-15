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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFrameworkCore.FirebirdSql.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update;
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;

namespace EntityFrameworkCore.FirebirdSql.Update.Internal
{
    public class FbUpdateSqlGenerator : UpdateSqlGenerator, IFbUpdateSqlGenerator
    {
        private readonly IRelationalTypeMapper _typeMapperRelational;
        private string _commaAppend;
        private string _typeReturn;

        public FbUpdateSqlGenerator(
            UpdateSqlGeneratorDependencies dependencies,
            IRelationalTypeMapper typeMapper,
            IFbOptions fbOptions)
            : base(dependencies)
        {
            _typeMapperRelational = typeMapper;
            _typeReturn = fbOptions.IsLegacyDialect ? "INT" : "BIGINT";
        }

        public ResultSetMapping AppendBulkInsertOperation(
            StringBuilder commandStringBuilder,
            StringBuilder variablesParameters,
            StringBuilder dataReturnField,
            IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition)
        {
            commandStringBuilder.Clear();
            _commaAppend = variablesParameters.Length > 0 ? "," : string.Empty;
            var resultMapping = ResultSetMapping.LastInResultSet;
            for (var i = 0; i < modificationCommands.Count; i++)
            {
                var name = modificationCommands[i].TableName;
                var schema = modificationCommands[i].Schema;
                var operations = modificationCommands[i].ColumnModifications;
                var writeOperations = operations.Where(o => o.IsWrite).ToArray();
                var readOperations = operations.Where(o => o.IsRead).ToArray();

                if (readOperations.Length > 0 && dataReturnField.Length == 0)
                {
                    AppendReturnOutputBlock(dataReturnField, readOperations, operations);
                }

                if (writeOperations.Any())
                {
                    AppendBlockVariable(variablesParameters, writeOperations);
                }

                AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
                AppendValuesHeader(commandStringBuilder, writeOperations);
                AppendValuesInsert(commandStringBuilder, writeOperations);
                if (readOperations.Length > 0)
                {
                    AppendInsertOutputClause(commandStringBuilder, readOperations, operations);
                    resultMapping = ResultSetMapping.LastInResultSet;
                }
                else if (readOperations.Length == 0)
                {
                    AppendSelectAffectedCountCommand(commandStringBuilder, name, schema, commandPosition);
                    resultMapping = ResultSetMapping.NoResultSet;
                }
            }
            return resultMapping;
        }

        public ResultSetMapping AppendBulkUpdateOperation(
            StringBuilder commandStringBuilder,
            StringBuilder variablesParameters,
            IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition)
        {
            commandStringBuilder.Clear();
            _commaAppend = variablesParameters.Length > 0 ? "," : string.Empty;
            for (var i = 0; i < modificationCommands.Count; i++)
            {
                var name = modificationCommands[i].TableName;
                var operations = modificationCommands[i].ColumnModifications;
                var writeOperations = operations.Where(o => o.IsWrite).ToArray();
                var conditionsOperations = operations.Where(o => o.IsCondition).ToArray();

                if (writeOperations.Any())
                {
                    AppendBlockVariable(variablesParameters, writeOperations);
                }

                commandStringBuilder
                    .Append($"UPDATE {SqlGenerationHelper.DelimitIdentifier(name)} SET ")
                    .AppendJoinUpadate(
                        writeOperations,
                        SqlGenerationHelper,
                        (sb, o, helper) =>
                        {
                            if (o.IsWrite)
                            {
                                sb.Append(SqlGenerationHelper.DelimitIdentifier(o.ColumnName))
                                    .Append(" = ")
                                    .Append($":{o.ParameterName}");
                            }
                        });

                if (conditionsOperations.Any())
                {
                    AppendBlockVariable(variablesParameters, conditionsOperations);
                }

                AppendWhereClauseCustom(commandStringBuilder, conditionsOperations);
                commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
                AppendUpdateOrDeleteOutputClause(commandStringBuilder);
                commandStringBuilder.AppendLine("SUSPEND;");
            }
            return ResultSetMapping.NotLastInResultSet;
        }

        public ResultSetMapping AppendBulkDeleteOperation(StringBuilder commandStringBuilder, StringBuilder variablesParameters, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition)
        {
            var name = modificationCommands[0].TableName;
            _commaAppend = variablesParameters.Length > 0 ? "," : string.Empty;
            for (var i = 0; i < modificationCommands.Count; i++)
            {
                var operations = modificationCommands[i].ColumnModifications;
                var conditionsOperations = operations.Where(o => o.IsCondition).ToArray();
                if (conditionsOperations.Any())
                {
                    AppendBlockVariable(variablesParameters, conditionsOperations);
                }
                commandStringBuilder.Append("DELETE FROM ");
                commandStringBuilder.Append(SqlGenerationHelper.DelimitIdentifier(name));
                AppendWhereClauseCustom(commandStringBuilder, conditionsOperations);
                commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
                AppendUpdateOrDeleteOutputClause(commandStringBuilder);
                commandStringBuilder.AppendLine("SUSPEND;");
            }
            return ResultSetMapping.NotLastInResultSet;
        }

        private void AppendBlockVariable(StringBuilder variablesParameters, IReadOnlyList<ColumnModification> operations)
        {
            foreach (var column in operations)
            {
                var _type = GetDataType(column.Property);
                variablesParameters.Append(_commaAppend);
                variablesParameters.Append($"{column.ParameterName}  {_type}=@{column.ParameterName}");
                _commaAppend = ",";
            }
        }

        private void AppendValuesInsert(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations)
        {
            if (operations.Count > 0)
            {
                commandStringBuilder
                    .Append("(")
                    .AppendJoin(operations, SqlGenerationHelper, (sb, o, helper) =>
                    {
                        if (o.IsWrite)
                        {
                            sb.Append(":").Append(o.ParameterName);
                        }
                    })
                    .Append(")");
            }
        }

        private void AppendWhereClauseCustom(StringBuilder commandStringBuilder, ColumnModification[] columns)
        {
            if (columns.FirstOrDefault(p => p.IsCondition) == null)
            {
                return;
            }

            commandStringBuilder
                .Append(" WHERE ")
                .AppendJoin(columns, SqlGenerationHelper, (sb, o, helper) =>
                {
                    if (o.IsCondition)
                    {
                        sb.Append(SqlGenerationHelper.DelimitIdentifier(o.ColumnName))
                            .Append(" = ")
                            .Append($":{o.ParameterName}");
                    }
                }, " AND ");
        }

        private void AppendUpdateOrDeleteOutputClause(StringBuilder commandStringBuilder)
            => commandStringBuilder.AppendLine("   AffectedRows=AffectedRows+1;");

        private void AppendInsertOutputClause(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations, IReadOnlyList<ColumnModification> allOperations)
        {
            if (allOperations.Count > 0 && allOperations[0] == operations[0])
            {
                commandStringBuilder.AppendLine($" RETURNING {SqlGenerationHelper.DelimitIdentifier(operations.First().ColumnName)} INTO :AffectedRows;");
            }
            else
            {
                commandStringBuilder
                    .Append(" RETURNING ")
                    .AppendJoin(operations, (b, e) =>
                    {
                        b.Append(SqlGenerationHelper.DelimitIdentifier(e.ColumnName));
                    }, ", ")
                    .Append(" INTO ")
                    .AppendJoin(operations, (b, e) =>
                    {
                        b.Append($":{e.ColumnName}");
                    }, ", ")
                    .AppendLine(SqlGenerationHelper.StatementTerminator);
            }
            commandStringBuilder.AppendLine("SUSPEND;");
        }

        private void AppendReturnOutputBlock(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations, IReadOnlyList<ColumnModification> allOperations)
        {
            if (allOperations.Count > 0 && allOperations[0] == operations[0])
            {
                commandStringBuilder.AppendLine($"RETURNS (AffectedRows {_typeReturn}) AS BEGIN");
                commandStringBuilder.AppendLine("AffectedRows=0;");
            }
            else
            {
                commandStringBuilder
                    .Append(" RETURNS (")
                    .AppendJoin(operations, (b, e) =>
                    {
                        b.Append(e.ColumnName);
                        b.Append(" ");
                        b.Append(GetDataType(e.Property));

                    }, ", ")
                    .AppendLine(") AS BEGIN");
            }
        }

        protected override ResultSetMapping AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, string name, string schema, int commandPosition)
        {
            commandStringBuilder
                .AppendLine(" RETURNING ROW_COUNT INTO :AffectedRows;")
                .AppendLine("SUSPEND;");

            return ResultSetMapping.LastInResultSet;
        }

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
            => throw new NotImplementedException();

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
            => throw new NotImplementedException();

        private string GetDataType(IProperty property)
        {
            var typeName = property.Firebird().ColumnType;
            if (typeName == null)
            {
                var propertyDefault = property.FindPrincipal();
                typeName = propertyDefault?.Firebird().ColumnType;
                if (typeName == null)
                {
                    if (property.ClrType == typeof(string))
                    {
                        typeName = _typeMapperRelational.StringMapper?.FindMapping(property.IsUnicode()
                            ?? propertyDefault?.IsUnicode()
                            ?? true, false, null).StoreType;
                    }

                    else if (property.ClrType == typeof(byte[]))
                    {
                        typeName = _typeMapperRelational.ByteArrayMapper?.FindMapping(false, false, null).StoreType;
                    }
                    else
                    {
                        typeName = _typeMapperRelational.FindMapping(property.ClrType).StoreType;
                    }
                }
            }
            if (property.ClrType == typeof(byte[]) && typeName != null)
            {
                return "BLOB SUB_TYPE BINARY";
            }
            return typeName;
        }
    }
}
