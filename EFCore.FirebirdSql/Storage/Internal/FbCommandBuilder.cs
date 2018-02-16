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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{
    public class FbCommandBuilder : IRelationalCommandBuilder
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Command> _logger;
        private readonly IndentedStringBuilder _commandTextBuilder = new IndentedStringBuilder();

        public FbCommandBuilder(IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, IRelationalCoreTypeMapper typeMapper)
        {
            _logger = logger;
            ParameterBuilder = new RelationalParameterBuilder(typeMapper);
        }

        IndentedStringBuilder IInfrastructure<IndentedStringBuilder>.Instance
            => _commandTextBuilder;

        public virtual IRelationalParameterBuilder ParameterBuilder { get; }

        public virtual IRelationalCommand Build()
            => BuildCore(_logger, _commandTextBuilder.ToString(), ParameterBuilder.Parameters);

        protected virtual IRelationalCommand BuildCore(IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, string commandText, IReadOnlyList<IRelationalParameter> parameters)
             => new FirebirdRelationalCommand(logger, commandText, parameters);

        public override string ToString() => _commandTextBuilder.ToString();

    }
}
