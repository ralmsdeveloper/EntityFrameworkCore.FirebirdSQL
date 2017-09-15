/*                 
 *                    EntityFrameworkCore.FirebirdSQL
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
 *                  All Rights Reserved.
 */

using System;
using System.Text; 
using Microsoft.EntityFrameworkCore.Infrastructure.Internal; 

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
	public class FbSqlGenerationHelper : RelationalSqlGenerationHelper
	{
		private readonly IFbOptions _options;

		public FbSqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies, IFbOptions options)
			: base(dependencies)
		{
			_options = options;
		}

		public override string EscapeIdentifier(string identifier)
		{
			return identifier.Substring(0, Math.Min(identifier.Length, _options.Settings.ObjectLengthName));
		}

		public override void EscapeIdentifier(StringBuilder builder, string identifier)
		{
			builder.Append(identifier.Substring(0, Math.Min(identifier.Length, _options.Settings.ObjectLengthName)));
		}

		public override string DelimitIdentifier(string identifier)
		{
			return $"\"{EscapeIdentifier(identifier)}\"";
		}

		public override void DelimitIdentifier(StringBuilder builder, string identifier)
		{
			builder.Append('"');
			EscapeIdentifier(builder, identifier.Substring(0, Math.Min(identifier.Length, _options.Settings.ObjectLengthName)));
			builder.Append('"');
		}

		public override string GenerateParameterName(string name)
		{
			return $"@{name.Substring(0, Math.Min(name.Length, _options.Settings.ObjectLengthName))}";
		}

		public override void GenerateParameterName(StringBuilder builder, string name)
		{
			builder.Append("@").Append(name.Substring(0, Math.Min(name.Length, _options.Settings.ObjectLengthName)));
		}
	}
}