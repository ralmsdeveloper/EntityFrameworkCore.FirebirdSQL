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

using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace EntityFrameworkCore.FirebirdSql.Scaffolding.Metadata
{
    public static class FbDatabaseModelExtensions
    {
        public static FbDatabaseColumnAnnotations Firebird(this DatabaseColumn column)
            => new FbDatabaseColumnAnnotations(column);
         
        public static FbIndexModelAnnotations Firebird(this DatabaseIndex index)
            => new FbIndexModelAnnotations(index);  
    }
}
