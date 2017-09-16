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

using System.Collections.Generic;

namespace System.Text
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, IEnumerable<string> values, string separator = ", ")
                => stringBuilder.AppendJoin(values, (sb, value) => sb.Append(value), separator);

        public static StringBuilder AppendJoin(this StringBuilder stringBuilder, string separator, params string[] values)
            => stringBuilder.AppendJoin(values, (sb, value) => sb.Append(value), separator);

        public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder,IEnumerable<T> values,Action<StringBuilder, T> joinAction,string separator = ", ")
        {
            var appended = false;

            foreach (var value in values)
            {
                joinAction(stringBuilder, value);
                stringBuilder.Append(separator);
                appended = true;
            }

            if (appended) 
                stringBuilder.Length -= separator.Length; 

            return stringBuilder;
        }

        public static StringBuilder AppendJoin<T, TParam>(this StringBuilder stringBuilder, IEnumerable<T> values, TParam param, Action<StringBuilder, T, TParam> joinAction, string separator = ", ")
        {
            var appended = false; 
            foreach (var value in values)
            {
                joinAction(stringBuilder,  value, param);
                stringBuilder.Append(separator);
                appended = true;
            }

            if (appended) 
                stringBuilder.Length -= separator.Length;
         
            return stringBuilder;
        }

        public static StringBuilder AppendJoinUpadate<T, TParam>(this StringBuilder stringBuilder, IEnumerable<T> values, TParam param, Action<StringBuilder, T, TParam> joinAction, string separator = ", ")
        {
            var appended = false;

            foreach (var value in values)
            {
                joinAction(stringBuilder,value, param);
                stringBuilder.Append(separator);
                appended = true;
            }

            if (appended)
                stringBuilder.Length -= separator.Length;

            return stringBuilder;
        }

        public static StringBuilder AppendJoin<T, TParam1, TParam2>(this StringBuilder stringBuilder, IEnumerable<T> values, TParam1 param1, TParam2 param2, Action<StringBuilder, T, TParam1, TParam2> joinAction, string separator = ", ")
        {
            var appended = false;

            foreach (var value in values)
            {
                joinAction(stringBuilder, value, param1, param2);
                stringBuilder.Append(separator);
                appended = true;
            }

            if (appended) 
                stringBuilder.Length -= separator.Length; 

            return stringBuilder;
        }
    }
}
