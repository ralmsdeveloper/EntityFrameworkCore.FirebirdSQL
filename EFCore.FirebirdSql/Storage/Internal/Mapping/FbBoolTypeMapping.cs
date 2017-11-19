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
using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage
{
    public class FbBoolTypeMapping : BoolTypeMapping
    {
        public const string TrueLiteral = "T";
        public const string FalseLiteral = "F";

        public FbBoolTypeMapping()
            : base("CHAR(1)", System.Data.DbType.Boolean)
        { }

        protected override void ConfigureParameter(DbParameter parameter)
            => ((FbParameter)parameter).Value = Convert.ToBoolean(parameter.Value) ? 'T' : 'F';

        protected override string GenerateNonNullSqlLiteral(object value)
            => (bool)value ? $"'{TrueLiteral}'" : $"'{FalseLiteral}'";
    }
}
