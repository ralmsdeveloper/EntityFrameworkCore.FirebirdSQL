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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.FirebirdSql.Update.Internal
{
    public class FbModificationCommandBatch : ReaderModificationCommandBatch
    {
        private const int MaxParameterCount = 1000;
        private const int MaxRowCount = 256;
        private int _countParameter = 1;
        private readonly int _maxBatchSize;
        private readonly List<ModificationCommand> _bulkInsertCommands;
        private readonly List<ModificationCommand> _bulkUpdateCommands;
        private readonly List<ModificationCommand> _bulkDeleteCommands;
        private readonly StringBuilder _variablesParameters;
        private readonly StringBuilder _dataReturnField;

        public FbModificationCommandBatch(IRelationalCommandBuilderFactory commandBuilderFactory, ISqlGenerationHelper sqlGenerationHelper, IFbUpdateSqlGenerator updateSqlGenerator, IRelationalValueBufferFactoryFactory valueBufferFactoryFactory, int? maxBatchSize)
            : base(commandBuilderFactory, sqlGenerationHelper, updateSqlGenerator, valueBufferFactoryFactory)
        {
            if (maxBatchSize.HasValue && maxBatchSize.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxBatchSize), RelationalStrings.InvalidMaxBatchSize);
            }

            _maxBatchSize = Math.Min(maxBatchSize ?? int.MaxValue, MaxRowCount);
            _variablesParameters = new StringBuilder();
            _dataReturnField = new StringBuilder();
            _bulkInsertCommands = new List<ModificationCommand>();
            _bulkUpdateCommands = new List<ModificationCommand>();
            _bulkDeleteCommands = new List<ModificationCommand>();
        }

        protected new virtual IFbUpdateSqlGenerator UpdateSqlGenerator()
            => (IFbUpdateSqlGenerator)base.UpdateSqlGenerator;

        protected override bool CanAddCommand(ModificationCommand modificationCommand)
        {
            if (ModificationCommands.Count >= _maxBatchSize)
            {
                return false;
            }

            var additionalParameterCount = CountParameters(modificationCommand);
            if (_countParameter + additionalParameterCount >= MaxParameterCount)
            {
                return false;
            }

            _countParameter += additionalParameterCount;
            return true;
        }

        protected override bool IsCommandTextValid() => true;

        protected override int GetParameterCount() => _countParameter;

        private static int CountParameters(ModificationCommand modificationCommand)
        {
            var parameterCount = 0;
            foreach (var columnModification in modificationCommand.ColumnModifications)
            {
                if (columnModification.UseCurrentValueParameter)
                {
                    parameterCount++;
                }

                if (columnModification.UseOriginalValueParameter)
                {
                    parameterCount++;
                }
            }
            return parameterCount;
        }

        protected override void ResetCommandText()
        {
            base.ResetCommandText();
            _variablesParameters.Clear();
            _dataReturnField.Clear();
            _bulkInsertCommands.Clear();
            _bulkUpdateCommands.Clear();
            _bulkDeleteCommands.Clear();
        }

        protected override string GetCommandText()
        {
            var sbCommands = new StringBuilder();
            var sbExecuteBlock = new StringBuilder();
            _variablesParameters.Clear();
            _dataReturnField.Clear();

            //Commands Insert/Update/Delete
            sbCommands.AppendLine(base.GetCommandText());
            sbCommands.Append(GetBlockInsertCommandText(ModificationCommands.Count));
            sbCommands.Append(GetBlockUpdateCommandText(ModificationCommands.Count));
            sbCommands.Append(GetBlockDeleteCommandText(ModificationCommands.Count));

            //Execute Block
            var parameters = _variablesParameters.ToString();
            sbExecuteBlock.Append("EXECUTE BLOCK ");
            if (parameters.Length > 0)
            {
                sbExecuteBlock.AppendLine("( ");
                sbExecuteBlock.Append(parameters);
                sbExecuteBlock.AppendLine(") ");
            }

            sbExecuteBlock.Append(_dataReturnField);
            sbExecuteBlock.Append(sbCommands);
            sbExecuteBlock.AppendLine("END;");

            return sbExecuteBlock.ToString();
        }

        private string GetBlockDeleteCommandText(int lastIndex)
        {
            if (_bulkDeleteCommands.Count == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            var resultSetMapping = UpdateSqlGenerator()
                .AppendBulkDeleteOperation(stringBuilder, _variablesParameters, _dataReturnField, _bulkDeleteCommands, lastIndex - _bulkDeleteCommands.Count);

            for (var i = lastIndex - _bulkDeleteCommands.Count; i < lastIndex; i++)
            {
                CommandResultSet[i] = resultSetMapping;
            }

            if (resultSetMapping != ResultSetMapping.NoResultSet)
            {
                CommandResultSet[lastIndex - 1] = ResultSetMapping.LastInResultSet;
            }

            return stringBuilder.ToString();
        }

        private string GetBlockUpdateCommandText(int lastIndex)
        {
            if (_bulkUpdateCommands.Count == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            var headStringBuilder = new StringBuilder();
            var resultSetMapping = UpdateSqlGenerator()
                .AppendBulkUpdateOperation(stringBuilder, _variablesParameters, _dataReturnField, _bulkUpdateCommands, lastIndex - _bulkUpdateCommands.Count);

            for (var i = lastIndex - _bulkUpdateCommands.Count; i < lastIndex; i++)
            {
                CommandResultSet[i] = resultSetMapping;
            }

            if (resultSetMapping != ResultSetMapping.NoResultSet)
            {
                CommandResultSet[lastIndex - 1] = ResultSetMapping.LastInResultSet;
            }

            return stringBuilder.ToString();
        }

        private string GetBlockInsertCommandText(int lastIndex)
        {
            if (_bulkInsertCommands.Count == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            var resultSetMapping = UpdateSqlGenerator()
                .AppendBulkInsertOperation(stringBuilder, _variablesParameters, _dataReturnField, _bulkInsertCommands, lastIndex - _bulkInsertCommands.Count);

            for (var i = lastIndex - _bulkInsertCommands.Count; i < lastIndex; i++)
            {
                CommandResultSet[i] = resultSetMapping;
            }

            if (resultSetMapping != ResultSetMapping.NoResultSet)
            {
                CommandResultSet[lastIndex - 1] = ResultSetMapping.LastInResultSet;
            }

            return stringBuilder.ToString();
        }

        protected override void UpdateCachedCommandText(int commandPosition)
        {
            var newModificationCommand = ModificationCommands[commandPosition];

            if (newModificationCommand.EntityState == EntityState.Added)
            {
                if (_bulkInsertCommands.Count > 0
                    && !CanBeInsertedInSameStatement(_bulkInsertCommands[0], newModificationCommand))
                {
                    CachedCommandText.Append(GetBlockInsertCommandText(commandPosition));
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
                    CachedCommandText.Append(GetBlockUpdateCommandText(commandPosition));
                    _bulkUpdateCommands.Clear();
                }
                _bulkUpdateCommands.Add(newModificationCommand);
                LastCachedCommandIndex = commandPosition;
            }
            else if (newModificationCommand.EntityState == EntityState.Deleted)
            {
                if (_bulkDeleteCommands.Count > 0
                    && !CanBeDeleteInSameStatement(_bulkDeleteCommands[0], newModificationCommand))
                {
                    CachedCommandText.Append(GetBlockDeleteCommandText(commandPosition));
                    _bulkDeleteCommands.Clear();
                }
                _bulkDeleteCommands.Add(newModificationCommand);
                LastCachedCommandIndex = commandPosition;
            }
            else
            {
                CachedCommandText.Append(GetBlockInsertCommandText(commandPosition));
                _bulkInsertCommands.Clear();
                base.UpdateCachedCommandText(commandPosition);
            }
        }

        private static bool CanBeDeleteInSameStatement(ModificationCommand firstCommand, ModificationCommand secondCommand)
            => string.Equals(firstCommand.TableName, secondCommand.TableName, StringComparison.Ordinal)
                   && firstCommand.ColumnModifications.Where(o => o.IsWrite)
                                  .Select(o => o.ColumnName)
                                  .SequenceEqual(secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
                   && firstCommand.ColumnModifications.Where(o => o.IsRead)
                                  .Select(o => o.ColumnName)
                                  .SequenceEqual(secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));

        private static bool CanBeUpdateInSameStatement(ModificationCommand firstCommand, ModificationCommand secondCommand)
            => string.Equals(firstCommand.TableName, secondCommand.TableName, StringComparison.Ordinal)
                   && firstCommand.ColumnModifications.Where(o => o.IsWrite)
                                  .Select(o => o.ColumnName)
                                  .SequenceEqual(secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
                   && firstCommand.ColumnModifications.Where(o => o.IsRead)
                                  .Select(o => o.ColumnName)
                                  .SequenceEqual(secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));

        private static bool CanBeInsertedInSameStatement(ModificationCommand firstCommand, ModificationCommand secondCommand)
            => string.Equals(firstCommand.TableName, secondCommand.TableName, StringComparison.Ordinal)
                   && firstCommand.ColumnModifications.Where(o => o.IsWrite)
                                  .Select(o => o.ColumnName)
                                  .SequenceEqual(secondCommand.ColumnModifications.Where(o => o.IsWrite).Select(o => o.ColumnName))
                   && firstCommand.ColumnModifications.Where(o => o.IsRead)
                                  .Select(o => o.ColumnName)
                                  .SequenceEqual(secondCommand.ColumnModifications.Where(o => o.IsRead).Select(o => o.ColumnName));

        protected override void Consume(RelationalDataReader relationalReader)
        {
            if (relationalReader == null)
            {
                throw new ArgumentNullException(nameof(relationalReader));
            }
            var dataReader = relationalReader.DbDataReader;
            var commandIndex = 0;
            try
            {
                while (true)
                {
                    while (commandIndex < CommandResultSet.Count && CommandResultSet[commandIndex] == ResultSetMapping.NoResultSet)
                    {
                        commandIndex++;
                    }

                    var propragation = commandIndex;
                    while (propragation < ModificationCommands.Count && !ModificationCommands[propragation].RequiresResultPropagation)
                    {
                        propragation++;
                    }

                    while (commandIndex < propragation)
                    {
                        commandIndex++;
                        if (!dataReader.Read())
                        {
                            throw new DbUpdateConcurrencyException(
                                RelationalStrings.UpdateConcurrencyException(
                                ModificationCommands.Count(m => m.RequiresResultPropagation), 0),
                                ModificationCommands[commandIndex].Entries);
                        }
                    }

                    //check if you've gone through all notifications
                    if (propragation == ModificationCommands.Count)
                    {
                        break;
                    }

                    var modifications = ModificationCommands[commandIndex];
                    if (!relationalReader.Read())
                    {
                        throw new DbUpdateConcurrencyException(
                            RelationalStrings.UpdateConcurrencyException(
                            ModificationCommands.Count(m => m.RequiresResultPropagation), 0),
                            ModificationCommands[commandIndex].Entries);
                    }

                    var bufferFactory = CreateValueBufferFactory(modifications.ColumnModifications);
                    var buffer = bufferFactory.Create(dataReader);
                    modifications.PropagateResults(bufferFactory.Create(dataReader));
                    dataReader.NextResult();

                    commandIndex++;
                }
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException(
                    RelationalStrings.UpdateStoreException,
                    ex, ModificationCommands[commandIndex].Entries);
            }
        }

        protected override async Task ConsumeAsync(
            RelationalDataReader relationalReader,
            CancellationToken cancellationToken = default)
        {
            if (relationalReader == null)
            {
                throw new ArgumentNullException(nameof(relationalReader));
            }

            var dataReader = relationalReader.DbDataReader;
            var commandIndex = 0;
            try
            {
                while (true)
                {
                    while (commandIndex < CommandResultSet.Count && CommandResultSet[commandIndex] == ResultSetMapping.NoResultSet)
                    {
                        commandIndex++;
                    }

                    var propragation = commandIndex;
                    while (propragation < ModificationCommands.Count && !ModificationCommands[propragation].RequiresResultPropagation)
                    {
                        propragation++;
                    }

                    while (commandIndex < propragation)
                    {
                        commandIndex++;
                        if (!(await relationalReader.ReadAsync()))
                        {
                            throw new DbUpdateConcurrencyException(
                                RelationalStrings.UpdateConcurrencyException(
                                ModificationCommands.Count(m => m.RequiresResultPropagation), 0),
                                ModificationCommands[commandIndex].Entries);
                        }
                    }

                    if (propragation == ModificationCommands.Count)
                    {
                        break;
                    }

                    var modifications = ModificationCommands[commandIndex];
                    if (!(await relationalReader.ReadAsync()))
                    {
                        throw new DbUpdateConcurrencyException(
                            RelationalStrings.UpdateConcurrencyException(
                            ModificationCommands.Count(m => m.RequiresResultPropagation), 0),
                            ModificationCommands[commandIndex].Entries);
                    }

                    var bufferFactory = CreateValueBufferFactory(modifications.ColumnModifications);
                    modifications.PropagateResults(bufferFactory.Create(dataReader));
                    await dataReader.NextResultAsync();
                    commandIndex++;
                }
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException(
                    RelationalStrings.UpdateStoreException,
                    ex, ModificationCommands[commandIndex].Entries);
            }
        }
    }
}
