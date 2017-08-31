using System;
using System.Diagnostics;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Xunit;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;

namespace SouchProd.EntityFrameworkCore.Firebird.Tests.Migrations
{
    public class MigrationSqlGeneratorTest : MigrationSqlGeneratorTestBase
    {
        protected override IMigrationsSqlGenerator SqlGenerator
        {
            get
            {
                // type mapper
                var typeMapper = new FbTypeMapper(new RelationalTypeMapperDependencies());

                // migrationsSqlGeneratorDependencies
                var commandBuilderFactory = new RelationalCommandBuilderFactory(
                    new FakeDiagnosticsLogger<DbLoggerCategory.Database.Command>(),
                    typeMapper);

                var FirebirdOptions = new Mock<IFbOptions>();
                FirebirdOptions.SetupGet(opts => opts.ConnectionSettings).Returns(
                    new FbConnectionSettings(new FbConnectionStringBuilder(), new ServerVersion("2.1")));

                FirebirdOptions
                    .Setup(fn =>
                        fn.GetCreateTable(It.IsAny<ISqlGenerationHelper>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns("s"
                    );

                var migrationsSqlGeneratorDependencies = new MigrationsSqlGeneratorDependencies(
                    commandBuilderFactory,
                    new FbSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()
                    , FirebirdOptions.Object),
                    typeMapper);

                return new FbMigrationsSqlGenerator(
                    migrationsSqlGeneratorDependencies,
                    FirebirdOptions.Object);

                //var FbOptions = new FirebirdOptions();
                //FirebirdOptions.SetupGet(opts => opts.ConnectionSettings).Returns(
                ///    new FbConnectionSettings(new FbConnectionStringBuilder(), new ServerVersion("2.1")));

                /*FirebirdOptions
                    .Setup(fn =>
                        fn.GetCreateTable(It.IsAny<ISqlGenerationHelper>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns("s"
                    );*/

                //return new FbMigrationsSqlGenerator(
                //    migrationsSqlGeneratorDependencies,
                //    FbOptions);
            }
        }

        private static FakeRelationalConnection CreateConnection(IDbContextOptions options = null)
            => new FakeRelationalConnection(options ?? CreateOptions());

        private static IDbContextOptions CreateOptions(RelationalOptionsExtension optionsExtension = null)
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(optionsExtension
                                      ?? new FakeRelationalOptionsExtension().WithConnectionString("test"));

            return optionsBuilder.Options;
        }

        [Fact]
        public override void AddColumnOperation_with_defaultValue()
        {
            base.AddColumnOperation_with_defaultValue();

            Assert.Equal(
                "ALTER TABLE \"People\" ADD \"Name\" varchar(30) NOT NULL DEFAULT 'John Doe';" + EOL,
                Sql);
        }

        [Fact]
        public override void AddColumnOperation_with_defaultValueSql()
        {
            base.AddColumnOperation_with_defaultValueSql();

            Assert.Equal(
                "ALTER TABLE \"People\" ADD \"Birthday\" timestamp DEFAULT CURRENT_TIMESTAMP;" + EOL,
                Sql);
        }

        [Fact]
        public override void AddColumnOperation_with_computed_column_SQL()
	    {
		    base.AddColumnOperation_with_computed_column_SQL();

		    Assert.Equal(
			    "ALTER TABLE \"PEOPLE\" ADD \"BIRTHDAY\" TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;" + EOL,
			    Sql.ToUpper());
	    }

        [Fact]
        public override void AddDefaultDatetimeOperation_with_valueOnUpdate()
        {
            base.AddDefaultDatetimeOperation_with_valueOnUpdate();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ADD \"BIRTHDAY\" timestamp DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP;" + EOL,
                Sql.ToUpper());
        }

        [Fact]
        public override void AddDefaultBooleanOperation()
        {
            base.AddDefaultBooleanOperation();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ADD \"ISLEADER\" bit DEFAULT TRUE;" + EOL,
                Sql.ToUpper());
        }

        public override void AddColumnOperation_without_column_type()
        {
            base.AddColumnOperation_without_column_type();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ADD \"ALIAS\" text NOT NULL;" + EOL,
                Sql.ToUpper());
        }

        public override void AddColumnOperation_with_maxLength()
        {
            base.AddColumnOperation_with_maxLength();

            Assert.Equal(
                "ALTER TABLE \"PERSON\" ADD \"NAME\" varchar(30);" + EOL,
                Sql.ToUpper());
        }

        public override void AddForeignKeyOperation_with_name()
        {
            base.AddForeignKeyOperation_with_name();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ADD CONSTRAINT \"FK_PEOPLE_COMPANIES\" FOREIGN KEY (\"EMPLOYERID1\", \"EMPLOYERID2\") REFERENCES \"HR\".\"COMPANIES\" (\"ID1\", \"ID2\") ON DELETE CASCADE;" + EOL,
                Sql.ToUpper());
        }

        public override void AddForeignKeyOperation_without_name()
        {
            base.AddForeignKeyOperation_without_name();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ADD FOREIGN KEY (\"SPOUSEID\") REFERENCES \"PEOPLE\" (\"ID\");" + EOL,
                Sql.ToUpper());
        }

        public override void AddPrimaryKeyOperation_with_name()
        {
            base.AddPrimaryKeyOperation_with_name();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ADD CONSTRAINT \"PK_PEOPLE\" PRIMARY KEY (\"ID1\", \"ID2\");" + EOL,
                Sql.ToUpper());
        }

        public override void AddPrimaryKeyOperation_without_name()
        {
            base.AddPrimaryKeyOperation_without_name();

            var test =
                "ALTER TABLE \"PEOPLE\" ADD PRIMARY KEY (\"ID\");" + EOL +
"DROP PROCEDURE IF EXISTS POMELO_AFTER_ADD_PRIMARY_KEY;" +
" CREATE PROCEDURE POMELO_AFTER_ADD_PRIMARY_KEY(IN \"SCHEMA_NAME_ARGUMENT\" VARCHAR(255), IN \"TABLE_NAME_ARGUMENT\" VARCHAR(255), IN \"COLUMN_NAME_ARGUMENT\" VARCHAR(255))" +
" BEGIN" +
" DECLARE HAS_AUTO_INCREMENT_ID integer;" +
" DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);" +
" DECLARE PRIMARY_KEY_TYPE VARCHAR(255);" +
" DECLARE SQL_EXP VARCHAR(1000);" +
" SELECT COUNT(*)" +
" INTO HAS_AUTO_INCREMENT_ID" +
" FROM \"information_schema\".\"COLUMNS\"" +
" WHERE \"TABLE_SCHEMA\" = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))" +
" AND \"TABLE_NAME\" = TABLE_NAME_ARGUMENT" +
" AND \"COLUMN_NAME\" = COLUMN_NAME_ARGUMENT" +
" AND \"COLUMN_TYPE\" LIKE '%int%'" +
" AND \"COLUMN_KEY\" = 'PRI';" +
" IF HAS_AUTO_INCREMENT_ID THEN" +
" SELECT \"COLUMN_TYPE\"" +
" INTO PRIMARY_KEY_TYPE" +
" FROM \"information_schema\".\"COLUMNS\"" +
" WHERE \"TABLE_SCHEMA\" = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))" +
" AND \"TABLE_NAME\" = TABLE_NAME_ARGUMENT" +
" AND \"COLUMN_NAME\" = COLUMN_NAME_ARGUMENT" +
" AND \"COLUMN_TYPE\" LIKE '%int%'" +
" AND \"COLUMN_KEY\" = 'PRI';" +
" SELECT \"COLUMN_NAME\"" +
" INTO PRIMARY_KEY_COLUMN_NAME" +
" FROM \"information_schema\".\"COLUMNS\"" +
" WHERE \"TABLE_SCHEMA\" = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))" +
" AND \"TABLE_NAME\" = TABLE_NAME_ARGUMENT" +
" AND \"COLUMN_NAME\" = COLUMN_NAME_ARGUMENT" +
" AND \"COLUMN_TYPE\" LIKE '%int%'" +
" AND \"COLUMN_KEY\" = 'PRI';" +
" SET SQL_EXP = CONCAT('ALTER TABLE \"', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '\".\"', TABLE_NAME_ARGUMENT, '\" ALTER COLUMN \"', PRIMARY_KEY_COLUMN_NAME, '\" ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');" +
" SET @SQL_EXP = SQL_EXP;" +
" PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;" +
" EXECUTE SQL_EXP_EXECUTE;" +
" DEALLOCATE PREPARE SQL_EXP_EXECUTE;" +
" END IF;" +
" END;" + EOL +
                "CALL POMELO_AFTER_ADD_PRIMARY_KEY(NULL, 'PEOPLE', 'ID');" + EOL +
                "DROP PROCEDURE IF EXISTS POMELO_AFTER_ADD_PRIMARY_KEY;" + EOL;
            
            Assert.Equal(test,
                Sql.ToUpper());
        }

        public override void AddUniqueConstraintOperation_with_name()
        {
            base.AddUniqueConstraintOperation_with_name();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ADD CONSTRAINT \"AK_PEOPLE_DRIVERLICENSE\" UNIQUE (\"DRIVERLICENSE_STATE\", \"DRIVERLICENSE_NUMBER\");" + EOL,
                Sql.ToUpper());
        }

        public override void AddUniqueConstraintOperation_without_name()
        {
            base.AddUniqueConstraintOperation_without_name();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ADD UNIQUE (\"SSN\");" + EOL,
                Sql.ToUpper());
        }

        public override void CreateIndexOperation_unique()
        {
            base.CreateIndexOperation_unique();

            Assert.Equal(
                "CREATE UNIQUE INDEX \"IX_PEOPLE_NAME\" ON \"PEOPLE\" (\"FIRSTNAME\", \"LASTNAME\");" + EOL,
                Sql.ToUpper());
        }
        
        public override void CreateIndexOperation_nonunique()
        {
            base.CreateIndexOperation_nonunique();

            Assert.Equal(
                "CREATE INDEX \"IX_PEOPLE_NAME\" ON \"PEOPLE\" (\"NAME\");" + EOL,
                Sql);
        }

        public override void RenameIndexOperation_works()
        {
            base.RenameIndexOperation_works();
            
            Assert.Equal("ALTER TABLE \"PEOPLE\" RENAME INDEX \"IX_People_Discriminator\" TO \"IX_People_DiscriminatorNew\";" + EOL,
                Sql);
        }

        public virtual void CreateDatabaseOperation()
        {
            Generate(new FbCreateDatabaseOperation { Name = "Northwind" });

            Assert.Equal(
                "CREATE SCHEMA  \"NORTHWIND\";" + EOL, Sql);
        }

        public override void CreateTableOperation()
        {
            base.CreateTableOperation();

            Assert.Equal(
                "CREATE TABLE \"PEOPLE\" (" + EOL +
                "    \"ID\" integer NOT NULL," + EOL +
                "    \"EMPLOYERID\" integer," + EOL +
                "    \"SSN\" varchar(11)," + EOL +
                "    PRIMARY KEY (\"ID\")," + EOL +
                "    UNIQUE (\"SSN\")," + EOL +
                "    FOREIGN KEY (\"EMPLOYERID\") REFERENCES \"COMPANIES\" (\"ID\")" + EOL +
                ");" + EOL,
                Sql.ToUpper());
        }

        public override void CreateTableUlongAi()
        {
            base.CreateTableUlongAi();

            Assert.Equal(
                "CREATE TABLE \"TestUlongAutoIncrement\" ("+
                "    \"Id\" bigint NOT NULL," +
                "    PRIMARY KEY(\"Id\")"+
                ");"+
                "CREATE GENERATOR \"TestUlongAutoIncrement_Id\"; CREATE OR ALTER TRIGGER \"TestUlongAutoIncrement_Id\" FOR \"TestUlongAutoIncrement\"" +
                "ACTIVE BEFORE INSERT POSITION 0 AS BEGIN" +
                "    IF(new.\"Id\" IS NULL) THEN       new.\"Id\" = GEN_ID(\"TestUlongAutoIncrement_Id\", 1);" +
                "END;" + EOL,
                Sql.ToUpper());
        }

        public override void DropColumnOperation()
        {
            base.DropColumnOperation();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" DROP \"LUCKYNUMBER\";",
                Sql.ToUpper());
        }

        public override void DropForeignKeyOperation()
        {
            base.DropForeignKeyOperation();

            Assert.Equal(
                "ALTER TABLE \"People\" DROP CONSTRAINT \"FK_People_Companies\";" + EOL,
                Sql);
        }

        public override void DropPrimaryKeyOperation()
        {
            base.DropPrimaryKeyOperation();

            Assert.Equal("ALTER TABLE \"People\" DROP CONSTRAINT PK_People;" + EOL,
                Sql);
        }

        public override void DropTableOperation()
        {
            base.DropTableOperation();

            Assert.Equal(
                "DROP TABLE \"People\";" + EOL,
                Sql);
        }

        public override void DropUniqueConstraintOperation()
        {
            base.DropUniqueConstraintOperation();

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" DROP CONSTRAINT \"AK_PEOPLE_SSN\";" + EOL,
                Sql.ToUpper());
        }

        public override void SqlOperation()
        {
            base.SqlOperation();

            Assert.Equal(
                "-- I <3 DDL" + EOL,
                Sql);
        }

        #region AlterColumn

        public override void AlterColumnOperation()
        {
            base.AlterColumnOperation();
            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ALTER COLUMN \"LUCKYNUMBER\" integer NOT NULL;" + EOL +
                "ALTER TABLE \"PEOPLE\" ALTER COLUMN \"LUCKYNUMBER\" SET DEFAULT 7" + EOL,
            Sql.ToUpper(), false, true, true);
        }

        public override void AlterColumnOperation_without_column_type()
        {
            base.AlterColumnOperation_without_column_type();
            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ALTER COLUMN \"LUCKYNUMBER\" integer NOT NULL;" + EOL +
                "ALTER TABLE \"PEOPLE\" ALTER COLUMN \"LUCKYNUMBER\" DROP DEFAULT;",
            Sql);
        }

        [Fact]
        public void AlterColumnOperation_dbgenerated_uuid()
        {
            Generate(
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "GuidKey",
                    ClrType = typeof(int),
                    ColumnType = "char(38)",
                    IsNullable = false,
                    [FbAnnotationNames.ValueGenerationStrategy] = FbValueGenerationStrategy.IdentityColumn
                });

            Assert.Equal(
                "ALTER TABLE \"PEOPLE\" ALTER COLUMN \"GUIDKEY\" char(38) NOT NULL;" + EOL +
                "ALTER TABLE \"PEOPLE\" ALTER COLUMN \"GUIDKEY\" DROP DEFAULT;",
            Sql.ToUpper(), false , true, true);
        }

        [Theory]
        [InlineData("BLOB TYPE 1")]
        [InlineData("BLOB TYPE 0")]
        public void AlterColumnOperation_with_no_default_value_column_types(string type)
        {
            Generate(
                new AlterColumnOperation
                {
                    Table = "People",
                    Name = "Blob",
                    ClrType = typeof(string),
                    ColumnType = type,
                    IsNullable = true,
                });

            Assert.Equal(
                $"ALTER TABLE \"PEOPLE\" ALTER COLUMN \"BLOB\" {type} NULL;" + EOL,
                Sql.ToUpper(), false, true, true);
        }

        #endregion

    }
}
