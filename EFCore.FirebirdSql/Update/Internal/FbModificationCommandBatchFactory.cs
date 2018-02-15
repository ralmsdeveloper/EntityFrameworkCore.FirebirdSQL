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

using System.Linq;
using EntityFrameworkCore.FirebirdSql.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace EntityFrameworkCore.FirebirdSql.Update.Internal
{
    public class FbModificationCommandBatchFactory : IModificationCommandBatchFactory
    {
        private readonly IRelationalCommandBuilderFactory _commandBuilderFactory;
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        private readonly IFbUpdateSqlGenerator _updateSqlGenerator;
        private readonly IRelationalValueBufferFactoryFactory _valueBufferFactoryFactory;
        private readonly IDbContextOptions _options;

        public FbModificationCommandBatchFactory(IRelationalCommandBuilderFactory commandBuilderFactory, ISqlGenerationHelper sqlGenerationHelper, IFbUpdateSqlGenerator updateSqlGenerator, IRelationalValueBufferFactoryFactory valueBufferFactoryFactory, IDbContextOptions options)
        {
            _commandBuilderFactory = commandBuilderFactory;
            _sqlGenerationHelper = sqlGenerationHelper;
            _updateSqlGenerator = updateSqlGenerator;
            _valueBufferFactoryFactory = valueBufferFactoryFactory;
            _options = options;
        }

        public virtual ModificationCommandBatch Create()
        {
            var optionsExtension = _options.Extensions.OfType<FbOptionsExtension>().FirstOrDefault();
            return
                new FbModificationCommandBatch(
                    _commandBuilderFactory,
                    _sqlGenerationHelper,
                    _updateSqlGenerator,
                    _valueBufferFactoryFactory,
                    optionsExtension?.MaxBatchSize);
        }
    }
}
