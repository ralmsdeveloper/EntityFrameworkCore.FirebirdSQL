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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using FirebirdSql.Data.FirebirdClient;

namespace Microsoft.EntityFrameworkCore.Update.Internal
{
    public class FirebirdSqlUpdateSqlGenerator : UpdateSqlGenerator, IFirebirdSqlUpdateSqlGenerator
    {
        public FirebirdSqlUpdateSqlGenerator(
            [NotNull] UpdateSqlGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }


        public override ResultSetMapping AppendInsertOperation(
           StringBuilder commandStringBuilder,
           ModificationCommand command,
           int commandPosition)
        {
            Check.NotNull(command, nameof(command));
            return AppendBlockInsertOperation(commandStringBuilder, new[] { command }, commandPosition);
        }


        public ResultSetMapping AppendBlockInsertOperation(StringBuilder commandStringBuilder, IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(modificationCommands, nameof(modificationCommands));
            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;
            var operations = modificationCommands[0].ColumnModifications;
            var writeOperations = operations.Where(o => o.IsWrite).ToArray();
            var readOperations = operations.Where(o => o.IsRead).ToArray();
            commandStringBuilder.Clear();
            commandStringBuilder.Append("EXECUTE BLOCK ");
            if (writeOperations.Any())
            {
                commandStringBuilder.AppendLine("(");
                var commaAppend = string.Empty;
                for (var i = 0; i < modificationCommands.Count; i++)
                { 
                    foreach (var column in modificationCommands[i].ColumnModifications.Where(o => o.IsWrite))
                    {
                        commandStringBuilder.Append(commaAppend); 
                        commandStringBuilder.AppendLine($"{column.ParameterName}  {FirebirdSqlSqlGenerationHelper.GetTypeColumnToString(column)}=?");
                        commaAppend = ",";
                    } 
                }
                commandStringBuilder.AppendLine(")");
            }
            commandStringBuilder.AppendLine("RETURNS (regAffeted INT) AS BEGIN");
            for (var i = 0; i < modificationCommands.Count; i++)
            {
                
                AppendInsertCommandHeader(commandStringBuilder, name, schema, writeOperations);
                AppendValuesHeader(commandStringBuilder, modificationCommands[i].ColumnModifications.Where(o => o.IsWrite).ToList());
                AppendValues(commandStringBuilder, modificationCommands[i].ColumnModifications.Where(o => o.IsWrite).ToList());
                if (readOperations.Length > 0)
                    AppendInsertOutputClause(commandStringBuilder, name, schema, readOperations, operations);

            }
            commandStringBuilder.AppendLine("END;");
            commandStringBuilder.Replace("@p", ":p"); 
            return ResultSetMapping.NotLastInResultSet;
        }
       
        
        public override ResultSetMapping AppendUpdateOperation(
            StringBuilder commandStringBuilder,
            ModificationCommand command,
            int commandPosition)
        => AppendBlockUpdateOperation(commandStringBuilder, new[] { command }, commandPosition);
         

        public override ResultSetMapping AppendDeleteOperation(
           StringBuilder commandStringBuilder,
           ModificationCommand command,
           int commandPosition)
        =>  AppendBlockDeleteOperation(commandStringBuilder, new[] { command }, commandPosition);

        


        public ResultSetMapping AppendBlockUpdateOperation(StringBuilder commandStringBuilder, IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(modificationCommands, nameof(modificationCommands));
            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;  
            commandStringBuilder.Clear();
            commandStringBuilder.Append("EXECUTE BLOCK ");
            if (modificationCommands.Any())
            {
                commandStringBuilder.AppendLine("(");
                var separator = string.Empty;
                for (var i = 0; i < modificationCommands.Count; i++)
                {
                    var operations = modificationCommands[i].ColumnModifications;
                    var writeOperations = operations.Where(o => o.IsWrite).ToArray();
                    var readOperations = operations.Where(o => o.IsRead).ToArray();
                    foreach (var param in writeOperations)
                    {
                        commandStringBuilder.Append(separator);
                        commandStringBuilder.Append(param.ParameterName.Replace("@", string.Empty));
                        commandStringBuilder.Append(" ");
                        commandStringBuilder.Append(" VARCHAR(100) ");
                        commandStringBuilder.Append(" = ");
                        commandStringBuilder.Append("@" + param.ParameterName);
                        separator = ", ";
                    }
                }
                commandStringBuilder.AppendLine();
                commandStringBuilder.Append(") ");
                commandStringBuilder.AppendLine("RETURNS (regAffeted INT) AS BEGIN")
                                   .AppendLine("regAffeted=0;");
            }
            

            for (var i = 0; i < modificationCommands.Count; i++)
            {
                var operations = modificationCommands[i].ColumnModifications;
                var writeOperations = operations.Where(o => o.IsWrite).ToArray();
                var readOperations = operations.Where(o => o.IsRead).ToArray();
                var condicoes = operations.Where(o => o.IsCondition).ToArray();
                commandStringBuilder.Append($"UPDATE {SqlGenerationHelper.DelimitIdentifier(name)} SET ")
                .AppendJoinUpadate(
                    writeOperations,
                    SqlGenerationHelper,
                    (sb, o, helper) =>
                    {
                        if (o.IsWrite)
                            sb.Append($"{SqlGenerationHelper.DelimitIdentifier(o.ColumnName)}=:{o.ParameterName} "); 
                    });

                AppendWhereClauseCustoom(commandStringBuilder, condicoes);
                commandStringBuilder.AppendLine(SqlGenerationHelper.StatementTerminator); 
                AppendUpdateOutputClause(commandStringBuilder);
            }
            commandStringBuilder.AppendLine("SUSPEND;");
            commandStringBuilder.AppendLine("END;"); 
            return ResultSetMapping.NotLastInResultSet;
        }

        private void AppendWhereClauseCustoom(StringBuilder commandStringBuilder, ColumnModification[] col)
        {
            if (col.Any())
            {
                commandStringBuilder.Append(" WHERE ");
                var And = string.Empty;
                foreach (var item in col)
                {
                    commandStringBuilder
                        .Append(And)
                        .Append(SqlGenerationHelper.DelimitIdentifier(item.ColumnName))
                        .Append("=")
                        .Append(item.Value);
                }
            }
        

        }

        public ResultSetMapping AppendBlockDeleteOperation(StringBuilder commandStringBuilder, IReadOnlyList<ModificationCommand> modificationCommands,
           int commandPosition)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(modificationCommands, nameof(modificationCommands));
            var name = modificationCommands[0].TableName;
            var schema = modificationCommands[0].Schema;

            commandStringBuilder.AppendLine("EXECUTE BLOCK RETURNS (regAffeted INT) AS BEGIN")
                                .AppendLine($"regAffeted=0;");

            for (var i = 0; i < modificationCommands.Count; i++)
            {
                var operations = modificationCommands[i].ColumnModifications;
                var writeOperations = operations.Where(o => o.IsWrite).ToArray();
                var readOperations = operations.Where(o => o.IsRead).ToArray();
                commandStringBuilder.Append($"DELETE FROM {SqlGenerationHelper.DelimitIdentifier(name)} ");
                commandStringBuilder.AppendLine($" WHERE {SqlGenerationHelper.DelimitIdentifier(operations.First().ColumnName)}={operations[0].Value}; ");
                AppendUpdateOutputClause(commandStringBuilder);
            }
            commandStringBuilder.AppendLine("SUSPEND;");
            commandStringBuilder.AppendLine("END;");
            return ResultSetMapping.NotLastInResultSet;
        }


        private void AppendUpdateOutputClause(StringBuilder commandStringBuilder)
        {
            //Increment of updates 
            commandStringBuilder
                    .AppendLine("IF (ROW_COUNT > 0) THEN")
                    .AppendLine("   regAffeted=regAffeted+1;");

        }


        private void AppendInsertOutputClause(
            StringBuilder commandStringBuilder,
            string name,
            string schema,
            IReadOnlyList<ColumnModification> operations,
            IReadOnlyList<ColumnModification> allOperations)
        {
            if (allOperations.Count > 0 && allOperations[0] == operations[0])
            {
                commandStringBuilder
                    .AppendLine($" RETURNING {SqlGenerationHelper.DelimitIdentifier(operations.First().ColumnName)} INTO :regAffeted;")
                    .AppendLine("IF (ROW_COUNT > 0) THEN")
                    .AppendLine("   SUSPEND;");
            }
        }

        protected override ResultSetMapping AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, string name,
            string schema, int commandPosition)
        {
            // Not Implemented!
            return ResultSetMapping.LastInResultSet;
        }

        public override void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
            //Insert FirebirdSqlSQL Fast(Insert/Update) 
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
        {
            // Not Implemented!
        }

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
        {
            // Not Implemented!
        }

    }
}
