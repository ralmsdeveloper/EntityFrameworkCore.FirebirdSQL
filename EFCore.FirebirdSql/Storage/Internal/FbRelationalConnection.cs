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

using System.Data.Common;
using System.Threading.Tasks;
using System.Data;
using System.Threading;
using System;
using Microsoft.EntityFrameworkCore.Internal;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{
    public class FbRelationalConnection : RelationalConnection, IFbRelationalConnection
    {
        public FbRelationalConnection(RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override DbConnection CreateDbConnection()
            => new FbConnection(ConnectionString);

        public virtual IFbRelationalConnection CreateMasterConnection()
        {
            var csb = new FbConnectionStringBuilder(ConnectionString)
            {
                Pooling = false
            };

            var contextOptions = new DbContextOptionsBuilder()
                .UseFirebird(csb.ConnectionString)
                .Options;

            return new FbRelationalConnection(Dependencies.With(contextOptions));
        }

        public override bool IsMultipleActiveResultSetsEnabled => true;

        public override async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction != null)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
            }
            await OpenAsync(cancellationToken).ConfigureAwait(false);
            return BeginTransactionWithNoPreconditions(isolationLevel, cancellationToken);
        }

        private IDbContextTransaction BeginTransactionWithNoPreconditions(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        {
            var dbTransaction = ((FbConnection)DbConnection).BeginTransaction(isolationLevel);
            CurrentTransaction = new FbRelationalTransaction(this, dbTransaction, Dependencies.TransactionLogger, true);
            Dependencies.TransactionLogger.TransactionStarted(this, dbTransaction, CurrentTransaction.TransactionId, DateTimeOffset.UtcNow);
            return CurrentTransaction;
        }

        public override IDbContextTransaction UseTransaction(DbTransaction transaction)
        {
            if (transaction == null)
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction = null;
                }
            }
            else
            {
                if (CurrentTransaction != null)
                {
                    throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
                }

                Open();
                CurrentTransaction = new FbRelationalTransaction(this, transaction, Dependencies.TransactionLogger, transactionOwned: false);
                Dependencies.TransactionLogger.TransactionUsed(this, transaction, CurrentTransaction.TransactionId, DateTimeOffset.UtcNow);
            }
            return CurrentTransaction;
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }
            await ((FbRelationalTransaction)CurrentTransaction).CommitAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }
            await ((FbRelationalTransaction)CurrentTransaction).RollbackAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
