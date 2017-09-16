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

using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal; 
using System.Collections.Concurrent; 

namespace EntityFrameworkCore.FirebirdSQL.Storage
{
    public class FbStringRelationalTypeMapper : IStringRelationalTypeMapper
    {

        static readonly RelationalTypeMapping UnboundedStringMapping
               = new FbTypeMapping("BLOB SUB_TYPE TEXT", typeof(string), FbDbType.Text);

        readonly ConcurrentDictionary<int, RelationalTypeMapping> _boundedStringMappings
            = new ConcurrentDictionary<int, RelationalTypeMapping>();

        public RelationalTypeMapping FindMapping(bool unicode, bool keyOrIndex, int? maxLength)
        {
            return maxLength.HasValue
                ? _boundedStringMappings.GetOrAdd(maxLength.Value,
                      ml => new FbTypeMapping($"VARCHAR({maxLength})", typeof(string))
                  )
                : UnboundedStringMapping;
        }

    }
}