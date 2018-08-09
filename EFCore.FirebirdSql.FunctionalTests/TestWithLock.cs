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
 
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class TestWithLock
    {
        private TestContext CreateContext() => new TestContext();

        [Fact]
        public void Hint_with_lock()
        {
            using (var db = CreateContext())
            {
                var query = db
                   .Set<Author>()
                   .WithLock()
                   .Select(p=>new { p.AuthorId, p.Active})
                   .ToSql();

                Assert.Equal(
                    query,
                    @"SELECT ""p"".""AuthorId"", ""p"".""Active""
FROM ""Author"" ""p"" WITH LOCK");
            }
        }
    }
}
