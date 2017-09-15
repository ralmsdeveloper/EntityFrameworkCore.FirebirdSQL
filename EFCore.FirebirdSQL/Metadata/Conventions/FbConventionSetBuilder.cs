/*                 
 *                    EntityFrameworkCore.FirebirdSQL
 *     
*
 *              
 *     Permission to use, copy, modify, and distribute this software and its
 *     documentation for any purpose, without fee, and without a written
 *     agreement is hereby granted, provided that the above copyright notice
 *     and this paragraph and the following two paragraphs appear in all copies. 
 * 
 *     The contents of this file are subject to the Initial
 *     Developer's Public License Version 1.0 (the "License");
 *     you may not use this file except in compliance with the
 *     License.
*
 *
 *     Software distributed under the License is distributed on
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations under the License.
 *
 *      Credits: Rafael Almeida (ralms@ralms.net)
 *                              Sergipe-Brazil
 *
 *
 *                              
 *                  All Rights Reserved.
 */


using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
	public class FbConventionSetBuilder : RelationalConventionSetBuilder
	{
		private readonly ISqlGenerationHelper _sqlGenerationHelper;
		private static IFbOptions _options;

		public FbConventionSetBuilder(
			RelationalConventionSetBuilderDependencies dependencies,
			IFbOptions options,
			ISqlGenerationHelper sqlGenerationHelper)
			: base(dependencies)
		{
			_sqlGenerationHelper = sqlGenerationHelper;
			_options = options;
		}

		public override ConventionSet AddConventions(ConventionSet conventionSet)
		{
			base.AddConventions(conventionSet);

			var valueGenerationStrategyConvention = new FbValueGenerationStrategyConvention();
			conventionSet.ModelInitializedConventions.Add(valueGenerationStrategyConvention);

			ReplaceConvention(conventionSet.PropertyAddedConventions,
				(DatabaseGeneratedAttributeConvention) valueGenerationStrategyConvention);
			ReplaceConvention(conventionSet.PropertyFieldChangedConventions,
				(DatabaseGeneratedAttributeConvention) valueGenerationStrategyConvention);

			return conventionSet;
		}
	}
}
