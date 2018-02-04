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
using EFCore.FirebirdSql.FunctionalTests;
using Microsoft.EntityFrameworkCore.TestModels.ConcurrencyModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore
{
    public class F1FirebirdFixture : F1RelationalFixture<FirebirdTestStore>
    {
        public static readonly string DatabaseName = "OptimisticConcurrencyTest";

        private readonly IServiceProvider _serviceProvider;

        private readonly string _connectionString = FirebirdTestStore.CreateConnectionString(DatabaseName);

        public TestSqlLoggerFactory TestSqlLoggerFactory { get; } = new TestSqlLoggerFactory();

        public F1FirebirdFixture()
            => _serviceProvider = new ServiceCollection()
                .AddEntityFrameworkFirebird()
                .AddSingleton(TestModelSource.GetFactory(OnModelCreating))
                .AddSingleton<ILoggerFactory>(TestSqlLoggerFactory)
                .BuildServiceProvider(validateScopes: true);

        public override FirebirdTestStore CreateTestStore()
            => FirebirdTestStore
                .GetOrCreateShared(DatabaseName,  initializeDatabase:
                    () =>
                        {
                            var optionsBuilder = new DbContextOptionsBuilder()
                                .UseFirebird(_connectionString)
                                .UseInternalServiceProvider(_serviceProvider);

                            using (var context = new F1Context(optionsBuilder.Options))
                            {
                                context.Database.EnsureDeleted();
                                context.Database.EnsureCreated();
                                ConcurrencyModelInitializer.Seed(context);
                            }
                        });

        public override F1Context CreateContext(FirebirdTestStore testStore)
        {
            var optionsBuilder = new DbContextOptionsBuilder()
                .UseFirebird(testStore.Connection)
                .UseInternalServiceProvider(_serviceProvider);

            var context = new F1Context(optionsBuilder.Options);
            context.Database.UseTransaction(testStore.Transaction);
            return context;
        }
    }
}
