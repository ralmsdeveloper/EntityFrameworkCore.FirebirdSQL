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

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EntityFrameworkCore.FirebirdSql.Metadata.Internal
{ 
    public static class FbInternalMetadataBuilderExtensions
    {
        public static FbModelBuilderAnnotations Firebird(this InternalModelBuilder builder,ConfigurationSource configurationSource)
            => new FbModelBuilderAnnotations(builder, configurationSource);

        public static FbPropertyBuilderAnnotations Firebird( this InternalPropertyBuilder builder, ConfigurationSource configurationSource)
            => new FbPropertyBuilderAnnotations(builder, configurationSource);
         
        public static RelationalEntityTypeBuilderAnnotations Firebird(this InternalEntityTypeBuilder builder, ConfigurationSource configurationSource)
            => new RelationalEntityTypeBuilderAnnotations(builder, configurationSource);
         
        public static RelationalKeyBuilderAnnotations Firebird(this InternalKeyBuilder builder, ConfigurationSource configurationSource)
            => new RelationalKeyBuilderAnnotations(builder, configurationSource);

        public static RelationalIndexBuilderAnnotations Firebird(this InternalIndexBuilder builder, ConfigurationSource configurationSource)
            => new RelationalIndexBuilderAnnotations(builder, configurationSource);

        public static RelationalForeignKeyBuilderAnnotations Firebird(this InternalRelationshipBuilder builder, ConfigurationSource configurationSource)
            => new RelationalForeignKeyBuilderAnnotations(builder, configurationSource);
    }
}
