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
using System.Data.Common;
using EntityFrameworkCore.FirebirdSql.Infrastructure;
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public static class FbDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseFirebird(
            this DbContextOptionsBuilder optionsBuilder,
            string connectionString,
            Action<FbDbContextOptionsBuilder> FbOptionsAction = null)
        {
            var extension = (FbOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            FbOptionsAction?.Invoke(new FbDbContextOptionsBuilder(optionsBuilder));
            ConfigureWarnings(optionsBuilder);
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseFirebird(
            this DbContextOptionsBuilder optionsBuilder,
            DbConnection connection,
            Action<FbDbContextOptionsBuilder> fbOptionsAction = null)
        {
            var extension = (FbOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
            fbOptionsAction?.Invoke(new FbDbContextOptionsBuilder(optionsBuilder));
            ConfigureWarnings(optionsBuilder);
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> UseFirebird<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            string connectionString,
            Action<FbDbContextOptionsBuilder> fbOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseFirebird((DbContextOptionsBuilder)optionsBuilder, connectionString, fbOptionsAction);

        public static DbContextOptionsBuilder<TContext> UseFirebird<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            DbConnection connection,
            Action<FbDbContextOptionsBuilder> fbOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UseFirebird((DbContextOptionsBuilder)optionsBuilder, connection, fbOptionsAction);

        private static FbOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<FbOptionsExtension>()
                ?? new FbOptionsExtension();

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {
            var coreOptionsExtension
                = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                ?? new CoreOptionsExtension();

            coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
            coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
            RelationalEventId.AmbientTransactionWarning, WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }
}
