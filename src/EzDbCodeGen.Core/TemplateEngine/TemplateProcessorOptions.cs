namespace EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents options for configuring a template processor.
/// </summary>
public class TemplateProcessorOptions
{
    /// <summary>
    /// Gets or sets the base path for resolving templates.
    /// </summary>
    public string? TemplatePath { get; set; }
    
    /// <summary>
    /// Gets or sets the base path for resolving partials.
    /// </summary>
    public string? PartialsPath { get; set; }
    
    /// <summary>
    /// Gets or sets the base path for resolving layouts.
    /// </summary>
    public string? LayoutsPath { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to use layouts.
    /// </summary>
    public bool UseLayouts { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the name of the body placeholder in layouts.
    /// </summary>
    public string BodyPlaceholder { get; set; } = "{{body}}";
    
    /// <summary>
    /// Gets or sets the default file extension for templates.
    /// </summary>
    public string DefaultTemplateExtension { get; set; } = ".hbs";
    
    /// <summary>
    /// Gets or sets a value indicating whether to use the file system.
    /// </summary>
    public bool UseFileSystem { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to enable template caching.
    /// </summary>
    public bool EnableCaching { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to preserve case in output paths.
    /// </summary>
    public bool PreserveCaseInOutputPaths { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to throw on missing templates.
    /// </summary>
    public bool ThrowOnMissingTemplate { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to process per entity.
    /// </summary>
    public bool ProcessPerEntity { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include schema in entity file names.
    /// </summary>
    public bool IncludeSchemaInEntityFileNames { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the entity file name format.
    /// </summary>
    public string EntityFileNameFormat { get; set; } = "{0}";
    
    /// <summary>
    /// Gets or sets a value indicating whether to enable incremental generation.
    /// </summary>
    public bool EnableIncrementalGeneration { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the snapshot directory for incremental generation.
    /// </summary>
    public string? SnapshotDirectory { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to use relative paths for output.
    /// </summary>
    public bool UseRelativePathsForOutput { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the output file handler.
    /// </summary>
    public Func<string, string, bool>? OutputFileHandler { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of output filters.
    /// </summary>
    public IList<IOutputFilter> OutputFilters { get; set; } = new List<IOutputFilter>();
    
    /// <summary>
    /// Gets or sets a collection of template variables.
    /// </summary>
    public IDictionary<string, object> TemplateVariables { get; set; } = new Dictionary<string, object>();
}
