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

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal; 

namespace Microsoft.EntityFrameworkCore
{ 

    public static class FbPropertyBuilderExtensions
    { 
        public static PropertyBuilder UseFbIdentityColumn(this PropertyBuilder propertyBuilder)
        {
            GetFbInternalBuilder(propertyBuilder).ValueGenerationStrategy(FbValueGenerationStrategy.IdentityColumn);
            return propertyBuilder;
        } 

        public static PropertyBuilder<TProperty> UseFbIdentityColumn<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseFbIdentityColumn((PropertyBuilder)propertyBuilder);

        private static FbPropertyBuilderAnnotations GetFbInternalBuilder(PropertyBuilder propertyBuilder)
            => propertyBuilder.GetInfrastructure<InternalPropertyBuilder>().Firebird(ConfigurationSource.Explicit);
    }
}
