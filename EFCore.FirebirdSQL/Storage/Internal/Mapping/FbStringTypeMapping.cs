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
using System.Data.Common;

using System.Collections.Concurrent;
using FirebirdSql.Data.FirebirdClient;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    public class FbStringTypeMapping : StringTypeMapping
    {
        readonly FbDbType _fbDbType;

        public FbStringTypeMapping(string storeType, FbDbType fbDbType, bool unicode = false,int? size=null)
            : base(storeType)
        {
            _fbDbType = fbDbType;
        }

        protected override void ConfigureParameter(DbParameter parameter)
            => ((FbParameter)parameter).FbDbType = _fbDbType;
    }
 
}
