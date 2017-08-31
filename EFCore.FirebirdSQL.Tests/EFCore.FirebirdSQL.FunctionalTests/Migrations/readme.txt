run scripts/rebuild.ps1 or scripts/rebuild.sh to instantiate the migrations and database

Scaffold-DbContext "User=SYSDBA;Password=masterkey;Database=D:/EfCoreTest.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=UTF8;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;" SouchProd.EntityFrameworkCore.Firebird -OutputDir Models2