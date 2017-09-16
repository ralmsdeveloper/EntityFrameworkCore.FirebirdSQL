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

namespace Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class FbCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
	    private static readonly IMethodCallTranslator[] _methodCallTranslators =
	    {
		    new FbContainsOptimizedTranslator(),
		    new FbConvertTranslator(),
		    new FbDateAddTranslator(),
		    new FbEndsWithOptimizedTranslator(),
		    new FbMathTranslator(),
		    new FbNewGuidTranslator(),
		    new FbObjectToStringTranslator(),
		    new FbRegexIsMatchTranslator(),
		    new FbStartsWithOptimizedTranslator(),
		    new FbStringIsNullOrWhiteSpaceTranslator(),
		    new FbStringReplaceTranslator(),
		    new FbStringSubstringTranslator(),
		    new FbStringToLowerTranslator(),
		    new FbStringToUpperTranslator(),
		    new FbStringTrimTranslator()
	    };

	    /// <summary>
	    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
	    ///     directly from your code. This API may change or be removed in future releases.
	    /// </summary>
	    public FbCompositeMethodCallTranslator(
		    RelationalCompositeMethodCallTranslatorDependencies dependencies)
		    : base(dependencies)
	    {
		    // ReSharper disable once DoNotCallOverridableMethodsInConstructor
		    AddTranslators(_methodCallTranslators);
	    }
    }
}
