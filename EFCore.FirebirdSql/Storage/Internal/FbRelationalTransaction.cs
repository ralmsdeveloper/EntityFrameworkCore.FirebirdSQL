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
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{
    public class FbRelationalTransaction : RelationalTransaction
    {
        private readonly IRelationalConnection _relationalConnection;
        private readonly DbTransaction _dbTransaction;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> _logger;
        private readonly bool _transactionOwned;
        private bool _connectionClosed;

        public FbRelationalTransaction(IRelationalConnection connection, DbTransaction transaction, IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> logger, bool transactionOwned)
            : base(connection, transaction, logger, transactionOwned)
        {
            if (connection.DbConnection != transaction.Connection)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAssociatedWithDifferentConnection);
            }
            _relationalConnection = connection;
            _dbTransaction = transaction;
            _logger = logger;
            _transactionOwned = transactionOwned;
        }

        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await Task.Run(() => (_dbTransaction as FbTransaction)?.Commit(), cancellationToken);
                _logger.TransactionCommitted(_relationalConnection, _dbTransaction, TransactionId, startTime, stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                _logger.TransactionError(_relationalConnection, _dbTransaction, TransactionId, nameof(CommitAsync), e, startTime, stopwatch.Elapsed);
                throw;
            }
            ClearTransaction();
        }

        public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await Task.Run(() => (_dbTransaction as FbTransaction)?.Rollback(), cancellationToken);
                _logger.TransactionRolledBack(_relationalConnection, _dbTransaction, TransactionId, startTime, stopwatch.Elapsed);
            }
            catch (Exception e)
            {
                _logger.TransactionError(_relationalConnection, _dbTransaction, TransactionId, nameof(RollbackAsync), e, startTime, stopwatch.Elapsed);
                throw;
            }
            ClearTransaction();
        }

        private void ClearTransaction()
        {
            _relationalConnection.UseTransaction(null);
            if (_connectionClosed)
                return;

            _connectionClosed = true;
            _relationalConnection.Close();
        }
    }
}
