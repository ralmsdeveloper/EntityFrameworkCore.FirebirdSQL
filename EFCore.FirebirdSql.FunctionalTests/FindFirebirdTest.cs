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

using System.Threading.Tasks;
using EFCore.FirebirdSql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public abstract class FindFirebirdTest : FindTestBase<FindFirebirdTest.FindFirebirdFixture>
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
            protected override ITestStoreFactory TestStoreFactory => FirebirdTestStoreFactory.Instance;
        }
    }
}
