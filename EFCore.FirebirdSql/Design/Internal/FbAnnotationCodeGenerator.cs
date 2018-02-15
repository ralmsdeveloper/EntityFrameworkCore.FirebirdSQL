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

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.FirebirdSql.Design.Internal
{
    public class FbAnnotationCodeGenerator : AnnotationCodeGenerator
    {
        public FbAnnotationCodeGenerator(AnnotationCodeGeneratorDependencies dependencies)
            : base(dependencies)
        { }

        public override bool IsHandledByConvention(IModel model, IAnnotation annotation) => true;

        public override MethodCallCodeFragment GenerateFluentApi(IIndex index, IAnnotation annotation) => null;
    }
}
