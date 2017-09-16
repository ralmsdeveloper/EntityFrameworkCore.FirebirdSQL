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

using System;
using System.Text;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore.Migrations.Internal
{
    public class FbHistoryRepository : HistoryRepository
    {

        public FbHistoryRepository(
            HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);
            history.Property(h => h.MigrationId).HasColumnType("VARCHAR(95)");
            history.Property(h => h.ProductVersion).HasColumnType("VARCHAR(32)").IsRequired();
        }

        protected override string ExistsSql
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("select 1 from rdb$relations where rdb$view_blr is null ")
                       .Append("And (rdb$system_flag is null or rdb$system_flag = 0) ")
                       .Append($"And RDB$RELATION_NAME='{TableName}';");  
                return builder.ToString();
            }
        }

        protected override bool InterpretExistsResult(object value) => value != null;

        public override string GetCreateIfNotExistsScript()
        {
            return GetCreateScript();
        }

        public override string GetBeginIfNotExistsScript(string migrationId)
        {
            throw new NotSupportedException("Not supported by Firebird EF Core");
        }

        public override string GetBeginIfExistsScript(string migrationId)
        {
            throw new NotSupportedException("Not supported by Firebird EF Core");
        }

        public override string GetEndIfScript()
        {
            throw new NotSupportedException("Not supported by Firebird EF Core");
        }
    }
}
