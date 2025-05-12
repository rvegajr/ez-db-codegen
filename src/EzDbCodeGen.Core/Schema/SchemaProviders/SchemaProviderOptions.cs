using System.Collections.Generic;

namespace EzDbCodeGen.Core.Schema.SchemaProviders
{
    /// <summary>
    /// Provides options for loading database schemas.
    /// </summary>
    public class SchemaProviderOptions
    {
        /// <summary>
        /// Gets or sets the schema names to include.
        /// </summary>
        public List<string> IncludeSchemas { get; set; } = new List<string> { "dbo" };

        /// <summary>
        /// Gets or sets the table names to include.
        /// </summary>
        public List<string> IncludeTables { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the table names to exclude.
        /// </summary>
        public List<string> ExcludeTables { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to include views.
        /// </summary>
        public bool IncludeViewsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the view names to include.
        /// </summary>
        public List<string> IncludeViews { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the view names to exclude.
        /// </summary>
        public List<string> ExcludeViews { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to include stored procedures.
        /// </summary>
        public bool IncludeStoredProcedures { get; set; } = true;

        /// <summary>
        /// Gets or sets the stored procedure names to include.
        /// </summary>
        public List<string> IncludeStoredProcedures { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the stored procedure names to exclude.
        /// </summary>
        public List<string> ExcludeStoredProcedures { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to include functions.
        /// </summary>
        public bool IncludeFunctions { get; set; } = true;

        /// <summary>
        /// Gets or sets the function names to include.
        /// </summary>
        public List<string> IncludeFunctions { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the function names to exclude.
        /// </summary>
        public List<string> ExcludeFunctions { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to include table triggers.
        /// </summary>
        public bool IncludeTableTriggers { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to include table indexes.
        /// </summary>
        public bool IncludeTableIndexes { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect relationships.
        /// </summary>
        public bool DetectRelationships { get; set; } = true;

        /// <summary>
        /// Gets or sets the relationship detection options.
        /// </summary>
        public RelationshipDetectionOptions RelationshipOptions { get; set; } = new RelationshipDetectionOptions();

        /// <summary>
        /// Gets or sets a value indicating whether to use case-sensitive object names.
        /// </summary>
        public bool UseCaseSensitiveObjectNames { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to include extended properties.
        /// </summary>
        public bool IncludeExtendedProperties { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include database statistics.
        /// </summary>
        public bool IncludeDatabaseStatistics { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating the maximum number of tables to load.
        /// </summary>
        public int? MaxTables { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating the maximum number of views to load.
        /// </summary>
        public int? MaxViews { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating the maximum number of stored procedures to load.
        /// </summary>
        public int? MaxStoredProcedures { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating the maximum number of functions to load.
        /// </summary>
        public int? MaxFunctions { get; set; } = null;
        
        /// <summary>
        /// Gets or sets a wildcard pattern for including tables.
        /// </summary>
        public string TableIncludePattern { get; set; } = "*";
        
        /// <summary>
        /// Gets or sets a wildcard pattern for excluding tables.
        /// </summary>
        public string TableExcludePattern { get; set; } = "";
    }
}
