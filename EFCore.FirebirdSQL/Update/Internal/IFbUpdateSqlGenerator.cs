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
using System.Text;


namespace Microsoft.EntityFrameworkCore.Update.Internal
{
    public interface IFbUpdateSqlGenerator : IUpdateSqlGenerator
    {
		ResultSetMapping AppendBlockInsertOperation(
		    StringBuilder commandStringBuilder,
		    StringBuilder headBlockStringBuilder,
		    IReadOnlyList<ModificationCommand> modificationCommands,
		    int commandPosition);

	    ResultSetMapping AppendBlockUpdateOperation(
		    StringBuilder commandStringBuilder,
		    StringBuilder headBlockStringBuilder,
		    IReadOnlyList<ModificationCommand> modificationCommands,
		    int commandPosition);

	    ResultSetMapping AppendBlockDeleteOperation(
		    StringBuilder commandStringBuilder,
		    IReadOnlyList<ModificationCommand> modificationCommands,
		    int commandPosition);
	}
   
}
