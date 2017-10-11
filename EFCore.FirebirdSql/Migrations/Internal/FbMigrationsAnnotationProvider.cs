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

using System.Collections.Generic;
using EntityFrameworkCore.FirebirdSql.Extensions;
using EntityFrameworkCore.FirebirdSql.Metadata.Internal;  
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata; 
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFrameworkCore.FirebirdSql.Migrations.Internal
{
    public class FbMigrationsAnnotationProvider : MigrationsAnnotationProvider
    {
        public FbMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
            : base(dependencies)
        { }

        public override IEnumerable<IAnnotation> For(IProperty property)
        {
            if (property.Firebird().ValueGenerationStrategy.HasValue)
            {
				yield return new Annotation(FbAnnotationNames.ValueGenerationStrategy, property.Firebird().ValueGenerationStrategy);
			}
        }
    }
}
