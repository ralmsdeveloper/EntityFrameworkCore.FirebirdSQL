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

using EFCore.FirebirdSql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class LazyLoadProxyFirebirdTest : LazyLoadProxyTestBase<LazyLoadProxyFirebirdTest.LoadFiribirdFixture>
    {
        public LazyLoadProxyFirebirdTest(LoadFiribirdFixture fixture)
            : base(fixture)
        {
        }

        public class LoadFiribirdFixture : LoadFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => FirebirdTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => base.AddOptions(builder).ConfigureWarnings(
                    c => c
                        .Log(RelationalEventId.QueryClientEvaluationWarning));
        }
    }
}
