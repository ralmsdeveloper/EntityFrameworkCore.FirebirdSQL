
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.FirebirdSQL.Storage
{
    public class FirebirdSqlStringRelationalTypeMapper : IStringRelationalTypeMapper
    {

        static readonly RelationalTypeMapping UnboundedStringMapping
               = new FirebirdSqlTypeMapping("blob sub_type text", typeof(string), FirebirdSql.Data.FirebirdClient.FbDbType.Text);

        readonly ConcurrentDictionary<int, RelationalTypeMapping> _boundedStringMappings
            = new ConcurrentDictionary<int, RelationalTypeMapping>();

        public RelationalTypeMapping FindMapping(bool unicode, bool keyOrIndex, int? maxLength)
        {
            return maxLength.HasValue
                ? _boundedStringMappings.GetOrAdd(maxLength.Value,
                      ml => new FirebirdSqlTypeMapping($"varchar({maxLength})", typeof(string))
                  )
                : UnboundedStringMapping;
        }

    }
}


