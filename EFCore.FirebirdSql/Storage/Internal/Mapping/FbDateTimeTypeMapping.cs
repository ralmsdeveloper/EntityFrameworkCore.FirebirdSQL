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
using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using JetBrains.Annotations;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal.Mapping
{


    public class FbTimestampTypeMapping : FbTypeMapping
    {
        public FbTimestampTypeMapping()
            : base("TIMESTAMP", typeof(DateTime), FbDbType.TimeStamp)
        { }

        protected FbTimestampTypeMapping(RelationalTypeMappingParameters parameters, FbDbType fbDbType)
            : base(parameters, fbDbType)
        { }

        public override RelationalTypeMapping Clone(string storeType, int? size)
            => new FbTimestampTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size), FbDbType);

        public override CoreTypeMapping Clone(ValueConverter converter)
            => new FbTimestampTypeMapping(Parameters.WithComposedConverter(converter), FbDbType);

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"TIMESTAMP '{(DateTime)value:yyyy-MM-dd HH:mm:ss.fff}'";
    }

    public class FbDateTypeMapping : FbTypeMapping
    {
        public FbDateTypeMapping()
            : base("DATE", typeof(DateTime), FbDbType.Date)
        { }

        protected FbDateTypeMapping(RelationalTypeMappingParameters parameters, FbDbType fbDbType)
            : base(parameters, fbDbType)
        { }

        public override RelationalTypeMapping Clone(string storeType, int? size)
            => new FbDateTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size), FbDbType);

        public override CoreTypeMapping Clone(ValueConverter converter)
            => new FbDateTypeMapping(Parameters.WithComposedConverter(converter), FbDbType);

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"DATE '{(DateTime)value:yyyy-MM-dd}'";
    }

    public class FbTimeTypeMapping : FbTypeMapping
    {
        public FbTimeTypeMapping()
            : base("TIME", typeof(TimeSpan), FbDbType.Time)
        { }

        protected FbTimeTypeMapping(RelationalTypeMappingParameters parameters, FbDbType fbDbType)
            : base(parameters, fbDbType)
        { }

        public override RelationalTypeMapping Clone(string storeType, int? size)
            => new FbTimeTypeMapping(Parameters.WithStoreTypeAndSize(storeType, size), FbDbType);

        public override CoreTypeMapping Clone(ValueConverter converter)
            => new FbTimeTypeMapping(Parameters.WithComposedConverter(converter), FbDbType);

        protected override string GenerateNonNullSqlLiteral(object value)
            => $"TIME '{(TimeSpan)value:HH:mm:ss.fff}'";
    }
    //public class FbDateTimeTypeMapping : DateTimeTypeMapping
    //{
    //    public FbDbType FbDbType { get; }

    //    public FbDateTimeTypeMapping(string storeType, FbDbType fbDbType)
    //        : base(storeType, dbType: (DbType)fbDbType)
    //        => FbDbType = fbDbType;


    //    protected override void ConfigureParameter(DbParameter parameter)
    //    {
    //        base.ConfigureParameter(parameter);
    //        ((FbParameter)parameter).FbDbType = FbDbType; 
    //    }

    //    protected FbDateTimeTypeMapping(RelationalTypeMappingParameters parameters)
    //       : base(parameters)
    //    {
    //    }

    //    protected override string SqlLiteralFormatString
    //    {
    //        get
    //        {
    //            switch (FbDbType)
    //            {
    //                case FbDbType.TimeStamp:
    //                    return "'{0:yyyy-MM-dd HH:mm:ss.fffk}'";
    //                case FbDbType.Date:
    //                    return "'{0:yyyy-MM-dd}'";
    //                case FbDbType.Time:
    //                    return "'{0:HH:mm:ss.fffk}'";
    //                default:
    //                    throw new ArgumentOutOfRangeException();
    //            }
    //        }
    //    }
    //}
}
