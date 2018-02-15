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
using System.Linq;
using System.Collections.Generic;
using EntityFrameworkCore.FirebirdSql.Utilities;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators; 

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{
    public sealed class FbCompositeMemberTranslator : RelationalCompositeMemberTranslator
    {
        private static readonly List<Type> _translatorsMethods
            = TranslatorMethods.GetTranslatorMethods<IMemberTranslator>().ToList();

        public FbCompositeMemberTranslator(RelationalCompositeMemberTranslatorDependencies dependencies)
            : base(dependencies)
            => AddTranslators(_translatorsMethods.Select(type => (IMemberTranslator)Activator.CreateInstance(type)));
    }
}
