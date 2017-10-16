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

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.FirebirdSql.Infrastructure.Internal
{
    public sealed class FbOptionsExtension : RelationalOptionsExtension
    {
        public FbOptionsExtension()
        { }

        public FbOptionsExtension(RelationalOptionsExtension copyFrom)
            : base(copyFrom)
        { }

        protected override RelationalOptionsExtension Clone()
        {
            return new FbOptionsExtension(this);
        }

        public override bool ApplyServices(IServiceCollection services)
        {
            services.AddEntityFrameworkFirebird();
            return true;
        }
    }
}
