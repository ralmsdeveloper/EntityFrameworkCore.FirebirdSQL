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
using Microsoft.EntityFrameworkCore;
using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.TestUtilities;
using EFCore.FirebirdSql.FunctionalTests.TestUtilities;
using System.IO;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class FirebirdTestStore : RelationalTestStore
    {
        public const int CommandTimeout = 30;

        public static FirebirdTestStore GetOrCreate(string name, bool sharedCache = true)
            => new FirebirdTestStore(name, sharedCache: sharedCache);

        public static FirebirdTestStore GetOrCreateInitialized(string name)
            => new FirebirdTestStore(name).InitializeFirebird(null, (Func<DbContext>)null, null);

        public static FirebirdTestStore GetExisting(string name)
            => new FirebirdTestStore(name, seed: false);

        public static FirebirdTestStore Create(string name, bool sharedCache = true)
            => new FirebirdTestStore(name, sharedCache: sharedCache, shared: false);

        public static FirebirdTestStore CreateInitialized(string name)
            => new FirebirdTestStore(name, shared: false).InitializeFirebird(null, (Func<DbContext>)null, null);

        private readonly bool _seed;

        private FirebirdTestStore(string name, bool seed = true, bool sharedCache = true, bool shared = true)
            : base(name, shared)
        {
            _seed = seed;
            ConnectionString = new FbConnectionStringBuilder(@"User=SYSDBA;Password=masterkey;DataSource=localhost;Port=3050;")
            {
                Database = Path.Combine(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..")).FullName , $"{Name}.fdb")
            }.ConnectionString;

            Connection = new FbConnection(ConnectionString);
        }

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => builder.UseFirebird(Connection, b => b.CommandTimeout(CommandTimeout));

        public FirebirdTestStore InitializeFirebird(IServiceProvider serviceProvider, Func<DbContext> createContext, Action<DbContext> seed)
            => (FirebirdTestStore)Initialize(serviceProvider, createContext, seed);

        public FirebirdTestStore InitializeFirebird(IServiceProvider serviceProvider, Func<FirebirdTestStore, DbContext> createContext, Action<DbContext> seed)
            => (FirebirdTestStore)Initialize(serviceProvider, () => createContext(this), seed);

        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed)
        {
            if (!_seed)
            {
                return;
            }
            using (var context = createContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                seed(context);
            }
        }

        public override void Clean(DbContext context)
            => context.Database.EnsureClean();

        public override void OpenConnection() => Connection.Open();

        public int ExecuteNonQuery(string sql, params object[] parameters)
        {
            using (var command = CreateCommand(sql, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        private DbCommand CreateCommand(string commandText, object[] parameters)
        {
            var command = (FbCommand)Connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandTimeout = CommandTimeout;

            for (var i = 0; i < parameters.Length; i++)
            {
                command.Parameters.AddWithValue("@p" + i, parameters[i]);
            }

            return command;
        }
    }
}
