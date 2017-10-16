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

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;

namespace EntityFrameworkCore.FirebirdSql.Migrations
{
    public class FbMigrationSqlGeneratorBehavior : IFbMigrationSqlGeneratorBehavior
    {
        readonly ISqlGenerationHelper _sqlHelper;

        public FbMigrationSqlGeneratorBehavior(ISqlGenerationHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public IEnumerable<MigrationCommandListBuilder> CreateIdentityForColumn(
            MigrationCommandListBuilder builder,
            string columnName,
            string tableName)
        {
            var mergeColumnTable = string.Format("{0}_{1}", columnName, tableName).ToUpper();
            var sequenceName = string.Format("GEN_{0}", mergeColumnTable);
            var triggerName = string.Format("ID_{0}", mergeColumnTable);

            builder.AppendLine("EXECUTE BLOCK");
            builder.AppendLine("AS");
            builder.AppendLine("BEGIN");
            builder.Append("if (not exists(select 1 from rdb$generators where rdb$generator_name = '");
            builder.Append(sequenceName);
            builder.Append("')) then");
            builder.AppendLine();
            builder.AppendLine("begin");
            builder.Indent();
            builder.Append("execute statement 'create sequence ");
            builder.Append(sequenceName);
            builder.Append("';");
            builder.DecrementIndent();
            builder.AppendLine();
            builder.AppendLine("end");
            builder.AppendLine("END");
            yield return builder;

            builder.Append("CREATE OR ALTER TRIGGER ");
            builder.Append(_sqlHelper.DelimitIdentifier(triggerName));
            builder.Append(" ACTIVE BEFORE INSERT ON ");
            builder.Append(_sqlHelper.DelimitIdentifier(tableName));
            builder.AppendLine();
            builder.AppendLine("AS");
            builder.AppendLine("BEGIN");
            builder.Append("if (new.");
            builder.Append(_sqlHelper.DelimitIdentifier(columnName));
            builder.Append(" is null) then");
            builder.AppendLine();
            builder.AppendLine("begin");
            builder.Indent();
            builder.Append("new.");
            builder.Append(_sqlHelper.DelimitIdentifier(columnName));
            builder.Append(" = next value for ");
            builder.Append(sequenceName);
            builder.Append(";");
            builder.DecrementIndent();
            builder.AppendLine();
            builder.AppendLine("end");
            builder.Append("END");
            yield return builder;
        }

        public IEnumerable<MigrationCommandListBuilder> DropIdentityForColumn(MigrationCommandListBuilder builder, string columnName, string tableName)
        {
            throw new System.NotImplementedException();
        }
    }
}
