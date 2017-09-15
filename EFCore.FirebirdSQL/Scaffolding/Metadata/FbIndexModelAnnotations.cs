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

namespace Microsoft.EntityFrameworkCore.Scaffolding.Metadata
{
	public class FbIndexModelAnnotations
	{
		private readonly DatabaseIndex _index;

		public FbIndexModelAnnotations(DatabaseIndex index)
		{ 
			_index = index;
		}

		/// <summary>
		/// If the index contains an expression (rather than simple column references), the expression is contained here.
		/// This is currently unsupported and will be ignored.
		/// </summary>
		public string Expression
		{
			get { return _index[FbDatabaseModelAnnotationNames.Expression] as string; }
 
			set { _index[FbDatabaseModelAnnotationNames.Expression] = value; }
		}
	}
}
