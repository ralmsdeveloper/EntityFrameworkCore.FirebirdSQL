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

using System;
using Microsoft.EntityFrameworkCore;
using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class FirebirdTestStore : RelationalTestStore
    {
        private FbConnection _connection;
        private readonly string _name;
        private bool _deleteDatabase;
        private static int _scratchCount;
        private static string BaseDirectory => AppContext.BaseDirectory;

        public static FirebirdTestStore GetNorthwindStore() => GetOrCreateShared("northwind", () => { });

        public static FirebirdTestStore GetOrCreateShared(string name, Action initializeDatabase = null)
            => new FirebirdTestStore(name).CreateShared(initializeDatabase, true);

        public const int CommandTimeout = 30;

        private FirebirdTestStore(string name) => _name = name;

        public override string ConnectionString => Connection.ConnectionString;

        private FirebirdTestStore CreateShared(Action initializeDatabase, bool openConnection)
        {
            CreateShared(typeof(FirebirdTestStore).Name + _name, initializeDatabase);

            CreateConnection();

            if (openConnection)
            {
                OpenConnection();
            }

            return this;
        }

        private FirebirdTestStore CreateTransient()
        {
            CreateConnection();
            OpenConnection();

            _deleteDatabase = true;
            return this;
        }

        private void CreateConnection()
        {
            _connection = new FbConnection(CreateConnectionString(_name));

            OpenConnection();
        }

        public override void OpenConnection()
        {
            if(_connection?.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
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
            command.CommandTimeout = CommandTimeout;

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

        public static string CreateConnectionString(string name)
        =>  new FbConnectionStringBuilder("User=SYSDBA;Password=masterkey;")
            {
                Database = $"{AppDomain.CurrentDomain.BaseDirectory}{name}.fdb",
                Port = 3050
            }.ConnectionString;
    }
}
