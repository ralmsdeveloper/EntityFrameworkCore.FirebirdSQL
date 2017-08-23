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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Microsoft.EntityFrameworkCore.Update.Internal
{

    public class FirebirdSqlBatchExecutor : IBatchExecutor
    {

        public int Execute(
            IEnumerable<ModificationCommandBatch> commandBatches,
            IRelationalConnection connection)
        {
            var registrosAfetados = 0;
            connection.Open();
            IDbContextTransaction startedTransaction = null;
            try
            {
                if (connection.CurrentTransaction == null) 
                    startedTransaction = connection.BeginTransaction();
             

                foreach (var commandbatch in commandBatches)
                {
                    commandbatch.Execute(connection);
                    registrosAfetados += commandbatch.ModificationCommands.Count;
                } 
                startedTransaction?.Commit();
                startedTransaction?.Dispose();
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
                try
                {
                    startedTransaction?.Rollback();
                    startedTransaction?.Dispose();
                }
                catch
                {
                    // if the connection was lost, rollback command will fail.  prefer to throw original exception in that case
                }
                throw;
            }
            finally
            {
                connection.Close();
            }

            return registrosAfetados;
        }

        public async Task<int> ExecuteAsync(
            IEnumerable<ModificationCommandBatch> commandBatches,
            IRelationalConnection connection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var registrosAfetados = 0;
            await connection.OpenAsync(cancellationToken, false).ConfigureAwait(false);
            FirebirdSqlRelationalTransaction startedTransaction = null;
            try
            {
                if (connection.CurrentTransaction == null) 
                    startedTransaction = await (connection as FirebirdSqlRelationalConnection).BeginTransactionAsync(cancellationToken).ConfigureAwait(false) as FirebirdSqlRelationalTransaction;
               

                foreach (var commandbatch in commandBatches)
                {
                    await commandbatch.ExecuteAsync(connection, cancellationToken).ConfigureAwait(false);
                    registrosAfetados += commandbatch.ModificationCommands.Count;
                }

                if (startedTransaction != null) 
                    await startedTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
           
                startedTransaction?.Dispose();
            }
            catch (Exception err)
            {
                try
                {
                    startedTransaction?.Rollback();
                    startedTransaction?.Dispose();
                }
                catch
                {
                    // if the connection was lost, rollback command will fail.  prefer to throw original exception in that case
                }
                throw err;
            }
            finally
            {
                connection.Close();
            } 
            return registrosAfetados;
        }
    }
}
