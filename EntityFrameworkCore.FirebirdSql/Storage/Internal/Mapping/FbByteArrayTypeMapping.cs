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
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal.Mapping
{ 
	public class FbByteArrayTypeMapping : ByteArrayTypeMapping
	{
		public FbByteArrayTypeMapping()
			: base("BLOB SUB_TYPE BINARY", System.Data.DbType.Binary)
		{ }

		protected override string GenerateNonNullSqlLiteral(object value)
		{
			var hex =  BitConverter.ToString(((byte[])value)).Replace("-", string.Empty);
			return $"x'{hex}'";
		}
	}
}
