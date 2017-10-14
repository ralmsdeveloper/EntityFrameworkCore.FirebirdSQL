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
  
using System.Data.Common; 
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{
    public class FbStringTypeMapping : StringTypeMapping
    {
        readonly FbDbType _fbDbType;

        public FbStringTypeMapping(string storeType, FbDbType fbDbType, int? size = null)
            : base(storeType, unicode: true, size: size)
        {
            _fbDbType = fbDbType;
        }

        protected override void ConfigureParameter(DbParameter parameter)
        {
            ((FbParameter)parameter).FbDbType = _fbDbType;
        }

        protected override string GenerateNonNullSqlLiteral(object value)
        {
            //Credit Jiri Cincura
            return IsUnicode
                       ? $"_UTF8'{EscapeSqlLiteral((string)value)}'"
                       : $"'{EscapeSqlLiteral((string)value)}'";
        }
    }
 
}
