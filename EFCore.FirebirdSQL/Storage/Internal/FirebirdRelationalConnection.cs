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
 
 using System.Data.Common;
using System.Threading.Tasks; 
using JetBrains.Annotations;
using System.Data;
using System.Threading;
using System;
using Microsoft.EntityFrameworkCore.Internal;
using FirebirdSql.Data.FirebirdClient;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class FirebirdSqlRelationalConnection : RelationalConnection, IFirebirdSqlRelationalConnection
    {
        
        public FirebirdSqlRelationalConnection([NotNull] RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override DbConnection CreateDbConnection() => new FbConnection(ConnectionString);

        public virtual IFirebirdSqlRelationalConnection CreateMasterConnection()
        {
            var csb = new FbConnectionStringBuilder(ConnectionString)
            {
                //Database = "",
                Pooling = false
            };

            var contextOptions = new DbContextOptionsBuilder()
                .UseFirebirdSql(csb.ConnectionString)
                .Options;
                
            return new FirebirdSqlRelationalConnection(Dependencies.With(contextOptions));
        }

        public override bool IsMultipleActiveResultSetsEnabled => true;

        [NotNull]
        public override async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (CurrentTransaction != null)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
            }

            await OpenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return await BeginTransactionWithNoPreconditionsAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IDbContextTransaction> BeginTransactionWithNoPreconditionsAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken=default(CancellationToken))
        {
            var dbTransaction = (DbConnection as FbConnection).BeginTransaction(isolationLevel);

            CurrentTransaction
                = new FirebirdSqlRelationalTransaction(
                    this,
                    dbTransaction,
                    Dependencies.TransactionLogger,
                    transactionOwned: true);
            
            Dependencies.TransactionLogger.TransactionStarted(
                this, 
                dbTransaction, 
                CurrentTransaction.TransactionId,
                DateTimeOffset.UtcNow);

            return CurrentTransaction;
        }

        /// <summary>
        ///     Specifies an existing <see cref="DbTransaction" /> to be used for database operations.
        /// </summary>
        /// <param name="transaction"> The transaction to be used. </param>
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

                CurrentTransaction = new FirebirdSqlRelationalTransaction(
                    this, 
                    transaction, 
                    Dependencies.TransactionLogger, 
                    transactionOwned: false);

                Dependencies.TransactionLogger.TransactionUsed(
                    this, 
                    transaction, 
                    CurrentTransaction.TransactionId,
                    DateTimeOffset.UtcNow);
            }

            return CurrentTransaction;
        }

        public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken=default(CancellationToken))
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            await (CurrentTransaction as FirebirdSqlRelationalTransaction).CommitAsync().ConfigureAwait(false);
        }

        public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken=default(CancellationToken))
        {
            if (CurrentTransaction == null)
            {
                throw new InvalidOperationException(RelationalStrings.NoActiveTransaction);
            }

            await (CurrentTransaction as FirebirdSqlRelationalTransaction).RollbackAsync().ConfigureAwait(false);
        }
    }
}
