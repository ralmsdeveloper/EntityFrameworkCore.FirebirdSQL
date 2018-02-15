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

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Migrations
{
    public class FbMigrationSqlGeneratorBehavior : IFbMigrationSqlGeneratorBehavior
    {
        private readonly ISqlGenerationHelper _sqlHelper;

        public FbMigrationSqlGeneratorBehavior(ISqlGenerationHelper sqlHelper)
            => _sqlHelper = sqlHelper;

        private string SequenceName(string column, string table)
            => $"GEN_{column}_{table}";

        private string TriggerName(string column, string table)
            => $"TRG_{column}_{table}";

        public virtual void CreateIdentityForColumn(
            MigrationCommandListBuilder builder,
            string columnName,
            string tableName)
        {
            var sequenceName = SequenceName(columnName, tableName);
            var triggerName = TriggerName(columnName, tableName);

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
            builder.Append(_sqlHelper.DelimitIdentifier(sequenceName));
            builder.Append("';");
            builder.DecrementIndent();
            builder.AppendLine();
            builder.AppendLine("end");
            builder.AppendLine("END");
            builder.EndCommand();

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
            builder.Append(_sqlHelper.DelimitIdentifier(sequenceName));
            builder.Append(";");
            builder.DecrementIndent();
            builder.AppendLine();
            builder.AppendLine("end");
            builder.Append("END");
            builder.EndCommand();
        }

        public void DropIdentityForColumn(
            MigrationCommandListBuilder builder,
            string columnName,
            string tableName)
        {
            var triggerName = TriggerName(columnName, tableName);

            builder.AppendLine("EXECUTE BLOCK");
            builder.AppendLine("AS");
            builder.AppendLine("BEGIN");
            builder.Indent();
            builder.Append("if (exists(select 1 from rdb$triggers where rdb$trigger_name = '");
            builder.Append(triggerName);
            builder.Append("')) then");
            builder.AppendLine();
            builder.AppendLine("begin");
            builder.Indent();
            builder.Append("execute statement 'drop trigger ");
            builder.Append(_sqlHelper.DelimitIdentifier(triggerName));
            builder.Append("'");
            builder.Append(_sqlHelper.StatementTerminator);
            builder.AppendLine();
            builder.DecrementIndent();
            builder.AppendLine("end");
            builder.DecrementIndent();
            builder.Append("END");
            builder.EndCommand();
        }
    }
}
