/*                 
 *     EntityFrameworkCore.FirebirdSqlSQL  - Congratulations EFCore Team
 *              https://www.FirebirdSqlsql.org/en/net-provider/ 
 *     Permission to use, copy, modify, and distribute this software and its
 *     documentation for any purpose, without fee, and without a written
 *     agreement is hereby granted, provided that the above copyright notice
 *     and this paragraph and the following two paragraphs appear in all copies. 
 * 
 *     The contents of this file are subject to the Initial
 *     Developer's Public License Version 1.0 (the "License");
 *     you may not use this file except in compliance with the
 *     License. You may obtain a copy of the License at
 *     http://www.FirebirdSqlsql.org/index.php?op=doc&id=idpl
 *
 *     Software distributed under the License is distributed on
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations under the License.
 *
 *              Copyright (c) 2017 Rafael Almeida
 *         Made In Sergipe-Brasil - ralms@ralms.net 
 *                  All Rights Reserved.
 */

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class FirebirdSqlServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkFirebirdSql([NotNull] this IServiceCollection serviceCollection)
        {
            Check.NotNull(serviceCollection, nameof(serviceCollection)); 

            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<IRelationalCommandBuilderFactory, FirebirdSqlCommandBuilderFactory>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<FbOptionsExtension>>() 
                .TryAdd<ISqlGenerationHelper, FirebirdSqlSqlGenerationHelper>()
                .TryAdd<IRelationalTypeMapper, FirebirdSqlTypeMapper>() 
                .TryAdd<IMigrationsAnnotationProvider, FirebirdSqlMigrationsAnnotationProvider>()
                .TryAdd<IConventionSetBuilder, FirebirdSqlConventionSetBuilder>()
                .TryAdd<IUpdateSqlGenerator>(p => p.GetService<IFirebirdSqlUpdateSqlGenerator>())
                .TryAdd<IModificationCommandBatchFactory, FirebirdSqlModificationCommandBatchFactory>()
                .TryAdd<IValueGeneratorSelector, FirebirdSqlValueGeneratorSelector>()
                .TryAdd<IRelationalConnection>(p => p.GetService<IFirebirdSqlRelationalConnection>())
     
                .TryAdd<IMigrationsSqlGenerator, FirebirdSqlMigrationsSqlGenerator>()
               .TryAdd<IBatchExecutor, FirebirdSqlBatchExecutor>()
                .TryAdd<IBatchExecutor, BatchExecutor>()
                .TryAdd<IRelationalDatabaseCreator, FirebirdSqlDatabaseCreator>()
                .TryAdd<IHistoryRepository, FirebirdSqlHistoryRepository>()
                .TryAdd<IMemberTranslator, FirebirdSqlCompositeMemberTranslator>()
                .TryAdd<ICompositeMethodCallTranslator, FirebirdSqlCompositeMethodCallTranslator>()
                .TryAdd<IQuerySqlGeneratorFactory, FirebirdSqlQuerySqlGeneratorFactory>()
                .TryAdd<ISingletonOptions, IFirebirdSqlOptions>(p => p.GetService<IFirebirdSqlOptions>())
                .TryAddProviderSpecificServices(b => b
                    .TryAddSingleton<IFirebirdSqlOptions, FirebirdSqlOptions>()
                    .TryAddScoped<IFirebirdSqlUpdateSqlGenerator, FirebirdSqlUpdateSqlGenerator>()
                    .TryAddScoped<IFirebirdSqlRelationalConnection, FirebirdSqlRelationalConnection>());

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
