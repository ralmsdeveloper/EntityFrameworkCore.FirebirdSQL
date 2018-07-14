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
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public static class ExtensionSql
    {
        private static readonly TypeInfo _queryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo _queryCompiler
            = typeof(EntityQueryProvider)
                .GetTypeInfo()
                .DeclaredFields
                .Single(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo _queryModelGenerator
            = _queryCompilerTypeInfo
                .DeclaredFields
                .Single(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo _database = _queryCompilerTypeInfo
            .DeclaredFields
            .Single(x => x.Name == "_database");

        private static readonly PropertyInfo _dependencies
            = typeof(Database)
                .GetTypeInfo()
                .DeclaredProperties
                .Single(x => x.Name == "Dependencies");

        public static string ToSql<T>(this IQueryable<T> queryable)
            where T : class
        {
            var queryCompiler = _queryCompiler.GetValue(queryable.Provider) as IQueryCompiler;
            var queryModel = (_queryModelGenerator.GetValue(queryCompiler) as IQueryModelGenerator).ParseQuery(queryable.Expression);
            var queryCompilationContextFactory
                = ((DatabaseDependencies)_dependencies.GetValue(_database.GetValue(queryCompiler)))
                    .QueryCompilationContextFactory;

            var queryCompilationContext = queryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();

            modelVisitor.CreateQueryExecutor<T>(queryModel);

            return modelVisitor
                .Queries
                .FirstOrDefault()
                .ToString();
        }
    }
}
