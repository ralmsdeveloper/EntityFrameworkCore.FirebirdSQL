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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using FirebirdSql.Data.FirebirdClient;


namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class FirebirdSqlDatabaseCreator : RelationalDatabaseCreator
    {
        private readonly IFirebirdSqlRelationalConnection _connection;
	    private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;
        

	    /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
	    public FirebirdSqlDatabaseCreator(
            [NotNull] RelationalDatabaseCreatorDependencies dependencies,
            [NotNull] IFirebirdSqlRelationalConnection connection,
            [NotNull] IRawSqlCommandBuilder rawSqlCommandBuilder)
            : base(dependencies)
        { 
            _connection = connection;
            _rawSqlCommandBuilder = rawSqlCommandBuilder;
        }

        public virtual TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);

        public virtual TimeSpan RetryTimeout { get; set; } = TimeSpan.FromMinutes(2);

	    public override void Create()
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                FbConnection.CreateDatabase(_connection.ConnectionString);
                //Dependencies.MigrationCommandExecutor
                //    .ExecuteNonQuery(CreateCreateOperations(), masterConnection);

                ClearPool();
            }

            Exists(retryOnNotExists: true);
        }

        public override async Task CreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var masterConnection = _connection.CreateMasterConnection())
            {
                await Dependencies.MigrationCommandExecutor.ExecuteNonQueryAsync(CreateCreateOperations(), masterConnection, cancellationToken).ConfigureAwait(false);

                ClearPool();
            }

            await ExistsAsync(retryOnNotExists: true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        protected override bool HasTables()
        {
            var count = (long)CreateHasTablesCommand().ExecuteScalar(Dependencies.Connection); 
            return count != 0;
        }
        
        protected override Task<bool> HasTablesAsync(CancellationToken cancellationToken = default(CancellationToken))
            => Dependencies.ExecutionStrategyFactory.Create().ExecuteAsync(_connection,
                async (connection, ct) => (long)await CreateHasTablesCommand().ExecuteScalarAsync(connection, cancellationToken: ct).ConfigureAwait(false) != 0, cancellationToken);
 

        private IRelationalCommand CreateHasTablesCommand()
            => _rawSqlCommandBuilder
                .Build(@"select count(*) from rdb$relations where rdb$view_blr is null and (rdb$system_flag is null or rdb$system_flag = 0);");

        private IReadOnlyList<MigrationCommand> CreateCreateOperations()
        {
            var builder = new FbConnectionStringBuilder(_connection.DbConnection.ConnectionString);
            return Dependencies.MigrationsSqlGenerator.Generate((new[] { new FirebirdSqlCreateDatabaseOperation { Name = _connection.DbConnection.Database } }));
        }
        public override bool Exists()
            => Exists(retryOnNotExists: false);

        private bool Exists(bool retryOnNotExists)
            => Dependencies.ExecutionStrategyFactory.Create().Execute(DateTime.UtcNow + RetryTimeout, giveUp =>
                {
                    while (true)
                    {
                        try
                        {
                            _connection.DbConnection.Open();
                            _connection.DbConnection.Close();
                            return true;
                        }
                        catch (FbException e)
                        {
                            if (!retryOnNotExists && IsDoesNotExist(e))
                                return false;

                            if (DateTime.UtcNow > giveUp || !RetryOnExistsFailure(e))
                                throw;

                            Thread.Sleep(RetryDelay);
                        }
                    }
                });

        public override Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
            => ExistsAsync(retryOnNotExists: false, cancellationToken: cancellationToken);

        private Task<bool> ExistsAsync(bool retryOnNotExists, CancellationToken cancellationToken)
            => Dependencies.ExecutionStrategyFactory.Create().ExecuteAsync(DateTime.UtcNow + RetryTimeout, async (giveUp, ct) =>
                {
                    while (true)
                    {
                        try
                        {
                            await _connection.DbConnection.OpenAsync(ct).ConfigureAwait(false);
                            _connection.DbConnection.Close();
                            return true;
                        }
                        catch (FbException e)
                        {
                            if (!retryOnNotExists && IsDoesNotExist(e))
                                return false;

                            if (DateTime.UtcNow > giveUp || !RetryOnExistsFailure(e))
                                throw;

                            await Task.Delay(RetryDelay, ct).ConfigureAwait(false);
                        }
                    }
                }, cancellationToken);
         
        private static bool IsDoesNotExist(FbException exception) => exception.ErrorCode == 335544344;
         
        private bool RetryOnExistsFailure(FbException exception)
        {
            if (exception.ErrorCode == 335544721)
            {
                ClearPool();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void Delete()
        {
            ClearAllPools();
            FbConnection.DropDatabase(_connection.ConnectionString);
           
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override async Task DeleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(()=>Delete()) ; 
        }

        private IReadOnlyList<MigrationCommand> CreateDropCommands()
        {
            var operations = new MigrationOperation[]
            { 
                new FirebirdSqlDropDatabaseOperation { Name = _connection.DbConnection.Database }
            };

            var masterCommands = Dependencies.MigrationsSqlGenerator.Generate(operations);
            return masterCommands;
        }
         
        private static void ClearAllPools() => FbConnection.ClearAllPools(); 
        private void ClearPool() => FbConnection.ClearPool(_connection.DbConnection as FbConnection);
    }
}
