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
using System.Linq.Expressions;
using EntityFrameworkCore.FirebirdSql.Query.Sql.Internal;

namespace EntityFrameworkCore.FirebirdSql.Query.Expressions.Internal
{ 
    public class FbSubStringExpression : Expression
    {
	    public virtual Expression ValueExpression { get; }
	    public virtual Expression FromExpression { get; }
	    public virtual Expression ForExpression { get; }

	    public FbSubStringExpression(Expression valueExpression, Expression fromExpression, Expression forExpression)
	    {
		    ValueExpression = valueExpression;
		    FromExpression = fromExpression;
		    ForExpression = forExpression;
	    }

	    public override ExpressionType NodeType => ExpressionType.Extension;
	    public override bool CanReduce => false;
	    public override Type Type => typeof(string);

	    protected override Expression Accept(ExpressionVisitor visitor)
	    {
		    if (visitor is IFbExpressionVisitor specificVisitor)
		    {
			    return specificVisitor.VisitSubString(this);
		    }
		    else
		    {
			    return base.Accept(visitor);
		    }
	    }

	    protected override Expression VisitChildren(ExpressionVisitor visitor)
	    {
		    var newValueExpression = visitor.Visit(ValueExpression);
		    var newFromExpression = visitor.Visit(FromExpression);
		    var newForExpression = visitor.Visit(ForExpression);

		    return newValueExpression != ValueExpression || newFromExpression != FromExpression || newForExpression != ForExpression
			           ? new FbSubStringExpression(newValueExpression, newFromExpression, newForExpression)
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
		    return obj.GetType() == GetType() && Equals((FbSubStringExpression)obj);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
			    var hashCode = ValueExpression.GetHashCode();
			    hashCode = (hashCode * 397) ^ FromExpression.GetHashCode();
			    hashCode = (hashCode * 397) ^ ForExpression.GetHashCode();
			    return hashCode;
		    }
	    }
    }
}