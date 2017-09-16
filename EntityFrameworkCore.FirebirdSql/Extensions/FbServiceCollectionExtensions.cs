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

using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using EntityFrameworkCore.FirebirdSql.Internal;
using EntityFrameworkCore.FirebirdSql.Metadata.Conventions;
using EntityFrameworkCore.FirebirdSql.Migrations;
using EntityFrameworkCore.FirebirdSql.Migrations.Internal;
using EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal;
using EntityFrameworkCore.FirebirdSql.Query.Sql.Internal;
using EntityFrameworkCore.FirebirdSql.Storage.Internal;
using EntityFrameworkCore.FirebirdSql.Storage.Internal.Mapping;
using EntityFrameworkCore.FirebirdSql.Update.Internal;
using EntityFrameworkCore.FirebirdSql.ValueGeneration.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal; 
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal; 
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal; 
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class FbServiceCollectionExtensions
	{
		public static IServiceCollection AddEntityFrameworkFirebird(this IServiceCollection serviceCollection)
		{
			var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
				.TryAdd<IRelationalDatabaseCreator, FbDatabaseCreator>()
				.TryAdd<IDatabaseProvider, DatabaseProvider<FbOptionsExtension>>()
				.TryAdd<IRelationalTypeMapper, FbTypeMapper>()
				.TryAdd<IRelationalCommandBuilderFactory, FbCommandBuilderFactory>()
				.TryAdd<ISqlGenerationHelper, FbSqlGenerationHelper>()
				.TryAdd<IMigrationsAnnotationProvider, FbMigrationsAnnotationProvider>()
				.TryAdd<IConventionSetBuilder, FbConventionSetBuilder>()
				.TryAdd<IUpdateSqlGenerator>(p => p.GetService<IFbUpdateSqlGenerator>())
				.TryAdd<IModificationCommandBatchFactory, FbModificationCommandBatchFactory>()
				.TryAdd<IValueGeneratorSelector, FbValueGeneratorSelector>()
				.TryAdd<IRelationalConnection>(p => p.GetService<IFbRelationalConnection>())
				.TryAdd<IMigrationsSqlGenerator, FbMigrationsSqlGenerator>()
				.TryAdd<IBatchExecutor, FbBatchExecutor>()
				.TryAdd<IBatchExecutor, BatchExecutor>()
				.TryAdd<IHistoryRepository, FbHistoryRepository>()
				.TryAdd<IMemberTranslator, FbCompositeMemberTranslator>()
				.TryAdd<ICompositeMethodCallTranslator, FbCompositeMethodCallTranslator>()
				.TryAdd<IQuerySqlGeneratorFactory, FbQuerySqlGeneratorFactory>()
				.TryAdd<ISingletonOptions, IFbOptions>(p => p.GetService<IFbOptions>())
				.TryAddProviderSpecificServices(b => b
					                                .TryAddSingleton<IFbOptions, FbOptions>()
					                                .TryAddScoped<IFbUpdateSqlGenerator, FbUpdateSqlGenerator>()
					                                .TryAddScoped<IFbRelationalConnection, FbRelationalConnection>());

			builder.TryAddCoreServices(); 
			return serviceCollection;
		}
	}
}
