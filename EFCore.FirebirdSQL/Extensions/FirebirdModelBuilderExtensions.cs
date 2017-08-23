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
    /// <summary>
    ///     FirebirdSQL specific extension methods for <see cref="ModelBuilder" />.
    /// </summary>
    public static class FirebirdSqlModelBuilderExtensions
    {
        /// <summary>
        ///     Configures the model to use the FirebirdSQL IDENTITY Compatibility FB.3 feature to generate values for key properties
        ///     marked as <see cref="ValueGenerated.OnAdd" />, when targeting FirebirdSQL. This is the default
        ///     behavior when targeting FirebirdSQL.
        /// </summary>
        /// <param name="modelBuilder"> The model builder. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        public static ModelBuilder ForFirebirdSqlUseIdentityColumns(
            [NotNull] this ModelBuilder modelBuilder)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder)); 
            var property = modelBuilder.Model; 
            property.FirebirdSql().ValueGenerationStrategy = FirebirdSqlValueGenerationStrategy.IdentityColumn; 
            return modelBuilder;
        }

        public static ModelBuilder ForFirebirdSqlUseComputedColumns(
            [NotNull] this ModelBuilder modelBuilder)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder)); 
            var property = modelBuilder.Model; 
            property.FirebirdSql().ValueGenerationStrategy = FirebirdSqlValueGenerationStrategy.ComputedColumn; 
            return modelBuilder;
        }
    }
}
