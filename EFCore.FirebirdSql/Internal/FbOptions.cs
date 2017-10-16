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
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EntityFrameworkCore.FirebirdSql.Utilities;

namespace EntityFrameworkCore.FirebirdSql.Internal
{
    public class FbOptions : IFbOptions
    {
        private Lazy<FbSettings> _fbSettings;
        public virtual FbSettings Settings => _fbSettings.Value;

        public virtual void Initialize(IDbContextOptions options)
        {
            var fbOptions = GetOptions(options);
            _fbSettings = new Lazy<FbSettings>(() =>
                fbOptions.Connection != null
                ? new FbSettings().GetSettings(fbOptions.Connection)
                : new FbSettings().GetSettings(fbOptions.ConnectionString));
        }

        public virtual void Validate(IDbContextOptions options)
        {
            var fbOptions = GetOptions(options);
        }

        private FbOptionsExtension GetOptions(IDbContextOptions options)
        => options.FindExtension<FbOptionsExtension>() ?? new FbOptionsExtension();

    }
}
