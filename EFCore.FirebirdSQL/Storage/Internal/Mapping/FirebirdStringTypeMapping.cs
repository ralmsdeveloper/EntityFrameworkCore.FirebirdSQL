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
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;
using System.Collections.Concurrent;
using FirebirdSql.Data.FirebirdClient;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class FirebirdSqlStringTypeMapping : StringTypeMapping
    {
        readonly FbDbType _fbDbType;

        public FirebirdSqlStringTypeMapping(string storeType, FbDbType fbDbType)
            : base(storeType)
        {
            _fbDbType = fbDbType;
        }

        protected override void ConfigureParameter([NotNull] DbParameter parameter)
            => ((FbParameter)parameter).FbDbType = _fbDbType;
    }

    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    //public class FirebirdSqlStringTypeMapping : RelationalTypeMapping
    //{
    //    //private readonly int _maxSpecificSize;

    //    ///// <summary>
    //    /////     Initializes a new instance of the <see cref="FirebirdSqlStringTypeMapping" /> class.
    //    ///// </summary>
    //    ///// <param name="storeType"> The name of the database type. </param>
    //    ///// <param name="dbType"> The <see cref="DbType" /> to be used. </param>
    //    ///// <param name="unicode"> A value indicating whether the type should handle Unicode data or not. </param>
    //    //public FirebirdSqlStringTypeMapping(
    //    //    [NotNull] string storeType,
    //    //    [CanBeNull] DbType? dbType,
    //    //    bool unicode = false)
    //    //    : this(storeType, dbType, unicode, size: null)
    //    //{
    //    //}

    //    ///// <summary>
    //    /////     Initializes a new instance of the <see cref="FirebirdSqlStringTypeMapping" /> class.
    //    ///// </summary>
    //    ///// <param name="storeType"> The name of the database type. </param>
    //    ///// <param name="dbType"> The <see cref="DbType" /> to be used. </param>
    //    ///// <param name="unicode"> A value indicating whether the type should handle Unicode data or not. </param>
    //    ///// <param name="size"> The size of data the property is configured to store, or null if no size is configured. </param>
    //    //public FirebirdSqlStringTypeMapping(
    //    //    [NotNull] string storeType,
    //    //    [CanBeNull] DbType? dbType,
    //    //    bool unicode,
    //    //    int? size)
    //    //    : base(storeType, dbType, unicode, size)
    //    //{
    //    //    _maxSpecificSize = CalculateSize(unicode, size);
    //    //}

    //    //private static int CalculateSize(bool unicode, int? size)
    //    //    => unicode
    //    //        ? size.HasValue && size < 4000 ? size.Value : 4000
    //    //        : size.HasValue && size < 8000 ? size.Value : 8000;

    //    ///// <summary>
    //    /////     This API supports the Entity Framework Core infrastructure and is not intended to be used
    //    /////     directly from your code. This API may change or be removed in future releases.
    //    ///// </summary>
    //    //public override RelationalTypeMapping Clone(string storeType, int? size)
    //    //    => new FirebirdSqlStringTypeMapping(
    //    //        storeType,
    //    //        DbType,
    //    //        IsUnicode,
    //    //        size);

    //    ///// <summary>
    //    /////     This API supports the Entity Framework Core infrastructure and is not intended to be used
    //    /////     directly from your code. This API may change or be removed in future releases.
    //    ///// </summary>
    //    //protected override void ConfigureParameter(DbParameter parameter)
    //    //{
    //    //    // For strings and byte arrays, set the max length to the size facet if specified, or
    //    //    // 8000 bytes if no size facet specified, if the data will fit so as to avoid query cache
    //    //    // fragmentation by setting lots of different Size values otherwise always set to 
    //    //    // -1 (unbounded) to avoid SQL client size inference.

    //    //    var value = parameter.Value;
    //    //    var length = (value as string)?.Length ?? (value as byte[])?.Length;

    //    //    parameter.Size = value == null || value == DBNull.Value || length != null && length <= _maxSpecificSize
    //    //        ? _maxSpecificSize
    //    //        : -1;
    //    //}

    //    ///// <summary>
    //    /////     Generates the SQL representation of a literal value.
    //    ///// </summary>
    //    ///// <param name="value">The literal value.</param>
    //    ///// <returns>
    //    /////     The generated string.
    //    ///// </returns>
    //    //protected override string GenerateNonNullSqlLiteral(object value)
    //    //    => IsUnicode
    //    //        ? $"N'{EscapeSqlLiteral((string)value)}'" // Interpolation okay; strings
    //    //        : $"'{EscapeSqlLiteral((string)value)}'";



    //    static readonly RelationalTypeMapping UnboundedStringMapping
    //       = new FirebirdSqlTypeMapper("text", typeof(string), FirebirdSql.Data.FirebirdClient.FbDbType.Text);

    //    readonly ConcurrentDictionary<int, RelationalTypeMapping> _boundedStringMappings
    //        = new ConcurrentDictionary<int, RelationalTypeMapping>();

    //    public RelationalTypeMapping FindMapping(bool unicode, bool keyOrIndex, int? maxLength)
    //    {
    //        return maxLength.HasValue
    //            ? _boundedStringMappings.GetOrAdd(maxLength.Value,
    //                  ml => new FirebirdSqlTypeMapper($"varchar({maxLength})", typeof(string))
    //              )
    //            : UnboundedStringMapping;
    //    }

    //}
}
