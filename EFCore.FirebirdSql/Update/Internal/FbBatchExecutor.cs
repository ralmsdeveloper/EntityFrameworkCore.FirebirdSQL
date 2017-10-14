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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using EntityFrameworkCore.FirebirdSql.Storage.Internal;

namespace EntityFrameworkCore.FirebirdSql.Update.Internal
{ 
    public class FbBatchExecutor : IBatchExecutor
    { 
        public int Execute(IEnumerable<ModificationCommandBatch> commandBatches, IRelationalConnection connection)
        {
            var recordAffecteds = 0;
            if(connection?.DbConnection?.State != System.Data.ConnectionState.Open)
                connection.Open();

            IDbContextTransaction currentTransaction = null;
            try
            {
                if (connection.CurrentTransaction == null) 
                    currentTransaction = connection.BeginTransaction(); 

                foreach (var commandbatch in commandBatches)
                {
                    commandbatch.Execute(connection);
                    recordAffecteds += commandbatch.ModificationCommands.Count;
                } 
                currentTransaction?.Commit();
                currentTransaction?.Dispose();
            }
            catch(Exception ex)
            {
                try
                {
                    currentTransaction?.Rollback();
                    currentTransaction?.Dispose();
                }
                catch
                {
                    //
                }
                throw ex;
            }
            finally
            {
                connection?.Close();
            } 
            return recordAffecteds;
        }

        public async Task<int> ExecuteAsync( IEnumerable<ModificationCommandBatch> commandBatches,IRelationalConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            var RowsAffecteds = 0;
            await connection.OpenAsync(cancellationToken, false).ConfigureAwait(false);
            FbRelationalTransaction currentTransaction = null;
            try
            {
                if (connection.CurrentTransaction == null) 
                    currentTransaction = await ((FbRelationalConnection)connection).BeginTransactionAsync(cancellationToken).ConfigureAwait(false) as FbRelationalTransaction;
                
                foreach (var commandbatch in commandBatches)
                {
                    await commandbatch.ExecuteAsync(connection, cancellationToken).ConfigureAwait(false);
                    RowsAffecteds += commandbatch.ModificationCommands.Count;
                }

                if (currentTransaction != null) 
                    await currentTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
           
                currentTransaction?.Dispose();
            }
            catch (Exception err)
            {
                try
                {
                    currentTransaction?.Rollback();
                    currentTransaction?.Dispose();
                }
                catch
                {
                    //
                }
                throw err;
            }
            finally
            {
                connection?.Close();
            } 
            return RowsAffecteds;
        }
    }
}
