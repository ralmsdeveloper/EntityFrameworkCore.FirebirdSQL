/*
 *          Copyright (c) 2017-2018 Rafael Almeida (ralms@ralms.net)
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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal
{
    public class WithLockExpressionNode : ResultOperatorExpressionNodeBase
    {
        public static readonly IReadOnlyCollection<MethodInfo> SupportedMethods = new[]
            { FbQueryableExtensions.WithLockMethodInfo };

        public WithLockExpressionNode(
            MethodCallExpressionParseInfo parseInfo,
            ConstantExpression WithLockExpressionExpression)
            : base(parseInfo, null, null)
        {
        }

        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
            => new WithLockResultOperator();

        public override Expression Resolve(
            ParameterExpression inputParameter,
            Expression expressionToBeResolved,
            ClauseGenerationContext clauseGenerationContext)
            => Source.Resolve(inputParameter, expressionToBeResolved, clauseGenerationContext);
    }
}
