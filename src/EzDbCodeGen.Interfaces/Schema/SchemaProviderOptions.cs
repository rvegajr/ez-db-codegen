using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines options for loading database schemas.
    /// </summary>
    public class SchemaProviderOptions
    {
        /// <summary>
        /// Gets or sets a collection of schema names to include (if empty, all schemas are included).
        /// </summary>
        public ICollection<string> IncludeSchemas { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of schema names to exclude.
        /// </summary>
        public ICollection<string> ExcludeSchemas { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of table names to include (if empty, all tables are included).
        /// </summary>
        public ICollection<string> IncludeTables { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of table names to exclude.
        /// </summary>
        public ICollection<string> ExcludeTables { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of table patterns to include (using wildcard matching).
        /// </summary>
        public ICollection<string> IncludeTablePatterns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of table patterns to exclude (using wildcard matching).
        /// </summary>
        public ICollection<string> ExcludeTablePatterns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to include views.
        /// </summary>
        public bool IncludeViews { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include stored procedures.
        /// </summary>
        public bool IncludeStoredProcedures { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include functions.
        /// </summary>
        public bool IncludeFunctions { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include system tables.
        /// </summary>
        public bool IncludeSystemTables { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to include table columns.
        /// </summary>
        public bool IncludeColumns { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include primary keys.
        /// </summary>
        public bool IncludePrimaryKeys { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include foreign keys.
        /// </summary>
        public bool IncludeForeignKeys { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include indexes.
        /// </summary>
        public bool IncludeIndexes { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include unique constraints.
        /// </summary>
        public bool IncludeUniqueConstraints { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include descriptions.
        /// </summary>
        public bool IncludeDescriptions { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of tables to load.
        /// </summary>
        public int? MaxTables { get; set; } = null;

        /// <summary>
        /// Gets or sets the timeout in seconds for loading the schema.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 60;

        /// <summary>
        /// Gets or sets a value indicating whether to use case-sensitive name matching.
        /// </summary>
        public bool CaseSensitiveMatching { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to perform automatic relationship detection.
        /// </summary>
        public bool DetectRelationships { get; set; } = true;

        /// <summary>
        /// Gets or sets the options for relationship detection.
        /// </summary>
        public RelationshipDetectionOptions RelationshipDetectionOptions { get; set; } = RelationshipDetectionOptions.Default();

        /// <summary>
        /// Creates a new instance of SchemaProviderOptions with default settings.
        /// </summary>
        /// <returns>A SchemaProviderOptions instance with default settings.</returns>
        public static SchemaProviderOptions Default() => new SchemaProviderOptions();

        /// <summary>
        /// Creates a new instance of SchemaProviderOptions with minimal settings (tables only).
        /// </summary>
        /// <returns>A SchemaProviderOptions instance with minimal settings.</returns>
        public static SchemaProviderOptions Minimal() => new SchemaProviderOptions
        {
            IncludeViews = false,
            IncludeStoredProcedures = false,
            IncludeFunctions = false,
            IncludeIndexes = false,
            IncludeUniqueConstraints = false,
            IncludeDescriptions = false,
            DetectRelationships = false
        };

        /// <summary>
        /// Creates a new instance of SchemaProviderOptions with settings optimized for Entity Framework Core.
        /// </summary>
        /// <returns>A SchemaProviderOptions instance optimized for Entity Framework Core.</returns>
        public static SchemaProviderOptions ForEFCore() => new SchemaProviderOptions
        {
            IncludeViews = true,
            IncludeStoredProcedures = false,
            IncludeFunctions = false,
            IncludeIndexes = true,
            IncludeUniqueConstraints = true,
            IncludeDescriptions = true,
            DetectRelationships = true,
            RelationshipDetectionOptions = RelationshipDetectionOptions.AccuracyOptimized()
        };
    }
}
