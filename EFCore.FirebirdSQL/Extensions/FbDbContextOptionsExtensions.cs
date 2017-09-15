/*                 
 *                    EntityFrameworkCore.FirebirdSQL
 *     
*
 *              
 *     Permission to use, copy, modify, and distribute this software and its
 *     documentation for any purpose, without fee, and without a written
 *     agreement is hereby granted, provided that the above copyright notice
 *     and this paragraph and the following two paragraphs appear in all copies. 
 * 
 *     The contents of this file are subject to the Initial
 *     Developer's Public License Version 1.0 (the "License");
 *     you may not use this file except in compliance with the
 *     License.
*
 *
 *     Software distributed under the License is distributed on
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations under the License.
 *
 *      Credits: Rafael Almeida (ralms@ralms.net)
 *                              Sergipe-Brazil
 *
 *
 *                              
 *                  All Rights Reserved.
 */

using System;
using System.Data.Common;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using FirebirdSql.Data.FirebirdClient;

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
