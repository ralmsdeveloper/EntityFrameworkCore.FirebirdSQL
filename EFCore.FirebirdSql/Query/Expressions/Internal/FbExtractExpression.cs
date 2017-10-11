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

//$Authors = Jiri Cincura (jiri@cincura.net), Rafael Almeida(ralms@ralms.net)

using System;
using System.Linq.Expressions;
using EntityFrameworkCore.FirebirdSql.Query.Sql.Internal;

namespace EntityFrameworkCore.FirebirdSql.Query.Expressions.Internal
{
	public class FbExtractExpression : Expression
	{
		public virtual string Part { get; }
		public virtual Expression ValueExpression { get; }

		public FbExtractExpression(string part, Expression valueExpression)
		{
			Part = part;
			ValueExpression = valueExpression;
		}

		public override ExpressionType NodeType => ExpressionType.Extension;
		public override bool CanReduce => false;
		public override Type Type => typeof(int);

		protected override Expression Accept(ExpressionVisitor visitor)
		{
			if (visitor is IFbExpressionVisitor specificVisitor)
			{
				return specificVisitor.VisitExtract(this);
			}
			else
			{
				return base.Accept(visitor);
			}
		}

		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			var newValueExpression = visitor.Visit(ValueExpression);

			return newValueExpression != ValueExpression
				       ? new FbExtractExpression(Part, newValueExpression)
				       : this;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return obj.GetType() == GetType() && Equals((FbExtractExpression) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Part.GetHashCode();
				hashCode = (hashCode * 397) ^ ValueExpression.GetHashCode();
				return hashCode;
			}
		} 
	}
}
