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

//$Authors = Jiri Cincura (jiri@cincura.net), Rafael Almeida(ralms@ralms.net)

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Migrations.Internal
{
    public class FbHistoryRepository : HistoryRepository
    {
        public FbHistoryRepository(HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        { }

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
                var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
                return $@"
SELECT COUNT(*)
FROM rdb$relations r
WHERE
    COALESCE(r.rdb$system_flag, 0) = 0
    AND
    rdb$view_blr IS NULL
    AND
    rdb$relation_name = {stringTypeMapping.GenerateSqlLiteral(TableName)}";
            }
        }

        protected override bool InterpretExistsResult(object value) => value != DBNull.Value;

        public override string GetCreateIfNotExistsScript() => GetCreateScript();

        public override string GetBeginIfExistsScript(string migrationId)
            => throw new NotSupportedException("Generating idempotent scripts is currently not supported.");

        public override string GetBeginIfNotExistsScript(string migrationId)
            => throw new NotSupportedException("Generating idempotent scripts is currently not supported.");

        public override string GetEndIfScript()
            => throw new NotSupportedException("Generating idempotent scripts is currently not supported.");
    }
}
