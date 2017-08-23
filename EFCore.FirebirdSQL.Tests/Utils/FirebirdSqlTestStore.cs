using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic; 
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.DependencyInjection;
using EFCore.FirebirdSQL.Tests;

namespace Microsoft.EntityFrameworkCore.Utilities
{

    public class FirebirdSqlTestStore : RelationalTestStore
    {
        private FbConnection _connection;
        public override string ConnectionString => Connection.ConnectionString;
        private FirebirdSqlTestStore CreateShared()
        {
           
            CreateConnection();
            OpenConnection();
            return this;
        }

        public void CreateConnection()
        {

            var sb = new FbConnectionStringBuilder(CreateConnectionString());
            if(File.Exists(sb.Database))
                FbConnection.DropDatabase(_connection.ConnectionString);

            FbConnection.CreateDatabase(sb.ToString());
            _connection = new FbConnection(sb.ToString());
            OpenConnection();
        }

        public override void OpenConnection()
        { 
            _connection.Open();  
        }

        public int ExecuteNonQuery(string sql, params object[] parameters)
        {
            using (var command = CreateCommand(sql, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        private DbCommand CreateCommand(string commandText, object[] parameters)
        {
            var command = _connection.CreateCommand(); 
            command.CommandText = commandText;
            command.CommandTimeout = 30; 
            for (var i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue("@p" + i, parameters[i]);
            } 

            return command;
        }

        public override DbConnection Connection => _connection;
        public override DbTransaction Transaction => null;

        public override void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
            base.Dispose();

            
        }

        public static string CreateConnectionString()
        {
            string connectionString =
            "User=SYSDBA;" +
            "Password=masterkey;" +
            $"Database={AppContext.BaseDirectory}Test.fdb;" +
            "DataSource=localhost;" +
            "Port=2017;" +
            "Dialect=3;" +
            "Charset=NONE;" +
            "Role=;" +
            "Connection lifetime=15;" +
            "Pooling=true;" +
            "MinPoolSize=1;" +
            "MaxPoolSize=50;" +
            "Packet Size=8192;" +
            "ServerType=0";
           return new FbConnectionStringBuilder(connectionString).ToString();

        }
            
    }

}
 
