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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityFrameworkCore.FirebirdSql.Utilities
{
    public class TranslatorMethods
    {
        public static IEnumerable<Type> GetTranslatorMethods<TInteface>()
            => Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t.GetInterfaces().Any(i => i == typeof(TInteface)) &&
                    t.GetConstructors().Any(c => c.GetParameters().Length == 0));

    }
}
