using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public abstract class FindFirebirdTest
        : FindTestBase<FirebirdTestStore, FindFirebirdTest.FindFirebirdFixture>
    {
        protected FindFirebirdTest(FindFirebirdFixture fixture)
            : base(fixture)
        {
        }

        public class FindFirebirdTestSet : FindFirebirdTest
        {
            public FindFirebirdTestSet(FindFirebirdFixture fixture)
                : base(fixture)
            {
            }

            protected override TEntity Find<TEntity>(DbContext context, params object[] keyValues)
                => context.Set<TEntity>().Find(keyValues);

            protected override Task<TEntity> FindAsync<TEntity>(DbContext context, params object[] keyValues)
                => context.Set<TEntity>().FindAsync(keyValues);
        }

        public class FindFirebirdTestContext : FindFirebirdTest
        {
            public FindFirebirdTestContext(FindFirebirdFixture fixture)
                : base(fixture)
            {
            }

            protected override TEntity Find<TEntity>(DbContext context, params object[] keyValues)
                => context.Find<TEntity>(keyValues);

            protected override Task<TEntity> FindAsync<TEntity>(DbContext context, params object[] keyValues)
                => context.FindAsync<TEntity>(keyValues);
        }

        public class FindFirebirdTestNonGeneric : FindFirebirdTest
        {
            public FindFirebirdTestNonGeneric(FindFirebirdFixture fixture)
                : base(fixture)
            {
            }

            protected override TEntity Find<TEntity>(DbContext context, params object[] keyValues)
                => (TEntity)context.Find(typeof(TEntity), keyValues);

            protected override async Task<TEntity> FindAsync<TEntity>(DbContext context, params object[] keyValues)
                => (TEntity)await context.FindAsync(typeof(TEntity), keyValues);
        }

        public class FindFirebirdFixture : FindFixtureBase
        {
            private readonly DbContextOptions _options;
            private readonly string DatabaseName = "FindTest";

            public FindFirebirdFixture()
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkFirebird()
                    .AddSingleton(TestModelSource.GetFactory(OnModelCreating))
                    .BuildServiceProvider(validateScopes: true);

                _options = new DbContextOptionsBuilder()
                    .UseFirebird(FirebirdTestStore.CreateConnectionString(DatabaseName))
                    .UseInternalServiceProvider(serviceProvider)
                    .Options;
            }

            public override FirebirdTestStore CreateTestStore()
                => FirebirdTestStore.GetOrCreateShared(DatabaseName, () =>
                {
                    using (var context = new FindContext(_options))
                    {
                        context.Database.EnsureCreated();
                        Seed(context);
                    }
                });

            public override DbContext CreateContext(FirebirdTestStore testStore)
                => new FindContext(_options);
        }
    }
}
