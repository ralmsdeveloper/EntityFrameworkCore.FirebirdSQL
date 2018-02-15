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
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.FirebirdSql.Extensions
{
    public static class FbMetadataExtensions
    {
        public static FbPropertyAnnotations Firebird(this IProperty property)
            => new FbPropertyAnnotations(property);

        public static RelationalKeyAnnotations Firebird(this IKey key)
            => new RelationalKeyAnnotations(key);

        public static RelationalForeignKeyAnnotations Firebird(this IForeignKey foreignKey)
            => new RelationalForeignKeyAnnotations(foreignKey);

        public static RelationalIndexAnnotations Firebird(this IIndex index)
            => new RelationalIndexAnnotations(index);

        public static FbModelAnnotations Firebird(this IModel model)
            => new FbModelAnnotations(model);

        public static RelationalEntityTypeAnnotations Firebird(this IEntityType entityType)
            => new RelationalEntityTypeAnnotations(entityType);
    }
}
