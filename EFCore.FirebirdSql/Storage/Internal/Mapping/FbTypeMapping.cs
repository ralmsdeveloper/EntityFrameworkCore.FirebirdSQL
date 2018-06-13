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
using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal.Mapping
{
    public abstract class FbTypeMapping : RelationalTypeMapping
    {
        public FbDbType FbDbType { get; }

        public FbTypeMapping(
            string storeType,
            Type clrType,
            FbDbType fbDbType)
            : base(storeType, clrType)
            => FbDbType = fbDbType;

        protected FbTypeMapping(RelationalTypeMappingParameters parameters, FbDbType fbDbType)
            : base(parameters)
            => FbDbType = fbDbType;

        protected override void ConfigureParameter(DbParameter parameter)
        {
            base.ConfigureParameter(parameter);

            ((FbParameter)parameter).FbDbType = FbDbType;
        }
    }
}
