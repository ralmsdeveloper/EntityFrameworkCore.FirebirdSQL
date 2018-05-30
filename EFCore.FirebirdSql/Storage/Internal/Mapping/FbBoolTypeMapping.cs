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

using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using FB = FirebirdSql.Data.FirebirdClient;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal.Mapping
{
     
    public class FbBoolTypeMapping : BoolTypeMapping
    {
        private static readonly MethodInfo _readMethod
           = typeof(FbDataReader).GetTypeInfo().GetDeclaredMethod(nameof(FbDataReader.GetBoolean));

        private static CoreTypeMappingParameters _convert =
            new CoreTypeMappingParameters(
                typeof(bool),
                new ValueConverter<bool, int>(
                    v => v ? 1 : 0,
                    v => v.Equals(1))); 

        public FbBoolTypeMapping(string storeType)
            : base(
                new RelationalTypeMappingParameters(
                    _convert,
                    storeType))
        {
        }

        protected override void ConfigureParameter(DbParameter parameter)
            => ((FbParameter)parameter).FbDbType = FbDbType.SmallInt;
         
        protected override string GenerateNonNullSqlLiteral(object value)
             => (bool)value ? "1" : "0";
    }
}
