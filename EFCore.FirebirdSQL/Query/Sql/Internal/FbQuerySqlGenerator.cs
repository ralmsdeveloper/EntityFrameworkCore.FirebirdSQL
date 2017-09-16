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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Sql.Internal
{
	/// <summary>
	///     This API supports the Entity Framework Core infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	public class FbQuerySqlGenerator : DefaultQuerySqlGenerator, IFbExpressionVisitor
	{
		protected override string TypedTrueLiteral => "TRUE";
		protected override string TypedFalseLiteral => "FALSE";

		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public FbQuerySqlGenerator(
			QuerySqlGeneratorDependencies dependencies,
			SelectExpression selectExpression)
			: base(dependencies, selectExpression)
		{
		}

		public override Expression VisitSelect(SelectExpression selectExpression)
		{
			base.VisitSelect(selectExpression);

			//Fix Any()
			if (selectExpression.Type == typeof(bool))
				Sql.Append(" FROM RDB$DATABASE");
			return selectExpression;
		}

		protected override void GenerateTop(SelectExpression selectExpression)
		{
			if (selectExpression.Limit != null)
			{
				Sql.AppendLine().Append("FIRST ");
				Visit(selectExpression.Limit);
				Sql.AppendLine().Append(" ");
			}

			if (selectExpression.Offset != null)
			{
				if (selectExpression.Limit == null)
					Sql.AppendLine().Append("FIRST ").Append(1000000).Append(" ");
				Sql.Append(" SKIP ");
				Visit(selectExpression.Offset);
			}
		}

		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		protected override void GenerateLimitOffset(SelectExpression selectExpression)
		{
		}

		/// <summary>
		///     This API supports the Entity Framework Core infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public override Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
		{
			return base.VisitSqlFunction(sqlFunctionExpression);
		}


		protected override void GenerateProjection(Expression projection)
		{
			var aliasedProjection = projection as AliasExpression;
			var expressionToProcess = aliasedProjection?.Expression ?? projection;
			var updatedExperssion = ExplicitCastToBool(expressionToProcess);

			expressionToProcess = aliasedProjection != null
				? new AliasExpression(aliasedProjection.Alias, updatedExperssion)
				: updatedExperssion;

			base.GenerateProjection(expressionToProcess);
		}

		private Expression ExplicitCastToBool(Expression expression)
		{
			return (expression as BinaryExpression)?.NodeType == ExpressionType.Coalesce
			       && expression.Type.UnwrapNullableType() == typeof(bool)
				? new ExplicitCastExpression(expression, expression.Type)
				: expression;
		}

		protected override Expression VisitBinary(BinaryExpression binaryExpression)
		{
			if (binaryExpression.NodeType == ExpressionType.Add &&
			    binaryExpression.Left.Type == typeof(string) &&
			    binaryExpression.Right.Type == typeof(string))
			{
				Sql.Append("(");
				Visit(binaryExpression.Left);
				Sql.Append("||");
				var exp = Visit(binaryExpression.Right);
				Sql.Append(")");
				return exp;
			}

			var expr = base.VisitBinary(binaryExpression);

			return expr;
		}

		public virtual Expression VisitSubString(FbSubStringExpression sbString)
		{
			Sql.Append(" SUBSTRING(");
			Visit(sbString.SubjectExpression);
			Sql.Append(" FROM ");
			Visit(sbString.FromExpression);
			Sql.Append(" FOR ");
			Visit(sbString.ForExpression);
			Sql.Append(")");
			return sbString;
		}

		public Expression VisitRegexp(FbRegexpExpression regexpExpression)
		{
			throw new NotImplementedException();
		}
	}
}
