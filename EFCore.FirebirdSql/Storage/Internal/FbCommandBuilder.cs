using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal; 

// ReSharper disable once CheckNamespace
namespace EntityFrameworkCore.FirebirdSql.Storage.Internal
{ 
    public class FbCommandBuilder : IRelationalCommandBuilder
    {
        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Command> _logger; 
        private readonly IndentedStringBuilder _commandTextBuilder = new IndentedStringBuilder();

        public FbCommandBuilder(IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, IRelationalTypeMapper typeMapper)
        {
            _logger = logger;
            ParameterBuilder = new RelationalParameterBuilder(typeMapper);
        }

        IndentedStringBuilder IInfrastructure<IndentedStringBuilder>.Instance
            => _commandTextBuilder;

        public virtual IRelationalParameterBuilder ParameterBuilder { get; }

        public virtual IRelationalCommand Build()
	    {
		    return BuildCore(_logger, _commandTextBuilder.ToString(), ParameterBuilder.Parameters);
	    }

		protected virtual IRelationalCommand BuildCore(IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger, string commandText, IReadOnlyList<IRelationalParameter> parameters)
		{
			return new FirebirdRelationalCommand(logger, commandText, parameters);
		}

		public override string ToString()
	    {
		    return _commandTextBuilder.ToString();
	    }
    }
}