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

namespace EntityFrameworkCore.FirebirdSql.Storage
{
    public class FbDateTimeTypeMapping : DateTimeTypeMapping
    {
        private readonly FbDbType _fbDbType;

        public FbDateTimeTypeMapping(string storeType, FbDbType fbDbType)
            : base(storeType)
            => _fbDbType = fbDbType;

        protected override void ConfigureParameter(DbParameter parameter)
            => ((FbParameter)parameter).FbDbType = _fbDbType;

        protected override string SqlLiteralFormatString
        {
            get
            {
                switch (_fbDbType)
                {
                    case FbDbType.TimeStamp:
                        return "'{0:yyyy-MM-dd HH:mm:ss}'";
                    case FbDbType.Date:
                        return "'{0:yyyy-MM-dd}'";
                    case FbDbType.Time:
                        return "'{0:HH:mm:ss}'";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
