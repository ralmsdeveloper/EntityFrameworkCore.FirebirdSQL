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

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EntityFrameworkCore.FirebirdSql.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class FirebirdLoggerExtensions
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void SchemaConfiguredWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Model.Validation> diagnostics,
            IEntityType entityType,
            string schema)
        {
            var definition = FbStrings.LogSchemaConfigured;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    entityType.DisplayName(), schema);
            }
        }

        private static string SchemaConfiguredWarning(EventDefinitionBase definition, EventData payload)
        {
            var d = (EventDefinition<string, string>)definition;
            var p = (EntityTypeSchemaEventData)payload;
            return d.GenerateMessage(
                p.EntityType.DisplayName(),
                p.Schema);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void SequenceConfiguredWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Model.Validation> diagnostics,
            ISequence sequence)
        {
            var definition = FbStrings.LogSequenceConfigured;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    sequence.Name);
            }
        }

        private static string SequenceConfiguredWarning(EventDefinitionBase definition, EventData payload)
        {
            var d = (EventDefinition<string>)definition;
            var p = (SequenceEventData)payload;
            return d.GenerateMessage(p.Sequence.Name);
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void ColumnFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string tableName,
            string columnName,
            string dataTypeName,
            bool notNull,
            string defaultValue)
        {
            var definition = FbStrings.LogFoundColumn;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    tableName, columnName, dataTypeName, notNull, defaultValue);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void SchemasNotSupportedWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics)
        {
            var definition = FbStrings.LogUsingSchemaSelectionsWarning;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(diagnostics, warningBehavior);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void ForeignKeyReferencesMissingTableWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string foreignKeyName)
        {
            var definition = FbStrings.LogForeignKeyScaffoldErrorPrincipalTableNotFound;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    foreignKeyName);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void TableFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string tableName)
        {
            var definition = FbStrings.LogFoundTable;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    tableName);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void MissingTableWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string tableName)
        {
            var definition = FbStrings.LogMissingTable;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    tableName);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void ForeignKeyPrincipalColumnMissingWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string foreignKeyName,
            string tableName,
            string principalColumnName,
            string principalTableName)
        {
            var definition = FbStrings.LogPrincipalColumnNotFound;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    foreignKeyName, tableName, principalColumnName, principalTableName);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void IndexFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string indexName,
            string tableName,
            bool? unique)
        {
            var definition = FbStrings.LogFoundIndex;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    indexName, tableName, unique);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void ForeignKeyFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string tableName,
            string id,
            string principalTableName,
            string deleteAction)
        {
            var definition = FbStrings.LogFoundForeignKey;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    tableName, id, principalTableName, deleteAction);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void PrimaryKeyFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string primaryKeyName,
            string tableName)
        {
            var definition = FbStrings.LogFoundPrimaryKey;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    primaryKeyName, tableName);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static void UniqueConstraintFound(
            this IDiagnosticsLogger<DbLoggerCategory.Scaffolding> diagnostics,
            string uniqueConstraintName,
            string tableName)
        {
            var definition = FbStrings.LogFoundUniqueConstraint;

            var warningBehavior = definition.GetLogBehavior(diagnostics);
            if (warningBehavior != WarningBehavior.Ignore)
            {
                definition.Log(
                    diagnostics,
                    warningBehavior,
                    uniqueConstraintName, tableName);
            }
            // No DiagnosticsSource events because these are purely design-time messages
        }
    }
}
