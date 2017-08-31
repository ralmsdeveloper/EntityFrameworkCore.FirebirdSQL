using System;

namespace SouchProd.EntityFrameworkCore.Firebird.FunctionalTests.Commands{

    public static class ConnectionStringCommand{

        public static void Run(){
            Console.Write(AppConfig.Config["Data:ConnectionString"]);
        }

    }

}
