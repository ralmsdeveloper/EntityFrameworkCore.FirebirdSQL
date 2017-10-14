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

using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.FirebirdSql.Update.Internal
{
    public interface IFbUpdateSqlGenerator : IUpdateSqlGenerator
    {
        ResultSetMapping AppendBulkInsertOperation(
            StringBuilder commandStringBuilder,
            StringBuilder variablesParameters,
            IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition);

        ResultSetMapping AppendBulkUpdateOperation(
            StringBuilder commandStringBuilder,
            StringBuilder variablesParameters,
            IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition);

        ResultSetMapping AppendBulkDeleteOperation(
            StringBuilder commandStringBuilder,
            StringBuilder variablesParameters,
            IReadOnlyList<ModificationCommand> modificationCommands,
            int commandPosition);
    }
   
}
