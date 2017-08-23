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
        
        // boolean
        private readonly FirebirdSqlBoolTypeMapping _bit     = new FirebirdSqlBoolTypeMapping();

        // integers 
	    private readonly ShortTypeMapping _smallint         = new ShortTypeMapping("smallint", DbType.Int16); 
        private readonly IntTypeMapping _int                = new IntTypeMapping("integer", DbType.Int32); 
	    private readonly LongTypeMapping _bigint            = new LongTypeMapping("int64", DbType.Int64); 

	    // decimals
	    private readonly DecimalTypeMapping _decimal        = new DecimalTypeMapping("decimal", DbType.Decimal);
	    private readonly DoubleTypeMapping _double          = new DoubleTypeMapping("double", DbType.Double);
        private readonly FloatTypeMapping _float            = new FloatTypeMapping("float");
        private readonly DecimalTypeMapping _money            = new DecimalTypeMapping("decimal(18, 4)");

        // binary
        private readonly RelationalTypeMapping _binary           = new FirebirdSqlByteArrayTypeMapping("char", DbType.Binary, 8000);
        private readonly RelationalTypeMapping _varbinary        = new FirebirdSqlByteArrayTypeMapping("char", DbType.Binary,8000);
 
	    // string
        private readonly FirebirdSqlStringTypeMapping _char       = new FirebirdSqlStringTypeMapping("char", FbDbType.VarChar);
        private readonly FirebirdSqlStringTypeMapping _varchar    = new FirebirdSqlStringTypeMapping("varchar", FbDbType.VarChar);
        private readonly FirebirdSqlStringTypeMapping _text = new FirebirdSqlStringTypeMapping("blob sub_type text", FbDbType.Text);
       


        // DateTime
        private readonly FirebirdSqlDateTimeTypeMapping _dateTime  = new FirebirdSqlDateTimeTypeMapping("timestamp", DbType.DateTime);
        private readonly TimeSpanTypeMapping _date                 = new TimeSpanTypeMapping("date", DbType.Date); 

        // guid
	    private readonly GuidTypeMapping _uniqueidentifier   = new GuidTypeMapping("char(38)", DbType.Guid);

        readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
        readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;
        private readonly HashSet<string> _disallowedMappings;

        public FirebirdSqlTypeMapper([NotNull] RelationalTypeMapperDependencies dependencies)
            : base(dependencies)
        {
            _storeTypeMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    // boolean
                    { "bit", _bit },
                    // integers 
                    { "smallint", _smallint },
                    { "integer", _int },
                    { "int64", _bigint },  
                    // decimals
                    { "decimal", _decimal },
                    { "double", _double },
                    { "float", _float },
                     { "decimal(18, 4)", _money },
                    // binary
                    { "binary", _binary },
                    { "varbinary", _varbinary } , 
                    // string
                    { "char", _char },
                    { "varchar", _varchar }, 
                    { "blob sub_type text", _text },  
                    // DateTime
                    { "timestamp", _dateTime },
                    { "date", _date },  

                    // guid
                    { "char(36)", _uniqueidentifier }
                };

            _clrTypeMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
	                // boolean
	                { typeof(bool), _bit },

	                // integers
	                { typeof(short), _smallint }, 
	                { typeof(int), _int }, 
	                { typeof(long), _bigint }, 

	                // decimals
	                { typeof(decimal), _decimal },
	                { typeof(float), _float },
	                { typeof(double), _double },
                      
	                // DateTime
	                { typeof(DateTime), _dateTime }, 
	                { typeof(TimeSpan), _date },
                     
	                // guid
	                { typeof(Guid), _uniqueidentifier }
                };

            _disallowedMappings
                = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "binary",
                    "char",
                    "varbinary",
                    "varchar" 
                }; 

            StringMapper = new FirebirdSqlStringRelationalTypeMapper();
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
       // public override IByteArrayRelationalTypeMapper ByteArrayMapper { get; }

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
            {
                throw new ArgumentException("Daty Type Invalid!" + storeType);
            }
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
