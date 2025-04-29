namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Options for schema extraction.
/// </summary>
public class SchemaProviderOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to include system objects.
    /// </summary>
    public bool IncludeSystemObjects { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the schemas to include. If null or empty, all schemas are included.
    /// </summary>
    public ICollection<string>? IncludeSchemas { get; set; }
    
    /// <summary>
    /// Gets or sets the schemas to exclude.
    /// </summary>
    public ICollection<string>? ExcludeSchemas { get; set; }
    
    /// <summary>
    /// Gets or sets the tables to include. If null or empty, all tables are included.
    /// </summary>
    public ICollection<string>? IncludeTables { get; set; }
    
    /// <summary>
    /// Gets or sets the tables to exclude.
    /// </summary>
    public ICollection<string>? ExcludeTables { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to include views.
    /// </summary>
    public bool IncludeViews { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include stored procedures.
    /// </summary>
    public bool IncludeStoredProcedures { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include functions.
    /// </summary>
    public bool IncludeFunctions { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include table columns.
    /// </summary>
    public bool IncludeTableColumns { get; set; } = true;
    
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
    /// Gets or sets the command timeout in seconds.
    /// </summary>
    public int CommandTimeout { get; set; } = 30;
    
    /// <summary>
    /// Creates a new instance of the <see cref="SchemaProviderOptions"/> class with default values.
    /// </summary>
    /// <returns>A new instance of the <see cref="SchemaProviderOptions"/> class with default values.</returns>
    public static SchemaProviderOptions Default => new SchemaProviderOptions();
}
