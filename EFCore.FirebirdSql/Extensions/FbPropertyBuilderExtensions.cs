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

using EntityFrameworkCore.FirebirdSql.Metadata;
using EntityFrameworkCore.FirebirdSql.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EntityFrameworkCore.FirebirdSql
{
    public static class FbPropertyBuilderExtensions
    {
        public static PropertyBuilder UseFirebirdIdentityColumn(this PropertyBuilder propertyBuilder)
        {
            GetFbInternalBuilder(propertyBuilder).ValueGenerationStrategy(FbValueGenerationStrategy.IdentityColumn);
            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> UseFirebirdIdentityColumn<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseFirebirdIdentityColumn((PropertyBuilder)propertyBuilder);

        public static PropertyBuilder UseFirebirdSequenceTrigger(this PropertyBuilder propertyBuilder)
        {
            GetFbInternalBuilder(propertyBuilder).ValueGenerationStrategy(FbValueGenerationStrategy.SequenceTrigger);
            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> UseFirebirdSequenceTrigger<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
            => (PropertyBuilder<TProperty>)UseFirebirdSequenceTrigger((PropertyBuilder)propertyBuilder);

        private static FbPropertyBuilderAnnotations GetFbInternalBuilder(PropertyBuilder propertyBuilder)
            => propertyBuilder.GetInfrastructure<InternalPropertyBuilder>().Firebird(ConfigurationSource.Explicit);
    }
}
