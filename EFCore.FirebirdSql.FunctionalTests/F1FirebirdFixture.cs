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
