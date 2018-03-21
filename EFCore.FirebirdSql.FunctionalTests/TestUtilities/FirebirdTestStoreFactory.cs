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

using System.Diagnostics;
using EntityFrameworkCore.FirebirdSql.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Firebird.Design.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFCore.FirebirdSql.FunctionalTests.TestUtilities
{
    public class FirebirdTestStoreFactory : ITestStoreFactory
    {
        public static FirebirdTestStoreFactory Instance { get; } = new FirebirdTestStoreFactory();

        protected FirebirdTestStoreFactory()
        {
        }

        public virtual TestStore Create(string storeName)
            => FirebirdTestStore.Create(storeName);

        public virtual TestStore GetOrCreate(string storeName)
            => FirebirdTestStore.GetOrCreate(storeName);

        public virtual IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<ValueConverterSelectorDependencies>()
                .AddSingleton<DiagnosticSource>(new DiagnosticListener(DbLoggerCategory.Name))
                .AddSingleton<ILoggingOptions, LoggingOptions>()
                .AddSingleton(typeof(IDiagnosticsLogger<>), typeof(DiagnosticsLogger<>))
                .AddSingleton(typeof(IFbMigrationSqlGeneratorBehavior), typeof(FbMigrationSqlGeneratorBehavior))
                .AddSingleton<IValueConverterSelector, ValueConverterSelector>()
                .AddLogging();

            new FbDesignTimeServices().ConfigureDesignTimeServices(serviceCollection);

            return serviceCollection.AddEntityFrameworkFirebird()
                .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory());
        }
    }
}
