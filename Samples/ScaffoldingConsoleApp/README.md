# Scaffolding Sample

This very basic console app demonstrate & test the Scaffolding capability of the Firebird provider.

### Configuration

1. Create the sample database using the provided script, adjust the filename & credentials before.
2. Using the Package Manager Console in Visual Studio (ensur that the correct project is selected), execute the following command. You may adjust the database filenema and location, as well as the default password.

	``Scaffold-DbContext "User=SYSDBA;Password=masterkey;Database=D:/ScaffoldTestDb.fdb;DataSource=127.0.0.1;Port=3050;Dialect=3;Charset=UTF8;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;" "souchprod.EntityFrameworkCore.Firebird" -OutputDir Entities -Context "SampleDbContext" -DataAnnotations -force -verbose``

3. A folder "Entities" will be automatically created, it will contains the generated models and the DBContext, ready to be conssumed.