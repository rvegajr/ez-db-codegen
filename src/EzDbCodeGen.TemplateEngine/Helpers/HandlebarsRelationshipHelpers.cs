using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EzDbCodeGen.Core.Interfaces;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.TemplateEngine.Extensions;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;
using HandlebarsDotNet;
using HandlebarsDotNet.IO;

#nullable enable

namespace EzDbCodeGen.TemplateEngine.Helpers
{
    /// <summary>
    /// Provides relationship-related helpers for Handlebars templates.
    /// </summary>
    public class HandlebarsRelationshipHelpers : IRelationshipHelpers, IHelperRegistration
    {
        private readonly IEnumerable<ITable> _tables;
        private readonly IEnumerable<IForeignKey> _foreignKeys;
        private readonly IDatabaseSchema _schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlebarsRelationshipHelpers"/> class.
        /// </summary>
        /// <param name="tables">Database tables.</param>
        /// <param name="foreignKeys">Database foreign keys.</param>
        /// <param name="schema">Database schema.</param>
        public HandlebarsRelationshipHelpers(IEnumerable<ITable> tables, IEnumerable<IForeignKey> foreignKeys,
            IDatabaseSchema schema)
        {
            _tables = tables ?? Enumerable.Empty<ITable>();
            _foreignKeys = foreignKeys ?? Enumerable.Empty<IForeignKey>();
            _schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        /// <inheritdoc/>
        public void RegisterHelpers(ITemplateEngine templateEngine)
        {
            // Register foreign key helpers
            templateEngine.RegisterHelper("foreignKeys",
                (EncodedTextWriter writer, Context context, Arguments arguments) =>
                {
                    if (arguments.Length != 2)
                    {
                        throw new HandlebarsException(
                            "{{foreignKeys}} helper requires exactly two arguments: schema and table");
                    }

                    var schemaName = arguments[0]?.ToString()?.Trim() ?? string.Empty;
                    var tableName = arguments[1]?.ToString()?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(tableName))
                    {
                        throw new HandlebarsException("Schema and table names cannot be null or empty");
                    }

                    var table = _tables.FirstOrDefault(t =>
                        string.Equals(t.Schema, schemaName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));

                    if (table == null)
                    {
                        throw new HandlebarsException($"Table '{schemaName}.{tableName}' not found");
                    }

                    var foreignKeys = _foreignKeys.Where(fk => fk.Table == table);
                    writer.WriteSafeString(string.Join(", ", foreignKeys.Select(fk => fk.Name)));
                });

            // Register has foreign keys helper
            templateEngine.RegisterHelper("hasForeignKeys",
                (EncodedTextWriter writer, Context context, Arguments arguments) =>
                {
                    if (arguments.Length != 2)
                    {
                        throw new HandlebarsException(
                            "{{hasForeignKeys}} helper requires exactly two arguments: schema and table");
                    }

                    var schemaName = arguments[0]?.ToString()?.Trim() ?? string.Empty;
                    var tableName = arguments[1]?.ToString()?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(tableName))
                    {
                        throw new HandlebarsException("Schema and table names cannot be null or empty");
                    }

                    var table = _tables.FirstOrDefault(t =>
                        string.Equals(t.Schema, schemaName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));

                    if (table == null)
                    {
                        throw new HandlebarsException($"Table '{schemaName}.{tableName}' not found");
                    }

                    var hasForeignKeys = _foreignKeys.Any(fk => fk.Table == table);
                    writer.WriteSafeString(hasForeignKeys.ToString().ToLower());
                });

            // Register is foreign key helper
            templateEngine.RegisterHelper("isForeignKey",
                (EncodedTextWriter writer, Context context, Arguments arguments) =>
                {
                    if (arguments.Length != 3)
                    {
                        throw new HandlebarsException(
                            "{{isForeignKey}} helper requires exactly three arguments: schema, table, and column");
                    }

                    var schemaName = arguments[0]?.ToString()?.Trim() ?? string.Empty;
                    var tableName = arguments[1]?.ToString()?.Trim() ?? string.Empty;
                    var columnName = arguments[2]?.ToString()?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(tableName) ||
                        string.IsNullOrEmpty(columnName))
                    {
                        throw new HandlebarsException("Schema, table, and column names cannot be null or empty");
                    }

                    var table = _tables.FirstOrDefault(t =>
                        string.Equals(t.Schema, schemaName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));

                    if (table == null)
                    {
                        throw new HandlebarsException($"Table '{schemaName}.{tableName}' not found");
                    }

                    var column = table.Columns?.FirstOrDefault(c =>
                        string.Equals(c.Name, columnName, StringComparison.OrdinalIgnoreCase));
                    if (column == null)
                    {
                        throw new HandlebarsException(
                            $"Column '{columnName}' not found in table '{schemaName}.{tableName}'");
                    }

                    var isForeignKey = _foreignKeys.Any(fk =>
                        fk.Table == table &&
                        fk.Columns.Any(c =>
                            string.Equals(c.Name, columnName, StringComparison.OrdinalIgnoreCase)));

                    writer.WriteSafeString(isForeignKey.ToString().ToLower());
                });

            // Register is unique foreign key helper
            templateEngine.RegisterHelper("isUniqueForeignKey",
                (EncodedTextWriter writer, Context context, Arguments arguments) =>
                {
                    if (arguments.Length != 3)
                    {
                        throw new HandlebarsException(
                            "{{isUniqueForeignKey}} helper requires exactly three arguments: schema, table, and column");
                    }

                    var schemaName = arguments[0]?.ToString()?.Trim() ?? string.Empty;
                    var tableName = arguments[1]?.ToString()?.Trim() ?? string.Empty;
                    var columnName = arguments[2]?.ToString()?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(tableName) ||
                        string.IsNullOrEmpty(columnName))
                    {
                        throw new HandlebarsException("Schema, table, and column names cannot be null or empty");
                    }

                    var table = _tables.FirstOrDefault(t =>
                        string.Equals(t.Schema, schemaName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));

                    if (table == null)
                    {
                        throw new HandlebarsException($"Table '{schemaName}.{tableName}' not found");
                    }

                    var column = table.Columns?.FirstOrDefault(c =>
                        string.Equals(c.Name, columnName, StringComparison.OrdinalIgnoreCase));
                    if (column == null)
                    {
                        throw new HandlebarsException(
                            $"Column '{columnName}' not found in table '{schemaName}.{tableName}'");
                    }

                    var foreignKey = _foreignKeys.FirstOrDefault(fk =>
                        fk.Table == table &&
                        fk.Columns.Any(c =>
                            string.Equals(c.Name, columnName, StringComparison.OrdinalIgnoreCase)));

                    var isUnique = foreignKey != null && foreignKey.IsUnique();
                    writer.WriteSafeString(isUnique.ToString().ToLower());
                });

            // Register referenced tables helper
            templateEngine.RegisterHelper("referencedTables",
                (EncodedTextWriter writer, Context context, Arguments arguments) =>
                {
                    if (arguments.Length != 2)
                    {
                        throw new HandlebarsException(
                            "{{referencedTables}} helper requires exactly two arguments: schema and table");
                    }

                    var schemaName = arguments[0]?.ToString()?.Trim() ?? string.Empty;
                    var tableName = arguments[1]?.ToString()?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(tableName))
                    {
                        throw new HandlebarsException("Schema and table names cannot be null or empty");
                    }

                    var table = _tables.FirstOrDefault(t =>
                        string.Equals(t.Schema, schemaName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));

                    if (table == null)
                    {
                        throw new HandlebarsException($"Table '{schemaName}.{tableName}' not found");
                    }

                    var referencedTables = _foreignKeys
                        .Where(fk => fk.Table == table)
                        .Select(fk => fk.ReferencedTable)
                        .Where(t => t != null)
                        .Select(t => t.Name)
                        .Distinct();

                    writer.WriteSafeString(string.Join(", ", referencedTables));
                });

            // Register referencing tables helper
            templateEngine.RegisterHelper("referencingTables",
                (EncodedTextWriter writer, Context context, Arguments arguments) =>
                {
                    if (arguments.Length != 2)
                    {
                        throw new HandlebarsException(
                            "{{referencingTables}} helper requires exactly two arguments: schema and table");
                    }

                    var schemaName = arguments[0]?.ToString()?.Trim() ?? string.Empty;
                    var tableName = arguments[1]?.ToString()?.Trim() ?? string.Empty;

                    if (string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(tableName))
                    {
                        throw new HandlebarsException("Schema and table names cannot be null or empty");
                    }

                    var table = _tables.FirstOrDefault(t =>
                        string.Equals(t.Schema, schemaName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));

                    if (table == null)
                    {
                        throw new HandlebarsException($"Table '{schemaName}.{tableName}' not found");
                    }

                    var referencingTables = _foreignKeys
                        .Where(fk => fk.ReferencedTable == table)
                        .Select(fk => fk.Table)
                        .Where(t => t != null)
                        .Select(t => t.Name)
                        .Distinct();

                    writer.WriteSafeString(string.Join(", ", referencingTables));
                });
        }
    }
}