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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;


namespace Microsoft.EntityFrameworkCore
{

    public static class FirebirdSqlMetadataExtensions
    {
        public static FirebirdSqlPropertyAnnotations FirebirdSql([NotNull] this IMutableProperty property)
            => (FirebirdSqlPropertyAnnotations)FirebirdSql((IProperty)property);
        
        public static IFirebirdSqlPropertyAnnotations FirebirdSql([NotNull] this IProperty property)
            => new FirebirdSqlPropertyAnnotations(Check.NotNull(property, nameof(property)));
        
        public static FirebirdSqlEntityTypeAnnotations FirebirdSql([NotNull] this IMutableEntityType entityType)
            => (FirebirdSqlEntityTypeAnnotations)FirebirdSql((IEntityType)entityType);
        
        public static IFirebirdSqlEntityTypeAnnotations FirebirdSql([NotNull] this IEntityType entityType)
            => new FirebirdSqlEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)));
        
        public static FirebirdSqlKeyAnnotations FirebirdSql([NotNull] this IMutableKey key)
            => (FirebirdSqlKeyAnnotations)FirebirdSql((IKey)key);
        
        public static IFirebirdSqlKeyAnnotations FirebirdSql([NotNull] this IKey key)
            => new FirebirdSqlKeyAnnotations(Check.NotNull(key, nameof(key)));
        
        public static FirebirdSqlIndexAnnotations FirebirdSql([NotNull] this IMutableIndex index)
            => (FirebirdSqlIndexAnnotations)FirebirdSql((IIndex)index);
        
        public static IFirebirdSqlIndexAnnotations FirebirdSql([NotNull] this IIndex index)
            => new FirebirdSqlIndexAnnotations(Check.NotNull(index, nameof(index)));
        
        public static FirebirdSqlModelAnnotations FirebirdSql([NotNull] this IMutableModel model)
            => (FirebirdSqlModelAnnotations)FirebirdSql((IModel)model);
        
        public static IFirebirdSqlModelAnnotations FirebirdSql([NotNull] this IModel model)
            => new FirebirdSqlModelAnnotations(Check.NotNull(model, nameof(model)));
    }
}
