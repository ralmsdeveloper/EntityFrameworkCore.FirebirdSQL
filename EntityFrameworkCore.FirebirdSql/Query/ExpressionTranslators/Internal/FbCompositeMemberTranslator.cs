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

//$Authors = Jiri Cincura (jiri@cincura.net), Rafael Almeida(ralms@ralms.net)

using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.FirebirdSql.Utilities;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators; 

namespace EntityFrameworkCore.FirebirdSql.Query.ExpressionTranslators.Internal
{
	public sealed class FbCompositeMemberTranslator : RelationalCompositeMemberTranslator
	{
		static readonly List<Type> Translators = TranslatorsHelper.GetTranslators<IMemberTranslator>().ToList();

		public FbCompositeMemberTranslator(RelationalCompositeMemberTranslatorDependencies dependencies)
			: base(dependencies)
		{
			AddTranslators(Translators.Select(t => (IMemberTranslator)Activator.CreateInstance(t)));
		}
	}
}
