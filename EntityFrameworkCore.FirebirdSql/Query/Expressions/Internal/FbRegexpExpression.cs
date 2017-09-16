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

using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Expressions.Internal
{
    
    public class FbRegexpExpression : Expression
    {
        
        public FbRegexpExpression(Expression match, Expression pattern)
        {
            Match = match;
            Pattern = pattern;
        }

        public virtual Expression Match { get; }

        public virtual Expression Pattern { get; }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(bool);

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            var specificVisitor = visitor as IFbExpressionVisitor;

            return specificVisitor != null
                ? specificVisitor.VisitRegexp(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newMatchExpression = visitor.Visit(Match);
            var newPatternExpression = visitor.Visit(Pattern);

            return newMatchExpression != Match
                   || newPatternExpression != Pattern
                ? new FbRegexpExpression(newMatchExpression, newPatternExpression)
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

            return obj.GetType() == GetType() && Equals((FbRegexpExpression)obj);
        }

        private bool Equals(FbRegexpExpression other)
            => Equals(Match, other.Match)
               && Equals(Pattern, other.Pattern);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Match.GetHashCode();
                hashCode = (hashCode * 397) ^ Pattern.GetHashCode();

                return hashCode;
            }
        }

        public override string ToString() => $"{Match} SIMILAR TO {Pattern}";
        
    }
    
}
