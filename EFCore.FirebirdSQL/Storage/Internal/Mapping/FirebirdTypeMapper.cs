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

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using FirebirdSql.Data.FirebirdClient;
using EntityFrameworkCore.FirebirdSQL.Storage;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class FirebirdSqlTypeMapper : RelationalTypeMapper
    {

        // bool
        private FirebirdSqlBoolTypeMapping _boolean
                                            => new FirebirdSqlBoolTypeMapping();
        // int 
        private ShortTypeMapping _smallint
                                            => new ShortTypeMapping("SMALLINT", DbType.Int16);
        private IntTypeMapping _integer
                                            => new IntTypeMapping("INTEGER", DbType.Int32);
        private LongTypeMapping _bigint
                                            => new LongTypeMapping("BIGINT", DbType.Int64);
        // decimal
        private DecimalTypeMapping _decimal
                                            => new DecimalTypeMapping("DECIMAL(18,4)", DbType.Decimal);
        private DoubleTypeMapping _double
                                            => new DoubleTypeMapping("DOUBLE PRECISION", DbType.Double);
        private FloatTypeMapping _float                      
                                            => new FloatTypeMapping("FLOAT");
        // Binary
        private RelationalTypeMapping _binary                
                                            => new FirebirdSqlByteArrayTypeMapping("BLOB SUB_TYPE 0 SEGMENT SIZE 80", DbType.Binary);
        private RelationalTypeMapping _varbinary             
                                            => new FirebirdSqlByteArrayTypeMapping("BLOB SUB_TYPE 0 SEGMENT SIZE 80", DbType.Binary);
        private FirebirdSqlByteArrayTypeMapping _varbinary767
                                            => new FirebirdSqlByteArrayTypeMapping("BLOB SUB_TYPE 0 SEGMENT SIZE 80", DbType.Binary,767);
        private RelationalTypeMapping _varbinaryMax          
                                            => new FirebirdSqlByteArrayTypeMapping("BLOB SUB_TYPE 0 SEGMENT SIZE 80", DbType.Binary);

        // String   
        private FirebirdSqlStringTypeMapping _char        
                                            => new FirebirdSqlStringTypeMapping("CHAR", FbDbType.VarChar);
        private FirebirdSqlStringTypeMapping _varchar     
                                            => new FirebirdSqlStringTypeMapping("VARCHAR", FbDbType.VarChar);
        private FirebirdSqlStringTypeMapping _varchar127 
                                            => new FirebirdSqlStringTypeMapping("VARCHAR(127)", FbDbType.VarChar, true, 127);
        private FirebirdSqlStringTypeMapping _varcharMax 
                                            => new FirebirdSqlStringTypeMapping("VARCHAR(4000)", FbDbType.VarChar);
        private FirebirdSqlStringTypeMapping _text        
                                            => new FirebirdSqlStringTypeMapping("BLOB SUB_TYPE TEXT", FbDbType.Text);
        // DateTime
        private FirebirdSqlDateTimeTypeMapping _dateTime  
                                            => new FirebirdSqlDateTimeTypeMapping("TIMESTAMP", FbDbType.TimeStamp);
        private FirebirdSqlDateTimeTypeMapping _date      
                                            => new FirebirdSqlDateTimeTypeMapping("DATE", FbDbType.Date);
        private FirebirdSqlDateTimeTypeMapping _time      
                                            => new FirebirdSqlDateTimeTypeMapping("TIME", FbDbType.Time);
        // guid
        private FirebirdGuidTypeMapping _guid             
                                            => new FirebirdGuidTypeMapping("CHAR(16) CHARACTER SET OCTETS", FbDbType.Guid);
        //Row Version
        private RelationalTypeMapping _rowVersion 
                                            => new FirebirdSqlDateTimeTypeMapping("TIMESTAMP", FbDbType.TimeStamp);

        private Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
        private Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;
        private List<string> _disallowedMappings;

        public FirebirdSqlTypeMapper([NotNull] RelationalTypeMapperDependencies dependencies)
            : base(dependencies)
        {
            _storeTypeMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    // Boolean
                    { "BOOLEAN", _boolean },
                    // Integer 
                    { "SMALLINT", _smallint },
                    { "INTEGER", _integer },
                    { "BIGINT", _bigint },  
                    // Decimal
                    { "DECIMAL(18,4)", _decimal },
                    { "DOUBLE PRECICION(18,4)", _double },
                    { "FLOAT", _float }, 
                    // Binary
                    { "BINARY", _binary },
                    { "VARBINARY", _varbinary } , 
                    // String
                    { "CHAR", _char },
                    { "VARCHAR", _varchar }, 
                    { "BLOB SUB_TYPE TEXT", _text },  
                    // DateTime
                    { "TIMESTAMP", _dateTime },
                    { "DATE", _date },
                    { "TIME", _time },   
                    // Guid
                    { "CHAR(16) CHARACTER SET OCTETS", _guid }
                };

            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
	                // Boolean
	                { typeof(bool), _boolean }, 
	                // Integer
	                { typeof(short), _smallint }, 
	                { typeof(int), _integer }, 
	                { typeof(long), _bigint },  
	                // Decimal
	                { typeof(decimal), _decimal },
	                { typeof(float), _float },
	                { typeof(double), _double }, 
	                // DateTime
	                { typeof(DateTime), _dateTime }, 
	                { typeof(TimeSpan), _date },
                    { typeof(TimeSpan), _time }, 
	                // Guid
	                { typeof(Guid), _guid }
                };

            _disallowedMappings
                = new List<string>()
                {
                    "BINARY",
                    "CHAR", 
                    "VARBINARY",
                    "VARCHAR" 
                };

            ByteArrayMapper
                = new ByteArrayRelationalTypeMapper(
                    8000,
                    _binary,
                    _varbinaryMax,
                    _varbinary767,
                    _rowVersion,
                     size => new FirebirdSqlByteArrayTypeMapping(
                        "BLOB SUB_TYPE 0 SEGMENT SIZE 80",
                        DbType.Binary));

            StringMapper = new FirebirdSqlStringRelationalTypeMapper();

            StringMapper
                = new StringRelationalTypeMapper(
                    maxBoundedAnsiLength: 4000,
                    defaultAnsiMapping: _varcharMax,
                    unboundedAnsiMapping: _varcharMax,
                    keyAnsiMapping: _varchar127,
                    createBoundedAnsiMapping: size => new FirebirdSqlStringTypeMapping(
                        "VARCHAR(" + size + ")",
                        FbDbType.VarChar,
                        unicode: false,
                        size: size),
                    maxBoundedUnicodeLength: 4000,
                    defaultUnicodeMapping: _varcharMax,
                    unboundedUnicodeMapping: _varcharMax,
                    keyUnicodeMapping: _varchar127,
                    createBoundedUnicodeMapping: size => new FirebirdSqlStringTypeMapping(
                        "VARCHAR(" + size + ")",
                        FbDbType.VarChar,
                        unicode: false,
                        size: size));
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override IByteArrayRelationalTypeMapper ByteArrayMapper { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override IStringRelationalTypeMapper StringMapper { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override void ValidateTypeName(string storeType)
        {
            if (_disallowedMappings.Contains(storeType)) 
                throw new ArgumentException("Daty Type Invalid!" + storeType);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override string GetColumnType(IProperty property) => property.FirebirdSql().ColumnType;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> GetClrTypeMappings()
            => _clrTypeMappings;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override IReadOnlyDictionary<string, RelationalTypeMapping> GetStoreTypeMappings()
            => _storeTypeMappings;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override RelationalTypeMapping FindMapping(Type clrType)
        {
            Check.NotNull(clrType, nameof(clrType));

            clrType = clrType.UnwrapNullableType().UnwrapEnumType();
             
            return clrType == typeof(string)
                ? _varchar
                : (clrType == typeof(byte[])
                    ? _varbinary
                    : base.FindMapping(clrType));
        }

        // Indexes in FirebirdSQL have a max size of 900 bytes
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        protected override bool RequiresKeyMapping(IProperty property)
            => base.RequiresKeyMapping(property) || property.IsIndex();
    }
}
