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

using System;
using System.Linq; 
using Xunit; 

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class TestDateTime
    {
        private TestContext CreateContext() => new TestContext();

        [Fact]
        public void DateDiff_days()
        {
            using (var ctx = CreateContext())
            {  
                var sql = ctx
                    .Author
                    .Where(p => (DateTime.Now - p.TestDate).Days == 0)
                    .ToSql();

                Assert.Equal(@"SELECT ""p"".""AuthorId"", ""p"".""Active"", ""p"".""TestBytes"", ""p"".""TestDate"", ""p"".""TestDecimal"", ""p"".""TestDouble"", ""p"".""TestGuid"", ""p"".""TestInt"", ""p"".""TestIntNullable"", ""p"".""TestString"", ""p"".""TestdoubleNullable"", ""p"".""TimeSpan""
FROM ""Author"" ""p""
WHERE DATEDIFF(day, CAST('NOW' AS TIMESTAMP), ""p"".""TestDate"") = 0", sql);
            }
        }

        [Fact]
        public void DateDiff_hours()
        {
            using (var ctx = CreateContext())
            {
                var sql = ctx
                    .Author
                    .Where(p => (DateTime.Now - p.TestDate).Hours == 0)
                    .ToSql();

                Assert.Equal(@"SELECT ""p"".""AuthorId"", ""p"".""Active"", ""p"".""TestBytes"", ""p"".""TestDate"", ""p"".""TestDecimal"", ""p"".""TestDouble"", ""p"".""TestGuid"", ""p"".""TestInt"", ""p"".""TestIntNullable"", ""p"".""TestString"", ""p"".""TestdoubleNullable"", ""p"".""TimeSpan""
FROM ""Author"" ""p""
WHERE DATEDIFF(hour, CAST('NOW' AS TIMESTAMP), ""p"".""TestDate"") = 0", sql);
            }
        }

        [Fact]
        public void DateDiff_minutes()
        {
            using (var ctx = CreateContext())
            {
                var sql = ctx
                    .Author
                    .Where(p => (DateTime.Now - p.TestDate).Minutes == 0)
                    .ToSql();

                Assert.Equal(@"SELECT ""p"".""AuthorId"", ""p"".""Active"", ""p"".""TestBytes"", ""p"".""TestDate"", ""p"".""TestDecimal"", ""p"".""TestDouble"", ""p"".""TestGuid"", ""p"".""TestInt"", ""p"".""TestIntNullable"", ""p"".""TestString"", ""p"".""TestdoubleNullable"", ""p"".""TimeSpan""
FROM ""Author"" ""p""
WHERE DATEDIFF(minute, CAST('NOW' AS TIMESTAMP), ""p"".""TestDate"") = 0", sql);
            }
        }

        [Fact]
        public void DateDiff_seconds()
        {
            using (var ctx = CreateContext())
            {
                var sql = ctx
                    .Author
                    .Where(p => (DateTime.Now - p.TestDate).Seconds == 0)
                    .ToSql();

                Assert.Equal(@"SELECT ""p"".""AuthorId"", ""p"".""Active"", ""p"".""TestBytes"", ""p"".""TestDate"", ""p"".""TestDecimal"", ""p"".""TestDouble"", ""p"".""TestGuid"", ""p"".""TestInt"", ""p"".""TestIntNullable"", ""p"".""TestString"", ""p"".""TestdoubleNullable"", ""p"".""TimeSpan""
FROM ""Author"" ""p""
WHERE DATEDIFF(second, CAST('NOW' AS TIMESTAMP), ""p"".""TestDate"") = 0", sql);
            }
        }

        [Fact]
        public void DateDiff_milliseconds()
        {
            using (var ctx = CreateContext())
            {
                var sql = ctx
                    .Author
                    .Where(p => (DateTime.Now - p.TestDate).Milliseconds == 0)
                    .ToSql();

                Assert.Equal(@"SELECT ""p"".""AuthorId"", ""p"".""Active"", ""p"".""TestBytes"", ""p"".""TestDate"", ""p"".""TestDecimal"", ""p"".""TestDouble"", ""p"".""TestGuid"", ""p"".""TestInt"", ""p"".""TestIntNullable"", ""p"".""TestString"", ""p"".""TestdoubleNullable"", ""p"".""TimeSpan""
FROM ""Author"" ""p""
WHERE DATEDIFF(millisecond, CAST('NOW' AS TIMESTAMP), ""p"".""TestDate"") = 0", sql);
            }
        }
    }
}
