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
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using EntityFrameworkCore.FirebirdSql.Storage.Internal;

namespace EntityFrameworkCore.FirebirdSql.Update.Internal
{

    public class FbUpdateSqlGenerator : UpdateSqlGenerator, IFbUpdateSqlGenerator
    {
        private readonly IRelationalTypeMappingSource _typeMapper;
        private string _typeReturn;

        public FbUpdateSqlGenerator(
            UpdateSqlGeneratorDependencies dependencies,
            IRelationalTypeMappingSource typeMapper,
            IFbOptions fbOptions)
            : base(dependencies)
        {
            _typeMapper = typeMapper;
            _typeReturn = fbOptions.IsLegacyDialect ? "INT" : "BIGINT";
        }

        public override ResultSetMapping AppendInsertOperation(StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
        {
            var result = ResultSetMapping.NoResultSet;
            var name = command.TableName;
            var operations = command.ColumnModifications;
            var writeOperations = operations.Where(o => o.IsWrite).ToList();
            var readOperations = operations.Where(o => o.IsRead).ToList();
            var anyRead = readOperations.Any();
            AppendInsertCommandHeader(commandStringBuilder, name, null, writeOperations);
            AppendValuesHeader(commandStringBuilder, writeOperations);
            AppendValues(commandStringBuilder, writeOperations);
            if (anyRead)
            {
                commandStringBuilder.AppendLine();
                commandStringBuilder.Append("RETURNING ");
                commandStringBuilder.AppendJoin(readOperations, (b, e) =>
                {
                    b.Append(SqlGenerationHelper.DelimitIdentifier(e.ColumnName));
                }, ", ");
                result = ResultSetMapping.LastInResultSet;
            }
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
            return result;
        }

        public override ResultSetMapping AppendUpdateOperation(StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
        {
            var sqlGenerationHelper = (IFbSqlGenerationHelper)SqlGenerationHelper;
            var name = command.TableName;
            var operations = command.ColumnModifications;
            var writeOperations = operations.Where(o => o.IsWrite).ToList();
            var readOperations = operations.Where(o => o.IsRead).ToList();
            var conditionOperations = operations.Where(o => o.IsCondition).ToList();
            var inputOperations = operations.Where(o => o.IsWrite || o.IsCondition).ToList();
            var anyRead = readOperations.Any();
            commandStringBuilder.Append("EXECUTE BLOCK (");
            commandStringBuilder.AppendJoin(inputOperations, (b, e) =>
            {
                var type = GetColumnType(e);
                var parameterName = e.UseOriginalValueParameter
                    ? e.OriginalParameterName
                    : e.ParameterName;
                b.Append(parameterName);
                b.Append(" ");
                b.Append(type);
                b.Append(" = ?");
            }, ", ");
            commandStringBuilder.AppendLine(")");
            commandStringBuilder.Append("RETURNS (");
            if (anyRead)
            {
                commandStringBuilder.AppendJoin(readOperations, (b, e) =>
                {
                    var type = GetColumnType(e);
                    b.Append(SqlGenerationHelper.DelimitIdentifier(e.ColumnName));
                    b.Append(" ");
                    b.Append(type);
                }, ", ");
            }
            else
            {
                commandStringBuilder.Append($"RowsAffected {_typeReturn}");
            }
            commandStringBuilder.AppendLine(")");
            commandStringBuilder.AppendLine("AS");
            commandStringBuilder.AppendLine("BEGIN");
            var oldParameterNameMarker = sqlGenerationHelper.ParameterName;
            sqlGenerationHelper.ParameterName = ":";
            try
            {
                AppendUpdateCommandHeader(commandStringBuilder, name, null, writeOperations);
                AppendWhereClause(commandStringBuilder, conditionOperations);
            }
            finally
            {
                sqlGenerationHelper.ParameterName = oldParameterNameMarker;
            }
            if (anyRead)
            {
                commandStringBuilder.AppendLine();
                commandStringBuilder.Append("RETURNING ");
                commandStringBuilder.AppendJoin(readOperations, (b, e) =>
                {
                    b.Append(SqlGenerationHelper.DelimitIdentifier(e.ColumnName));
                    b.Append(" INTO :");
                    b.Append(SqlGenerationHelper.DelimitIdentifier(e.ColumnName));
                }, ", ");
            }
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
            if (!anyRead)
            {
                commandStringBuilder.AppendLine("RowsAffected = ROW_COUNT;");
                commandStringBuilder.AppendLine("SUSPEND;");
            }
            else
            {
                commandStringBuilder.AppendLine("IF (ROW_COUNT > 0) THEN");
                commandStringBuilder.AppendLine("SUSPEND;");
            }
            commandStringBuilder.Append("END");
            commandStringBuilder.Append(SqlGenerationHelper.StatementTerminator).AppendLine();
            return ResultSetMapping.LastInResultSet;
        }

        public override ResultSetMapping AppendDeleteOperation(StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
        {
            var sqlGenerationHelper = (IFbSqlGenerationHelper)SqlGenerationHelper;
            var name = command.TableName;
            var operations = command.ColumnModifications;
            var conditionOperations = operations.Where(o => o.IsCondition).ToList();
            var inputOperations = conditionOperations;
            commandStringBuilder.Append("EXECUTE BLOCK (");
            commandStringBuilder.AppendJoin(inputOperations, (b, e) =>
            {
                var type = GetColumnType(e);
                var parameterName = e.UseOriginalValueParameter
                    ? e.OriginalParameterName
                    : e.ParameterName;
                b.Append(parameterName);
                b.Append(" ");
                b.Append(type);
                b.Append(" = ?");
            }, ", ");
            commandStringBuilder.AppendLine(")");
            commandStringBuilder.AppendLine($"RETURNS (RowsAffected {_typeReturn})");
            commandStringBuilder.AppendLine("AS");
            commandStringBuilder.AppendLine("BEGIN");
            var oldParameterNameMarker = sqlGenerationHelper.ParameterName;
            sqlGenerationHelper.ParameterName = ":";
            try
            {
                AppendDeleteCommandHeader(commandStringBuilder, name, null);
                AppendWhereClause(commandStringBuilder, conditionOperations);
            }
            finally
            {
                sqlGenerationHelper.ParameterName = oldParameterNameMarker;
            }
            commandStringBuilder
                .Append(SqlGenerationHelper.StatementTerminator)
                .AppendLine();
            commandStringBuilder.AppendLine();
            commandStringBuilder.AppendLine("RowsAffected = ROW_COUNT;");
            commandStringBuilder.AppendLine("SUSPEND;");
            commandStringBuilder.Append("END");
            commandStringBuilder
                .Append(SqlGenerationHelper.StatementTerminator)
                .AppendLine();
            return ResultSetMapping.LastInResultSet;
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
            => throw new InvalidOperationException();

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
            => throw new InvalidOperationException();

        string GetColumnType(ColumnModification column)
            => _typeMapper.FindMapping(column.Property).StoreType;
    }
}
