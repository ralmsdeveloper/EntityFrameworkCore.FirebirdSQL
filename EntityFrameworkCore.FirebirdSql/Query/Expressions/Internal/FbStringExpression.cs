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

using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using System;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.Expressions.Internal
{

    public class FbSubStringExpression : Expression
    {

        public FbSubStringExpression(Expression subjectExpression, Expression fromExpression, Expression forExpression)
        {
			SubjectExpression = subjectExpression;
            FromExpression = fromExpression;
            ForExpression = forExpression;
        }
        public virtual Expression SubjectExpression { get; }

        public virtual Expression FromExpression { get; }

        public virtual Expression ForExpression { get; }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(string);

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            var specificVisitor = visitor as IFbExpressionVisitor;

            return specificVisitor != null
                ? specificVisitor.VisitSubString(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newSubjectExpression = visitor.Visit(SubjectExpression);
            var newFromExpression = visitor.Visit(FromExpression);
            var newForExpression = visitor.Visit(ForExpression);

            return newFromExpression != FromExpression
                   || newForExpression != ForExpression
                   || newSubjectExpression != SubjectExpression
                ? new FbSubStringExpression(newSubjectExpression, newFromExpression, newForExpression)
                : this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false; 

            if (ReferenceEquals(this, obj)) 
                return true; 

            return obj.GetType() == GetType() && Equals((FbSubStringExpression)obj);
        }

        private bool Equals(FbSubStringExpression other)
            => Equals(FromExpression, other.FromExpression)
               && Equals(ForExpression, other.ForExpression)
               && Equals(SubjectExpression, other.SubjectExpression);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SubjectExpression.GetHashCode();
                hashCode = (hashCode * 397) ^ FromExpression.GetHashCode();
                hashCode = (hashCode * 397) ^ ForExpression.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"SUBSTRING({SubjectExpression} FROM {FromExpression} FOR {ForExpression})";

    }

}