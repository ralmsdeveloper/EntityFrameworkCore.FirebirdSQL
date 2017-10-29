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

using System.Text;
using EntityFrameworkCore.FirebirdSql.Extensions;
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{
    public class FbSqlGenerationHelper : RelationalSqlGenerationHelper
    {
        private readonly IFbOptions _options;

        public FbSqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies, IFbOptions options)
            : base(dependencies)
        {
            _options = options;
        }

        public override string EscapeIdentifier(string identifier)
            => identifier.MaxLength(_options.ObjectLengthName); 

        public override void EscapeIdentifier(StringBuilder builder, string identifier)
            => builder.Append(identifier.MaxLength(_options.ObjectLengthName)); 

        public override string DelimitIdentifier(string identifier)
            => $"\"{EscapeIdentifier(identifier)}\"";

        public override void DelimitIdentifier(StringBuilder builder, string identifier)
        {
            builder.Append('"');
            EscapeIdentifier(builder, identifier.MaxLength(_options.ObjectLengthName));
            builder.Append('"');
        }

        public override string GenerateParameterName(string name)
            => $"@{name.MaxLength(_options.ObjectLengthName)}";

        public override void GenerateParameterName(StringBuilder builder, string name)
            => builder.Append("@").Append(name.MaxLength(_options.ObjectLengthName)); 
    }
}
