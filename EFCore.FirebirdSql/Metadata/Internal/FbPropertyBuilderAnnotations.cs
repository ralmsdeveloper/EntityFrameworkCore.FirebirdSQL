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

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EntityFrameworkCore.FirebirdSql.Metadata.Internal
{
    public class FbPropertyBuilderAnnotations : FbPropertyAnnotations
    {
        public FbPropertyBuilderAnnotations(InternalPropertyBuilder internalBuilder, ConfigurationSource configurationSource)
            : base(new RelationalAnnotationsBuilder(internalBuilder, configurationSource))
        { }

        public new virtual bool ColumnName(string value)
            => SetColumnName(value);

        public new virtual bool ColumnType(string value)
            => SetColumnType(value);

        public new virtual bool DefaultValueSql(string value)
            => SetDefaultValueSql(value);

        public new virtual bool ComputedColumnSql(string value)
            => SetComputedColumnSql(value);

        public new virtual bool DefaultValue(object value)
            => SetDefaultValue(value);

        public new virtual bool ValueGenerationStrategy(FbValueGenerationStrategy? value)
        {
            if (!SetValueGenerationStrategy(value))
            {
                return false;
            } 
            return true;
        }
    }
}
