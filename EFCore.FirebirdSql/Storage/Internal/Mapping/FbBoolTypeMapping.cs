/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
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
using System.Data;
using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage
{
     
    public class FbBoolTypeMapping : BoolTypeMapping
    {
        public FbBoolTypeMapping()
            : base("BOOLEAN",System.Data.DbType.Boolean)
        {
        }

        protected override void ConfigureParameter(DbParameter parameter)
             => ((FbParameter)parameter).FbDbType = FbDbType.Boolean;

        protected override string GenerateNonNullSqlLiteral(object value)
             => (bool)value ? "TRUE" : "FALSE";
    }
}
