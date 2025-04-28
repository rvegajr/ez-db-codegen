namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Represents options for configuring a template engine.
/// </summary>
public class TemplateEngineOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to cache compiled templates.
    /// </summary>
    public bool CacheTemplates { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to enable debugging features.
    /// </summary>
    public bool EnableDebugging { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to throw exceptions on errors or handle them silently.
    /// </summary>
    public bool ThrowOnError { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the path to search for partial templates.
    /// </summary>
    public string? PartialsPath { get; set; }
    
    /// <summary>
    /// Gets or sets the path to search for layout templates.
    /// </summary>
    public string? LayoutsPath { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to include line numbers in error messages.
    /// </summary>
    public bool IncludeLineNumbers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to enable case-insensitive property lookup.
    /// </summary>
    public bool CaseInsensitiveLookup { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a collection of paths to search for templates.
    /// </summary>
    public ICollection<string> TemplatePaths { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets a collection of helper script paths to include.
    /// </summary>
    public ICollection<string> HelperScriptPaths { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets additional configuration parameters for the template engine.
    /// </summary>
    public IDictionary<string, object> AdditionalParameters { get; set; } = new Dictionary<string, object>();
}
