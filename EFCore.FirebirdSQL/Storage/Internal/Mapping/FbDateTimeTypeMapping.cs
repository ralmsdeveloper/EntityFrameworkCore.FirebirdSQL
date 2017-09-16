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
using System.Data;
using System.Text;

using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;

namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    ///     <para>
    ///         Represents the mapping between a .NET <see cref="DateTime" /> type and a database type.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class FbDateTimeTypeMapping : DateTimeTypeMapping
    { 
        private readonly string _storeType;
        private readonly FbDbType _fbDbType;

        public FbDateTimeTypeMapping(string storeType, FbDbType fbDbType)
            : base(storeType)
        {
            _fbDbType = fbDbType;
            _storeType = storeType;
        }

        protected override void ConfigureParameter(DbParameter parameter)
            => ((FbParameter)parameter).FbDbType = _fbDbType;

        protected override string SqlLiteralFormatString
        {
            get
            {
                switch (_fbDbType)
                {
                    case FbDbType.TimeStamp:
                        return $"'{@"{0:yyyy-MM-dd HH\:mm\:ss}"}'";
                    case FbDbType.Date:
                        return $"'{@"{0:yyyy-MM-dd}"}'";
                    case FbDbType.Time:
                        return $"'{@"{0:HH\:mm\:ss}"}'";
                }
                return null;
            }
        } 
    }
}
