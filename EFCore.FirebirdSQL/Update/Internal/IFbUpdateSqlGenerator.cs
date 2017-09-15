/*                 
 *                    EntityFrameworkCore.FirebirdSQL
 *     
*
 *              
 *     Permission to use, copy, modify, and distribute this software and its
 *     documentation for any purpose, without fee, and without a written
 *     agreement is hereby granted, provided that the above copyright notice
 *     and this paragraph and the following two paragraphs appear in all copies. 
 * 
 *     The contents of this file are subject to the Initial
 *     Developer's Public License Version 1.0 (the "License");
 *     you may not use this file except in compliance with the
 *     License.
*
 *
 *     Software distributed under the License is distributed on
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *     express or implied.  See the License for the specific
 *     language governing rights and limitations under the License.
 *
 *      Credits: Rafael Almeida (ralms@ralms.net)
 *                              Sergipe-Brazil
 *
 *
 *                              
 *                  All Rights Reserved.
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
