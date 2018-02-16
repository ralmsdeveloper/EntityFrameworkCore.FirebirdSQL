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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{
    public class FbCommandBuilderFactory : IRelationalCommandBuilderFactory
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Command> _logger;
        private readonly IRelationalCoreTypeMapper _typeMapper;

        public FbCommandBuilderFactory(IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, IRelationalCoreTypeMapper typeMapper)
        {
            _logger = logger;
            _typeMapper = typeMapper;
        }

        public IRelationalParameterBuilder ParameterBuilder => throw new System.NotImplementedException();

        public IndentedStringBuilder Instance => throw new System.NotImplementedException();

        public virtual IRelationalCommandBuilder Create() => CreateCore(_logger, _typeMapper);

        protected virtual IRelationalCommandBuilder CreateCore(
            IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger,
            IRelationalCoreTypeMapper relationalTypeMapper)
                => new FbCommandBuilder(logger, relationalTypeMapper);
    }
}
