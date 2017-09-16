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

namespace EntityFrameworkCore.FirebirdSql.Update.Internal
{
    public class FbUpdateSqlGenerator : UpdateSqlGenerator, IFbUpdateSqlGenerator
    {
		private readonly IRelationalTypeMapper _typeMapperRelational;
		public FbUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies, IRelationalTypeMapper typeMapper)
			: base(dependencies)
		{
			_typeMapperRelational = typeMapper;
		}

		public override ResultSetMapping AppendInsertOperation(StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
		{
			return AppendBlockInsertOperation(commandStringBuilder, new StringBuilder(), new[] { command }, commandPosition);
		} 

		public ResultSetMapping AppendBlockInsertOperation(StringBuilder commandStringBuilder, StringBuilder headBlockStringBuilder, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition)
		{
			commandStringBuilder.Clear();
			var commaAppend = string.Empty;
			for (var i = 0; i < modificationCommands.Count; i++)
			{
				var name = modificationCommands[i].TableName;
				var schema = modificationCommands[i].Schema;
				var operations = modificationCommands[i].ColumnModifications;
				var writeOperations = operations.Where(o => o.IsWrite).ToArray();
				var readOperations = operations.Where(o => o.IsRead).ToArray();
				if (writeOperations.Any())
				{
					AppendBlockVariable(headBlockStringBuilder, writeOperations, commaAppend);
					commaAppend = ",";
				}
				AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
				AppendValuesHeader(commandStringBuilder, writeOperations);
				AppendValuesInsert(commandStringBuilder, writeOperations);
				if (readOperations.Length > 0)
				{
					AppendInsertOutputClause(commandStringBuilder, readOperations, operations);
				}
				else if (readOperations.Length == 0)
				{
					AppendSelectAffectedCountCommand(commandStringBuilder, name, schema, commandPosition);
				}
			}

			return ResultSetMapping.NotLastInResultSet;
		}

		private void AppendBlockVariable(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations, string commaAppend)
		{

			foreach (var column in operations)
			{
				var _type = GetDataType(column.Property);
				if (!_type.Equals("CHAR(16) CHARACTER SET OCTETS", StringComparison.InvariantCultureIgnoreCase))
				{
					commandStringBuilder.Append(commaAppend);
					commandStringBuilder.Append($"{column.ParameterName}  {_type}=@{column.ParameterName}");
					commaAppend = ",";
				}
			}
		}

		private void AppendValuesInsert(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations)
		{
			if (operations.Count > 0)
			{
				commandStringBuilder
					.Append("(")
					.AppendJoin(
						operations,
						SqlGenerationHelper,
						(sb, o, helper) =>
						{
							var property = GetDataType(o.Property);

							if (o.IsWrite)
							{
								switch (property)
								{
									case "CHAR(16) CHARACTER SET OCTETS":
										sb.Append($"CHAR_TO_UUID('{o.Value}')");
										break;
									default:
										sb.Append(":").Append(o.ParameterName);
										break;
								}
							}
						})
					.Append(")");
			}
		}

	    public override ResultSetMapping AppendUpdateOperation( StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
	    {
		   return AppendBlockUpdateOperation(commandStringBuilder, new StringBuilder(), new[] { command }, commandPosition);
	    }


	    public override ResultSetMapping AppendDeleteOperation(StringBuilder commandStringBuilder, ModificationCommand command, int commandPosition)
	    {
		  return AppendBlockDeleteOperation(commandStringBuilder, new[] { command }, commandPosition);
		} 


		public ResultSetMapping AppendBlockUpdateOperation(StringBuilder commandStringBuilder, StringBuilder headBlockStringBuilder, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition)
		{
			 
			commandStringBuilder.Clear();
			var commaAppend = string.Empty;
			for (var i = 0; i < modificationCommands.Count; i++)
			{
				var name = modificationCommands[i].TableName;
				var operations = modificationCommands[i].ColumnModifications;
				var writeOperations = operations.Where(o => o.IsWrite).ToArray();
				var conditionsOperations = operations.Where(o => o.IsCondition).ToArray();
				AppendBlockVariable(headBlockStringBuilder, writeOperations, commaAppend);
				commaAppend = ",";
				commandStringBuilder.Append($"UPDATE {SqlGenerationHelper.DelimitIdentifier(name)} SET ")
				.AppendJoinUpadate(writeOperations,SqlGenerationHelper,(sb, o, helper) =>
					{
						if (o.IsWrite)
							sb.Append($"{SqlGenerationHelper.DelimitIdentifier(o.ColumnName)}=:{o.ParameterName} ");
					});
				AppendWhereClauseCustom(commandStringBuilder, conditionsOperations);
				commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
				AppendUpdateOrDeleteOutputClause(commandStringBuilder);
				commandStringBuilder.AppendLine("SUSPEND;");
			}
			return ResultSetMapping.NotLastInResultSet;
		}

		private void AppendWhereClauseCustom(StringBuilder commandStringBuilder, ColumnModification[] col)
		{
			if (!col.Any())
				return;

			commandStringBuilder.Append(" WHERE ");
			foreach (var item in col)
			{
				commandStringBuilder.Append(SqlGenerationHelper.DelimitIdentifier(item.ColumnName))
				                    .Append("=")
				                    .Append(item.Value);
			}
		}

		public ResultSetMapping AppendBlockDeleteOperation(StringBuilder commandStringBuilder, IReadOnlyList<ModificationCommand> modificationCommands, int commandPosition)
		{
			 
			var name = modificationCommands[0].TableName; 
			for (var i = 0; i < modificationCommands.Count; i++)
			{
				var operations = modificationCommands[i].ColumnModifications;
				var conditionsOperations = operations.Where(o => o.IsCondition).ToArray();

				commandStringBuilder.Append("DELETE FROM ");
				commandStringBuilder.Append(SqlGenerationHelper.DelimitIdentifier(name));
				AppendWhereClauseCustom(commandStringBuilder, conditionsOperations);
				commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator);
				AppendUpdateOrDeleteOutputClause(commandStringBuilder);
				commandStringBuilder.AppendLine("SUSPEND;");
			}
			return ResultSetMapping.NotLastInResultSet;
		}

		private void AppendUpdateOrDeleteOutputClause(StringBuilder commandStringBuilder)
		{

			commandStringBuilder.AppendLine("IF (ROW_COUNT > 0) THEN")
								.AppendLine("   AffectedRows=AffectedRows+1;"); 

		}

		private void AppendInsertOutputClause(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations,IReadOnlyList<ColumnModification> allOperations)
		{
			if (allOperations.Count > 0 && allOperations[0] == operations[0])
			{
				commandStringBuilder.AppendLine($" RETURNING {SqlGenerationHelper.DelimitIdentifier(operations.First().ColumnName)} INTO :AffectedRows;")
									.AppendLine("IF (ROW_COUNT > 0) THEN")
									.AppendLine("   SUSPEND;");
			}
		}

		protected override ResultSetMapping AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, string name, string schema, int commandPosition)
		{
			commandStringBuilder.AppendLine(" RETURNING ROW_COUNT INTO :AffectedRows;")
			                    .AppendLine("   SUSPEND;");

			return ResultSetMapping.LastInResultSet;
		}

		protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
		{
			throw new NotImplementedException();
		}

		protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
		{
			throw new NotImplementedException();
		}

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
							?? true,false,null).StoreType;
					}

					else if (property.ClrType == typeof(byte[]))
						typeName = _typeMapperRelational.ByteArrayMapper?.FindMapping(false, false,null).StoreType;
					else
						typeName = _typeMapperRelational.FindMapping(property.ClrType).StoreType;
				}
			}
			if (property.ClrType == typeof(byte[]) && typeName != null)
				return "BLOB SUB_TYPE BINARY";

			return typeName;
		} 
	}
}