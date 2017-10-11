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
 
using EntityFrameworkCore.FirebirdSql.Metadata;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.FirebirdSql.Extensions
{ 
    public static class FbModelBuilderExtensions
    { 
        public static ModelBuilder ForFbUseIdentityColumns(this ModelBuilder modelBuilder)
        {
            var property = modelBuilder.Model; 
            property.Firebird().ValueGenerationStrategy = FbValueGenerationStrategy.IdentityColumn; 
            return modelBuilder;
        }

        public static ModelBuilder ForFbUseTriggerColumns(this ModelBuilder modelBuilder)
        {
            var property = modelBuilder.Model; 
            property.Firebird().ValueGenerationStrategy = FbValueGenerationStrategy.SequenceTrigger; 
            return modelBuilder;
        }
    }
}
