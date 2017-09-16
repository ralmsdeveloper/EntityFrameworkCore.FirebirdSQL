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
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class FbDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseFirebird(
            this DbContextOptionsBuilder optionsBuilder,
            string connectionString,
            Action<FbDbContextOptionsBuilder> FbOptionsAction = null)
        { 
            var extension = GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);            
            FbOptionsAction?.Invoke(new FbDbContextOptionsBuilder(optionsBuilder)); 
            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseFirebird(
            this DbContextOptionsBuilder optionsBuilder,
            DbConnection connection,
            Action<FbDbContextOptionsBuilder> fbOptionsAction = null)
        {
             
            var extension = GetOrCreateExtension(optionsBuilder).WithConnection(connection);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension); 
            fbOptionsAction?.Invoke(new FbDbContextOptionsBuilder(optionsBuilder)); 
            return optionsBuilder;
        }

		public static DbContextOptionsBuilder<TContext> UseFirebird<TContext>(
			this DbContextOptionsBuilder<TContext> optionsBuilder,
			string connectionString,
			Action<FbDbContextOptionsBuilder> fbOptionsAction = null)
			where TContext : DbContext
		{
			return (DbContextOptionsBuilder<TContext>)UseFirebird(
						   (DbContextOptionsBuilder)optionsBuilder, connectionString, fbOptionsAction);
		}

		public static DbContextOptionsBuilder<TContext> UseFirebird<TContext>(
			this DbContextOptionsBuilder<TContext> optionsBuilder,
			DbConnection connection,
			Action<FbDbContextOptionsBuilder> fbOptionsAction = null)
			where TContext : DbContext
		{
			return (DbContextOptionsBuilder<TContext>)UseFirebird(
						   (DbContextOptionsBuilder)optionsBuilder, connection, fbOptionsAction);
		}

		private static FbOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        {
            var existsExtension = optionsBuilder.Options.FindExtension<FbOptionsExtension>();
            return existsExtension != null
                ? new FbOptionsExtension(existsExtension)
                : new FbOptionsExtension();
        }
    
    }
}
