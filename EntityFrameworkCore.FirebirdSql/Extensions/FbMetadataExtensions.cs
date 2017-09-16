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

using Microsoft.EntityFrameworkCore.Metadata; 

namespace Microsoft.EntityFrameworkCore
{

    public static class FbMetadataExtensions
    {
        public static FbPropertyAnnotations Firebird(this IMutableProperty property)
            => (FbPropertyAnnotations)Firebird((IProperty)property);
        
        public static IFbPropertyAnnotations Firebird(this IProperty property)
            => new FbPropertyAnnotations(property);
        
        public static FbEntityTypeAnnotations Firebird(this IMutableEntityType entityType)
            => (FbEntityTypeAnnotations)Firebird((IEntityType)entityType);
        
        public static IFbEntityTypeAnnotations Firebird(this IEntityType entityType)
            => new FbEntityTypeAnnotations(entityType);
        
        public static FbKeyAnnotations Firebird(this IMutableKey key)
            => (FbKeyAnnotations)Firebird((IKey)key);
        
        public static IFbKeyAnnotations Firebird(this IKey key)
            => new FbKeyAnnotations(key);
        
        public static FbModelAnnotations Firebird(this IMutableModel model)
            => (FbModelAnnotations)Firebird((IModel)model);
        
        public static IFbModelAnnotations Firebird(this IModel model)
            => new FbModelAnnotations(model);
    }
}
