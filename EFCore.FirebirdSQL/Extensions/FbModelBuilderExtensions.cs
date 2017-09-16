/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSQL
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
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     FirebirdSQL specific extension methods for <see cref="ModelBuilder" />.
    /// </summary>
    public static class FbModelBuilderExtensions
    {
        /// <summary>
        ///     Configures the model to use the FirebirdSQL IDENTITY Compatibility FB.3 feature to generate values for key properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting FirebirdSQL. This is the default
        ///     behavior when targeting FirebirdSQL.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder ForFbUseIdentityColumns(
            this ModelBuilder modelBuilder)
        {
            var property = modelBuilder.Model; 
            property.Firebird().ValueGenerationStrategy = FbValueGenerationStrategy.IdentityColumn; 
            return modelBuilder;
        }

        public static ModelBuilder ForFbUseComputedColumns(this ModelBuilder modelBuilder)
        {
            var property = modelBuilder.Model; 
            property.Firebird().ValueGenerationStrategy = FbValueGenerationStrategy.ComputedColumn; 
            return modelBuilder;
        }
    }
}
