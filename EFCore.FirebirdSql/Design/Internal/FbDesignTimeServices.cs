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

using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using EntityFrameworkCore.FirebirdSql.Internal;
using EntityFrameworkCore.FirebirdSql.Scaffolding.Internal;
using EntityFrameworkCore.FirebirdSql.Storage.Internal.Mapping;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.FirebirdSql.Design.Internal
{
    public class FbDesignTimeServices : IDesignTimeServices
    {
        public virtual void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
            => serviceCollection
                .AddSingleton<IRelationalTypeMappingSource, FbTypeMappingSource>()
                .AddSingleton<IDatabaseModelFactory, FbDatabaseModelFactory>()
                .AddSingleton<IProviderConfigurationCodeGenerator, FbScaffoldingCodeGenerator>()
                .AddSingleton<IAnnotationCodeGenerator, FbAnnotationCodeGenerator>()
                .AddSingleton<IFbOptions, FbOptions>();
    }
}

