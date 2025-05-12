using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Schema.Filters
{
    /// <summary>
    /// Defines options for schema filtering.
    /// </summary>
    public class SchemaFilterOptions
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
        /// Gets or sets a collection of column patterns to include (using wildcard matching).
        /// </summary>
        public ICollection<string> IncludeColumnPatterns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of column patterns to exclude (using wildcard matching).
        /// </summary>
        public ICollection<string> ExcludeColumnPatterns { get; set; } = new List<string>();

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
        /// Gets or sets a value indicating whether to use case-sensitive name matching.
        /// </summary>
        public bool CaseSensitiveMatching { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to filter by table size (rows).
        /// </summary>
        public bool FilterByTableSize { get; set; } = false;

        /// <summary>
        /// Gets or sets the minimum table size (rows) to include.
        /// </summary>
        public int? MinTableSize { get; set; } = null;

        /// <summary>
        /// Gets or sets the maximum table size (rows) to include.
        /// </summary>
        public int? MaxTableSize { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether to filter related entities recursively.
        /// For example, if including "Orders" also include "Customers" that are related.
        /// </summary>
        public bool IncludeRelatedEntities { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum recursion depth for including related entities.
        /// </summary>
        public int? MaxRelatedDepth { get; set; } = 2;

        /// <summary>
        /// Creates a new instance of SchemaFilterOptions with default settings.
        /// </summary>
        /// <returns>A SchemaFilterOptions instance with default settings.</returns>
        public static SchemaFilterOptions Default() => new SchemaFilterOptions();
        
        /// <summary>
        /// Creates a new instance of SchemaFilterOptions with minimal settings (core tables only).
        /// </summary>
        /// <returns>A SchemaFilterOptions instance with minimal settings.</returns>
        public static SchemaFilterOptions CoreTablesOnly() => new SchemaFilterOptions
        {
            IncludeViews = false,
            IncludeStoredProcedures = false,
            IncludeFunctions = false,
            IncludeSystemTables = false,
            ExcludeTablePatterns = new List<string> { "Temp*", "Log*", "Audit*", "*Backup*" }
        };
    }
}
