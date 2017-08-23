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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Internal;


namespace Microsoft.EntityFrameworkCore.Update.Internal
{ 

    public class FirebirdSqlModificationCommandBatch : AffectedCountModificationCommandBatch
    {
        const int DefaultNetworkPacketSizeBytes = 4096;
        const int MaxScriptLength = 65536 * DefaultNetworkPacketSizeBytes / 2;
        const int MaxParameterCount = 2100;
        const int MaxRowCount = 256;
        int CountParameter = 1; 
        readonly int _maxBatchSize;
        readonly List<ModificationCommand> _bulkInsertCommands = new List<ModificationCommand>();
        readonly List<ModificationCommand> _bulkUpdateCommands = new List<ModificationCommand>();
        int _commandsLeftToLengthCheck = 50; 
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public FirebirdSqlModificationCommandBatch(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper,
            // ReSharper disable once SuggestBaseTypeForParameter
            [NotNull] IFirebirdSqlUpdateSqlGenerator updateSqlGenerator,
            [NotNull] IRelationalValueBufferFactoryFactory valueBufferFactoryFactory,
            [CanBeNull] int? maxBatchSize)
            : base( commandBuilderFactory, sqlGenerationHelper, updateSqlGenerator,valueBufferFactoryFactory)
        {
            if (maxBatchSize.HasValue && maxBatchSize.Value <= 0) 
                throw new ArgumentOutOfRangeException(nameof(maxBatchSize), RelationalStrings.InvalidMaxBatchSize);
           
            _maxBatchSize = Math.Min(maxBatchSize ?? int.MaxValue, MaxRowCount);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected new virtual IFirebirdSqlUpdateSqlGenerator UpdateSqlGenerator => (IFirebirdSqlUpdateSqlGenerator)base.UpdateSqlGenerator;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override bool CanAddCommand(ModificationCommand modificationCommand)
        {
            if (ModificationCommands.Count >= _maxBatchSize) 
                return false;
          

            var additionalParameterCount = CountParameters(modificationCommand); 
            if (CountParameter + additionalParameterCount >= MaxParameterCount) 
                return false;
          

            CountParameter += additionalParameterCount;
            return true;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override bool IsCommandTextValid()
            =>  true;
        

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override int GetParameterCount()
            => CountParameter;

        private static int CountParameters(ModificationCommand modificationCommand)
        {
            var parameterCount = 0;
            foreach (var columnModification in modificationCommand.ColumnModifications)
            {
                if (columnModification.UseCurrentValueParameter) 
                    parameterCount++; 

                if (columnModification.UseOriginalValueParameter) 
                    parameterCount++;
                
            }

            return parameterCount;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void ResetCommandText()
        {
            base.ResetCommandText();
            _bulkInsertCommands.Clear();
            _bulkUpdateCommands.Clear();
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override string GetCommandText()
            => base.GetCommandText() + GetBulkInsertCommandText(ModificationCommands.Count) + GetBulkUpdateCommandText(ModificationCommands.Count);

        private string GetBulkUpdateCommandText(int lastIndex)
        {
            if (_bulkUpdateCommands.Count == 0) 
                return string.Empty;
            

            var stringBuilder = new StringBuilder();
            var resultSetMapping = UpdateSqlGenerator.AppendBulkUpdateOperation(stringBuilder, _bulkUpdateCommands, lastIndex - _bulkUpdateCommands.Count);
            for (var i = lastIndex - _bulkUpdateCommands.Count; i < lastIndex; i++)
                CommandResultSet[i] = resultSetMapping; 

            if (resultSetMapping != ResultSetMapping.NoResultSet) 
                CommandResultSet[lastIndex - 1] = ResultSetMapping.LastInResultSet;
         
            if (CommandResultSet.Count == lastIndex)
                stringBuilder.AppendLine("END;");

            return stringBuilder.ToString();
        }

        private string GetBulkInsertCommandText(int lastIndex)
        {
            if (_bulkInsertCommands.Count == 0) 
                return string.Empty;
       
            var stringBuilder = new StringBuilder();
            var resultSetMapping = UpdateSqlGenerator.AppendBulkInsertOperation(stringBuilder, _bulkInsertCommands, lastIndex - _bulkInsertCommands.Count);
            for (var i = lastIndex - _bulkInsertCommands.Count; i < lastIndex; i++)
                CommandResultSet[i] = resultSetMapping;
            

            if (resultSetMapping != ResultSetMapping.NoResultSet) 
                CommandResultSet[lastIndex - 1] = ResultSetMapping.LastInResultSet;
         
            if (CommandResultSet.Count == lastIndex)
                stringBuilder.AppendLine("END;");
            
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override void UpdateCachedCommandText(int commandPosition)
        {
            var newModificationCommand = ModificationCommands[commandPosition];

            if (newModificationCommand.EntityState == EntityState.Added)
            {
                if (_bulkInsertCommands.Count > 0
                    && !CanBeInsertedInSameStatement(_bulkInsertCommands[0], newModificationCommand))
                {
                    CachedCommandText.Append(GetBulkInsertCommandText(commandPosition));
                    _bulkInsertCommands.Clear();
                }
                _bulkInsertCommands.Add(newModificationCommand); 
                LastCachedCommandIndex = commandPosition;
            }
            else if (newModificationCommand.EntityState == EntityState.Modified)
            {
                if (_bulkUpdateCommands.Count > 0
                    && !CanBeUpdateInSameStatement(_bulkUpdateCommands[0], newModificationCommand))
                {
                    CachedCommandText.Append(GetBulkUpdateCommandText(commandPosition));
                    _bulkUpdateCommands.Clear();
                }
                _bulkUpdateCommands.Add(newModificationCommand); 
                LastCachedCommandIndex = commandPosition;
            }
            else
            {
                CachedCommandText.Append(GetBulkInsertCommandText(commandPosition));
                _bulkInsertCommands.Clear(); 
                base.UpdateCachedCommandText(commandPosition);
            }
        }

        private static bool CanBeUpdateInSameStatement(ModificationCommand firstCommand, ModificationCommand secondCommand)
    => string.Equals(firstCommand.TableName, secondCommand.TableName, StringComparison.Ordinal)
       && string.Equals(firstCommand.Schema, secondCommand.Schema, StringComparison.Ordinal)
       && firstCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName).SequenceEqual(
           secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
       && firstCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName).SequenceEqual(
           secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));

        private static bool CanBeInsertedInSameStatement(ModificationCommand firstCommand, ModificationCommand secondCommand)
            => string.Equals(firstCommand.TableName, secondCommand.TableName, StringComparison.Ordinal)
               && string.Equals(firstCommand.Schema, secondCommand.Schema, StringComparison.Ordinal)
               && firstCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName).SequenceEqual(
                   secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
               && firstCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName).SequenceEqual(
                   secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));
    }
}
