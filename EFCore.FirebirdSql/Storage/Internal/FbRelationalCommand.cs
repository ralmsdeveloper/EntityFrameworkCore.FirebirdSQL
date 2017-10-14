/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *                               Jiri Cincura      (jiri@cincura.net)
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{
    public class FirebirdRelationalCommand : RelationalCommand
    {
        public FirebirdRelationalCommand(IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, string commandText, IReadOnlyList<IRelationalParameter> parameters)
            : base(logger, commandText, parameters)
        {
        }

        protected override object Execute(IRelationalConnection connection, DbCommandMethod executeMethod, IReadOnlyDictionary<string, object> parameterValues)
        {  
            return ExecuteAsync( connection, executeMethod, parameterValues)
                .GetAwaiter()
                .GetResult();
        } 
    }
}
