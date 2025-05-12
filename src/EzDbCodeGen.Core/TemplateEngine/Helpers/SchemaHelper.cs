using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;
using EzDbCodeGen.Interfaces.TemplateEngine.Helpers;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Core.TemplateEngine.Helpers
{
    /// <summary>
    /// Implementation of the ISchemaHelper interface for working with database schemas in templates.
    /// </summary>
    public class SchemaHelper : ISchemaHelper
    {
        private readonly ILogger<SchemaHelper> _logger;
        private readonly IDictionary<string, IDictionary<string, string>> _typeMapping;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaHelper"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public SchemaHelper(ILogger<SchemaHelper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Initialize type mappings for SQL Server to various languages
            // This would typically be loaded from configuration or a more sophisticated type mapping system
            _typeMapping = new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "csharp", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "bit", "bool" },
                        { "tinyint", "byte" },
                        { "smallint", "short" },
                        { "int", "int" },
                        { "bigint", "long" },
                        { "decimal", "decimal" },
                        { "numeric", "decimal" },
                        { "smallmoney", "decimal" },
                        { "money", "decimal" },
                        { "float", "double" },
                        { "real", "float" },
                        { "datetime", "DateTime" },
                        { "datetime2", "DateTime" },
                        { "smalldatetime", "DateTime" },
                        { "date", "DateTime" },
                        { "time", "TimeSpan" },
                        { "datetimeoffset", "DateTimeOffset" },
                        { "char", "string" },
                        { "varchar", "string" },
                        { "nchar", "string" },
                        { "nvarchar", "string" },
                        { "text", "string" },
                        { "ntext", "string" },
                        { "binary", "byte[]" },
                        { "varbinary", "byte[]" },
                        { "image", "byte[]" },
                        { "rowversion", "byte[]" },
                        { "timestamp", "byte[]" },
                        { "uniqueidentifier", "Guid" },
                        { "xml", "string" },
                        { "hierarchyid", "string" },
                        { "sql_variant", "object" },
                        { "geometry", "System.Data.Spatial.DbGeometry" },
                        { "geography", "System.Data.Spatial.DbGeography" }
                    }
                },
                {
                    "typescript", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "bit", "boolean" },
                        { "tinyint", "number" },
                        { "smallint", "number" },
                        { "int", "number" },
                        { "bigint", "number" },
                        { "decimal", "number" },
                        { "numeric", "number" },
                        { "smallmoney", "number" },
                        { "money", "number" },
                        { "float", "number" },
                        { "real", "number" },
                        { "datetime", "Date" },
                        { "datetime2", "Date" },
                        { "smalldatetime", "Date" },
                        { "date", "Date" },
                        { "time", "string" },
                        { "datetimeoffset", "Date" },
                        { "char", "string" },
                        { "varchar", "string" },
                        { "nchar", "string" },
                        { "nvarchar", "string" },
                        { "text", "string" },
                        { "ntext", "string" },
                        { "binary", "ArrayBuffer" },
                        { "varbinary", "ArrayBuffer" },
                        { "image", "ArrayBuffer" },
                        { "rowversion", "ArrayBuffer" },
                        { "timestamp", "ArrayBuffer" },
                        { "uniqueidentifier", "string" },
                        { "xml", "string" },
                        { "hierarchyid", "string" },
                        { "sql_variant", "any" },
                        { "geometry", "any" },
                        { "geography", "any" }
                    }
                },
                {
                    "java", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "bit", "boolean" },
                        { "tinyint", "byte" },
                        { "smallint", "short" },
                        { "int", "int" },
                        { "bigint", "long" },
                        { "decimal", "java.math.BigDecimal" },
                        { "numeric", "java.math.BigDecimal" },
                        { "smallmoney", "java.math.BigDecimal" },
                        { "money", "java.math.BigDecimal" },
                        { "float", "double" },
                        { "real", "float" },
                        { "datetime", "java.util.Date" },
                        { "datetime2", "java.time.LocalDateTime" },
                        { "smalldatetime", "java.util.Date" },
                        { "date", "java.time.LocalDate" },
                        { "time", "java.time.LocalTime" },
                        { "datetimeoffset", "java.time.OffsetDateTime" },
                        { "char", "String" },
                        { "varchar", "String" },
                        { "nchar", "String" },
                        { "nvarchar", "String" },
                        { "text", "String" },
                        { "ntext", "String" },
                        { "binary", "byte[]" },
                        { "varbinary", "byte[]" },
                        { "image", "byte[]" },
                        { "rowversion", "byte[]" },
                        { "timestamp", "byte[]" },
                        { "uniqueidentifier", "java.util.UUID" },
                        { "xml", "String" },
                        { "hierarchyid", "String" },
                        { "sql_variant", "Object" },
                        { "geometry", "Object" },
                        { "geography", "Object" }
                    }
                },
                {
                    "python", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "bit", "bool" },
                        { "tinyint", "int" },
                        { "smallint", "int" },
                        { "int", "int" },
                        { "bigint", "int" },
                        { "decimal", "Decimal" },
                        { "numeric", "Decimal" },
                        { "smallmoney", "Decimal" },
                        { "money", "Decimal" },
                        { "float", "float" },
                        { "real", "float" },
                        { "datetime", "datetime" },
                        { "datetime2", "datetime" },
                        { "smalldatetime", "datetime" },
                        { "date", "date" },
                        { "time", "time" },
                        { "datetimeoffset", "datetime" },
                        { "char", "str" },
                        { "varchar", "str" },
                        { "nchar", "str" },
                        { "nvarchar", "str" },
                        { "text", "str" },
                        { "ntext", "str" },
                        { "binary", "bytes" },
                        { "varbinary", "bytes" },
                        { "image", "bytes" },
                        { "rowversion", "bytes" },
                        { "timestamp", "bytes" },
                        { "uniqueidentifier", "UUID" },
                        { "xml", "str" },
                        { "hierarchyid", "str" },
                        { "sql_variant", "Any" },
                        { "geometry", "Any" },
                        { "geography", "Any" }
                    }
                }
            };
        }

        /// <inheritdoc/>
        public IEnumerable<IColumn> GetPrimaryKeyColumns(object tableObj)
        {
            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get primary key columns for an object that is not a table.");
                return Enumerable.Empty<IColumn>();
            }

            return table.PrimaryKeyColumns;
        }

        /// <inheritdoc/>
        public IEnumerable<IColumn> GetNonPrimaryKeyColumns(object tableObj)
        {
            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get non-primary key columns for an object that is not a table.");
                return Enumerable.Empty<IColumn>();
            }

            return table.Columns.Where(c => !c.IsPrimaryKey);
        }

        /// <inheritdoc/>
        public IEnumerable<IColumn> GetForeignKeyColumns(object tableObj)
        {
            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get foreign key columns for an object that is not a table.");
                return Enumerable.Empty<IColumn>();
            }

            // Get all columns that are part of any foreign key
            return table.ForeignKeys
                .SelectMany(fk => fk.SourceColumns)
                .Distinct();
        }

        /// <inheritdoc/>
        public IEnumerable<IColumn> GetNonForeignKeyColumns(object tableObj)
        {
            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get non-foreign key columns for an object that is not a table.");
                return Enumerable.Empty<IColumn>();
            }

            // Get all columns that are not part of any foreign key
            var foreignKeyColumns = GetForeignKeyColumns(table).ToHashSet();
            return table.Columns.Where(c => !foreignKeyColumns.Contains(c));
        }

        /// <inheritdoc/>
        public string GetColumnDbType(object columnObj)
        {
            if (!(columnObj is IColumn column))
            {
                _logger.LogWarning("Attempted to get database type for an object that is not a column.");
                return string.Empty;
            }

            return column.DataType;
        }

        /// <inheritdoc/>
        public string GetColumnClrType(object columnObj, string language = "csharp")
        {
            if (!(columnObj is IColumn column))
            {
                _logger.LogWarning("Attempted to get CLR type for an object that is not a column.");
                return string.Empty;
            }

            // Normalize language name
            language = language.ToLowerInvariant();

            // Check if we have mappings for this language
            if (!_typeMapping.TryGetValue(language, out var languageMapping))
            {
                _logger.LogWarning($"No type mappings found for language '{language}'. Defaulting to C#.");
                languageMapping = _typeMapping["csharp"];
            }

            // Try to get the mapped type
            string dataType = column.DataType.ToLowerInvariant();
            if (!languageMapping.TryGetValue(dataType, out string clrType))
            {
                _logger.LogWarning($"No mapping found for data type '{dataType}' in language '{language}'. Using 'object'.");
                
                // Default to a generic type based on language
                switch (language)
                {
                    case "csharp":
                        clrType = "object";
                        break;
                    case "typescript":
                        clrType = "any";
                        break;
                    case "java":
                        clrType = "Object";
                        break;
                    case "python":
                        clrType = "Any";
                        break;
                    default:
                        clrType = "object";
                        break;
                }
            }

            // Handle nullable types
            if (column.IsNullable)
            {
                switch (language)
                {
                    case "csharp":
                        // Special handling for reference types in C#
                        if (clrType != "string" && clrType != "object" && !clrType.EndsWith("[]"))
                        {
                            clrType += "?";
                        }
                        break;
                    case "typescript":
                        clrType += " | null";
                        break;
                    case "java":
                        // In Java, primitives need to be boxed for null
                        switch (clrType)
                        {
                            case "boolean":
                                clrType = "Boolean";
                                break;
                            case "byte":
                                clrType = "Byte";
                                break;
                            case "short":
                                clrType = "Short";
                                break;
                            case "int":
                                clrType = "Integer";
                                break;
                            case "long":
                                clrType = "Long";
                                break;
                            case "float":
                                clrType = "Float";
                                break;
                            case "double":
                                clrType = "Double";
                                break;
                        }
                        break;
                    case "python":
                        clrType = $"Optional[{clrType}]";
                        break;
                }
            }

            return clrType;
        }

        /// <inheritdoc/>
        public object GetTableByName(object schemaObj, string tableName)
        {
            if (!(schemaObj is IDatabaseSchema schema))
            {
                _logger.LogWarning("Attempted to get a table from an object that is not a database schema.");
                return null;
            }

            // Parse the table name (may include schema)
            string schemaName = "dbo";
            string tableNameOnly = tableName;

            if (tableName.Contains("."))
            {
                var parts = tableName.Split('.');
                schemaName = parts[0];
                tableNameOnly = parts[1];
            }

            return schema.GetTable(schemaName, tableNameOnly);
        }

        /// <inheritdoc/>
        public object GetColumnByName(object tableObj, string columnName)
        {
            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get a column from an object that is not a table.");
                return null;
            }

            return table.GetColumn(columnName);
        }

        /// <inheritdoc/>
        public object GetReferencedTable(object foreignKeyObj)
        {
            if (!(foreignKeyObj is IForeignKey foreignKey))
            {
                _logger.LogWarning("Attempted to get the referenced table from an object that is not a foreign key.");
                return null;
            }

            return foreignKey.ReferencedTable;
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetIncomingForeignKeys(object tableObj)
        {
            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get incoming foreign keys for an object that is not a table.");
                return Enumerable.Empty<object>();
            }

            // We need the database schema to find all foreign keys
            if (!(table.Columns.FirstOrDefault()?.Table as ITable)?.Columns.FirstOrDefault()?.Table is IDatabaseSchema schema)
            {
                _logger.LogWarning("Cannot determine the database schema from the provided table.");
                return Enumerable.Empty<object>();
            }

            // Find all foreign keys in all tables that reference this table
            return schema.Tables
                .SelectMany(t => t.ForeignKeys)
                .Where(fk => fk.ReferencedTable == table)
                .Cast<object>();
        }
    }
}
