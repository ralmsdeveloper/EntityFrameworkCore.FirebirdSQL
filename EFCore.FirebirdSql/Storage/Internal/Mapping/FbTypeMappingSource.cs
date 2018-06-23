/*
 *          Copyright (c) 2017-2018 Rafael Almeida (ralms@ralms.net)
 *                               Jiri Cincura   (juri@cincura.net)
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
using System.Collections.Generic;
using System.Data;
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using FirebirdSql.Data.FirebirdClient;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal.Mapping
{
    public class FbTypeMappingSource : RelationalTypeMappingSource
    {
        private readonly bool _isLegacy;
        public const int BinaryMaxSize = int.MaxValue;
        public const int VarcharMaxSize = 32764;
        public const int NVarcharMaxSize = 2000;
        public const int DefaultDecimalPrecision = 18;
        public const int DefaultDecimalScale = 2;

        private readonly LongTypeMapping _bigint;
        private readonly FbBoolTypeMapping _boolean;
        private readonly ShortTypeMapping _smallint = new ShortTypeMapping("SMALLINT", DbType.Int16);
        private readonly IntTypeMapping _integer = new IntTypeMapping("INTEGER", DbType.Int32);
        private readonly FbStringTypeMapping _char = new FbStringTypeMapping("CHAR", FbDbType.Char);
        private readonly FbStringTypeMapping _varchar = new FbStringTypeMapping("VARCHAR", FbDbType.VarChar);
        private readonly FbStringTypeMapping _varcharMax = new FbStringTypeMapping($"VARCHAR({VarcharMaxSize})", FbDbType.VarChar, size: VarcharMaxSize);
        private readonly FbStringTypeMapping _nvarcharMax = new FbStringTypeMapping($"VARCHAR({NVarcharMaxSize})", FbDbType.VarChar, size: NVarcharMaxSize);
        private readonly FbStringTypeMapping _clob = new FbStringTypeMapping("BLOB SUB_TYPE TEXT", FbDbType.Text);
        private readonly FbByteArrayTypeMapping _binary = new FbByteArrayTypeMapping();
        private readonly FloatTypeMapping _float = new FloatTypeMapping("FLOAT");
        private readonly DoubleTypeMapping _double = new DoubleTypeMapping("DOUBLE PRECISION");
        private readonly DecimalTypeMapping _decimal = new DecimalTypeMapping($"DECIMAL({DefaultDecimalPrecision},{DefaultDecimalScale})");
        private readonly FbTimestampTypeMapping _timeStamp = new FbTimestampTypeMapping();
        private readonly FbDateTypeMapping _date = new FbDateTypeMapping();
        private readonly FbTimeTypeMapping _time = new FbTimeTypeMapping();
        private readonly FbGuidTypeMapping _guid = new FbGuidTypeMapping();
        private readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
        private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;

        private readonly HashSet<string> _disallowedMappings
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
                "CHARACTER",
                "CHAR",
                "VARCHAR",
                "CHARACTER VARYING",
                "CHAR VARYING",
        };

        public FbTypeMappingSource(
            TypeMappingSourceDependencies dependencies,
            RelationalTypeMappingSourceDependencies relationalDependencies,
            IFbOptions options)
            : base(dependencies, relationalDependencies)
        {
            _isLegacy = options.IsLegacyDialect;
            _bigint = new LongTypeMapping(
                _isLegacy
                    ? "INTEGER" : "BIGINT",
            DbType.Int64);

            _boolean = new FbBoolTypeMapping(
                _isLegacy || options.ServerVersion?.Major < 3
                    ? "SMALLINT" : "BOOLEAN");

            _storeTypeMappings = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
            {
                { "BOOLEAN", _boolean },
                { "SMALLINT", _smallint },
                { "INTEGER", _integer },
                { "BIGINT", _bigint },
                { "CHAR", _char },
                { "VARCHAR", _varchar },
                { "BLOB SUB_TYPE TEXT", _clob },
                { "BLOB SUB_TYPE BINARY", _binary },
                { "FLOAT", _float },
                { "DOUBLE PRECISION", _double },
                { "DECIMAL", _decimal },
                { "NUMERIC", _decimal },
                { "TIMESTAMP", _timeStamp },
                { "DATE", _date },
                { "TIME", _time },
                { "CHAR(16) CHARACTER SET OCTETS", _guid },
            };

            _clrTypeMappings = new Dictionary<Type, RelationalTypeMapping>()
            {
                { typeof(bool), _boolean },
                { typeof(short), _smallint },
                { typeof(int), _integer },
                { typeof(long), _bigint },
                { typeof(float), _float },
                { typeof(double), _double},
                { typeof(decimal), _decimal },
                { typeof(byte[]), _binary },
                { typeof(DateTime), _timeStamp },
                { typeof(TimeSpan), _time },
                { typeof(Guid), _guid }
            };


        }

        protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
            => FindRawMapping(mappingInfo)?.Clone(mappingInfo);


        protected override void ValidateMapping(CoreTypeMapping mapping, IProperty property)
        {
            var relationalMapping = mapping as RelationalTypeMapping;

            // Refactor this later
            if (_disallowedMappings.Contains(relationalMapping?.StoreType))
            {
                if (property == null)
                {
                    throw new ArgumentException($"UnqualifiedDataType: {relationalMapping.StoreType}");
                }

                throw new ArgumentException($"UnqualifiedDataTypeOnProperty: {relationalMapping.StoreType}/{property.Name}");
            }
        }

        private RelationalTypeMapping FindRawMapping(RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            var storeTypeName = mappingInfo.StoreTypeName;
            var storeTypeNameBase = mappingInfo.StoreTypeNameBase;

            if (storeTypeName != null)
            {
                if (clrType == typeof(float)
                    && mappingInfo.Size != null
                    && mappingInfo.Size <= 24
                    && (storeTypeNameBase.Equals("decimal", StringComparison.OrdinalIgnoreCase)
                        || storeTypeNameBase.Equals("double precision", StringComparison.OrdinalIgnoreCase)))
                {
                    return _decimal;
                }

                if (_storeTypeMappings.TryGetValue(storeTypeName, out var mapping)
                    || _storeTypeMappings.TryGetValue(storeTypeNameBase, out mapping))
                {
                    return clrType == null
                           || mapping.ClrType == clrType
                        ? mapping
                        : null;
                }
            }

            if (clrType != null)
            {
                if (_clrTypeMappings.TryGetValue(clrType, out var mapping))
                {
                    return mapping;
                }

                if (clrType == typeof(string))
                {
                    var size = mappingInfo.Size ?? (mappingInfo.IsKeyOrIndex ? 256 : NVarcharMaxSize);
                    return new FbStringTypeMapping(
                        $"VARCHAR({size})",
                        FbDbType.VarChar,
                        size: size);
                }

                if (clrType == typeof(byte[]))
                {
                    return _binary;
                }
            }

            return null;
        }
    }
}
