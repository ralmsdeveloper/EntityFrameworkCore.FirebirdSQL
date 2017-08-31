using System;
using System.Collections.Generic;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using SouchProd.EntityFrameworkCore.Firebird.FunctionalTests;
using Xunit;

namespace EFCore.Firebird.FunctionalTests.Tests
{
    public class MigrationTests
    {
        private static readonly FbConnection Connection = new FbConnection(AppConfig.Config["Data:ConnectionString"]);

        private static AppDb NewDbContext(bool reuseConnection)
        {
            return reuseConnection ? new AppDb(Connection) : new AppDb();
        }

        [Fact]
        public void TableDDLGeneratoion()
        {
            using (var db = NewDbContext(false))
            {
                db.Database.OpenConnection();
                //db.Database.GetDbConnection().GetSchema()
                Assert.Equal("SouchProd.EntityFrameworkCore.Firebird", db.Database.ProviderName);
            }
        }
    }
}
