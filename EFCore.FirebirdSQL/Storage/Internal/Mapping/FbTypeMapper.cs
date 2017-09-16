/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSQL
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

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using FirebirdSql.Data.FirebirdClient;
using EntityFrameworkCore.FirebirdSQL.Storage;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
	public class FbTypeMapper : RelationalTypeMapper
	{
		// bool
		private FbBoolTypeMapping _boolean
			=> new FbBoolTypeMapping();

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
			=> new FbByteArrayTypeMapping("BLOB SUB_TYPE 0 SEGMENT SIZE 80", DbType.Binary);

		private RelationalTypeMapping _varbinary
			=> new FbByteArrayTypeMapping("BLOB SUB_TYPE 0 SEGMENT SIZE 80", DbType.Binary);

		private FbByteArrayTypeMapping _varbinary767
			=> new FbByteArrayTypeMapping("BLOB SUB_TYPE 0 SEGMENT SIZE 80", DbType.Binary, 767);

		private RelationalTypeMapping _varbinaryMax
			=> new FbByteArrayTypeMapping("BLOB SUB_TYPE 0 SEGMENT SIZE 80", DbType.Binary);

		// String   
		private FbStringTypeMapping _char
			=> new FbStringTypeMapping("CHAR", FbDbType.VarChar);

		private FbStringTypeMapping _varchar
			=> new FbStringTypeMapping("VARCHAR", FbDbType.VarChar);

		private FbStringTypeMapping _varchar127
			=> new FbStringTypeMapping("VARCHAR(127)", FbDbType.VarChar, true, 127);

		private FbStringTypeMapping _varcharMax
			=> new FbStringTypeMapping("VARCHAR(32765)", FbDbType.VarChar);

		private FbStringTypeMapping _text
			=> new FbStringTypeMapping("BLOB SUB_TYPE TEXT", FbDbType.Text);

		// DateTime
		private FbDateTimeTypeMapping _dateTime
			=> new FbDateTimeTypeMapping("TIMESTAMP", FbDbType.TimeStamp);

		private FbDateTimeTypeMapping _date
			=> new FbDateTimeTypeMapping("DATE", FbDbType.Date);

		private FbDateTimeTypeMapping _time
			=> new FbDateTimeTypeMapping("TIME", FbDbType.Time);

		// guid
		private FirebirdGuidTypeMapping _guid
			=> new FirebirdGuidTypeMapping("CHAR(16) CHARACTER SET OCTETS", FbDbType.Guid);

		//Row Version
		private RelationalTypeMapping _rowVersion
			=> new FbDateTimeTypeMapping("TIMESTAMP", FbDbType.TimeStamp);

		private readonly Dictionary<string, RelationalTypeMapping> _storeTypeMappings;
		private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings;
		private readonly List<string> _disallowedMappings;

		public FbTypeMapper(RelationalTypeMapperDependencies dependencies)
			: base(dependencies)
		{
			_storeTypeMappings
				= new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
				{
					// Boolean
					{"BOOLEAN", _boolean},
					// Integer 
					{"SMALLINT", _smallint},
					{"INTEGER", _integer},
					{"BIGINT", _bigint},
					// Decimal
					{"DECIMAL(18,4)", _decimal},
					{"DOUBLE PRECICION(18,4)", _double},
					{"FLOAT", _float},
					// Binary
					{"BINARY", _binary},
					{"VARBINARY", _varbinary},
					// String
					{"CHAR", _char},
					{"VARCHAR", _varchar},
					{"BLOB SUB_TYPE TEXT", _text},
					// DateTime
					{"TIMESTAMP", _dateTime},
					{"DATE", _date},
					{"TIME", _time},
					// Guid
					{"CHAR(16) CHARACTER SET OCTETS", _guid}
				};

			_clrTypeMappings
				= new Dictionary<Type, RelationalTypeMapping>
				{
					// Boolean
					{typeof(bool), _boolean},
					// Integer
					{typeof(short), _smallint},
					{typeof(int), _integer},
					{typeof(long), _bigint},
					// Decimal
					{typeof(decimal), _decimal},
					{typeof(float), _float},
					{typeof(double), _double},
					// DateTime
					{typeof(DateTimeOffset), _dateTime},
					{typeof(DateTime), _date},
					{typeof(TimeSpan), _time},
					// Guid
					{typeof(Guid), _guid}
				};

			_disallowedMappings
				= new List<string>
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
					size => new FbByteArrayTypeMapping(
						"BLOB SUB_TYPE 0 SEGMENT SIZE 80",
						DbType.Binary));

			StringMapper = new FbStringRelationalTypeMapper();

			StringMapper
				= new StringRelationalTypeMapper(
					32765,
					_varcharMax,
					_varcharMax,
					_varchar127,
					size => new FbStringTypeMapping(
						"VARCHAR(" + size + ")",
						FbDbType.VarChar,
						false,
						size),
					32765,
					_varcharMax,
					_varcharMax,
					_varchar127,
					size => new FbStringTypeMapping(
						"VARCHAR(" + size + ")",
						FbDbType.VarChar,
						false,
						size));
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
		protected override string GetColumnType(IProperty property)
		{
			return property.Firebird().ColumnType;
		}

		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		protected override IReadOnlyDictionary<Type, RelationalTypeMapping> GetClrTypeMappings()
		{
			return _clrTypeMappings;
		}

		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		protected override IReadOnlyDictionary<string, RelationalTypeMapping> GetStoreTypeMappings()
		{
			return _storeTypeMappings;
		}

		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public override RelationalTypeMapping FindMapping(Type clrType)
		{
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
		{
			return base.RequiresKeyMapping(property) || property.IsIndex();
		}
	}
}