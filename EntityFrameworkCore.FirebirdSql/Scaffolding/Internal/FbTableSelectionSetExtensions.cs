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

using System;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace EntityFrameworkCore.FirebirdSql.Scaffolding.Internal
{
	internal static class FbTableSelectionSetExtensions
	{
		public static bool Allows(this TableSelectionSet _tableSelectionSet, string schemaName, string tableName)
		{
			if (_tableSelectionSet == null
				|| (_tableSelectionSet.Schemas.Count == 0
				&& _tableSelectionSet.Tables.Count == 0))
			{
				return true;
			} 
			var result = false; 
			foreach (var schemaSelection in _tableSelectionSet.Schemas)
			{
				if (EqualsWithQuotes(schemaSelection.Text, schemaName))
				{
					schemaSelection.IsMatched = true;
					result = true;
				}
			}

			foreach (var tableSelection in _tableSelectionSet.Tables)
			{
				var components = tableSelection.Text.Split('.');
				if (components.Length == 1
					? EqualsWithQuotes(components[0], tableName)
					: EqualsWithQuotes(components[0], schemaName) && EqualsWithQuotes(components[1], tableName))
				{
					tableSelection.IsMatched = true;
					result = true;
				}
			}

			return result;
		}

		static bool EqualsWithQuotes(string expr, string name)
		{
			return expr[0] == '"' && expr[expr.Length - 1] == '"'
				? expr.Substring(0, expr.Length - 2).Equals(name)
				: expr.Equals(name, StringComparison.OrdinalIgnoreCase);
		}
	}
}
