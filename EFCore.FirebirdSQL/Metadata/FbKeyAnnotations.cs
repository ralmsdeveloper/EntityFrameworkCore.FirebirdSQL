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
 
namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class FbKeyAnnotations : RelationalKeyAnnotations, IFbKeyAnnotations
    {
        public FbKeyAnnotations(IKey key)
            : base(key)
        {
        }

        protected FbKeyAnnotations(RelationalAnnotations annotations)
            : base(annotations)
        {
        }
    }
}
