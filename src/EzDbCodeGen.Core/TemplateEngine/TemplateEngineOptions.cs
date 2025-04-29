namespace EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents options for configuring a template engine.
/// </summary>
public class TemplateEngineOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to enable template caching.
    /// </summary>
    public bool EnableCaching { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to throw exceptions on missing properties.
    /// </summary>
    public bool ThrowOnMissingProperty { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to throw exceptions on missing partials.
    /// </summary>
    public bool ThrowOnMissingPartial { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to throw exceptions on missing helpers.
    /// </summary>
    public bool ThrowOnMissingHelper { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to recompile partials.
    /// </summary>
    public bool RecompilePartials { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the path for resolving partials.
    /// </summary>
    public string? PartialsPath { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to register default helpers.
    /// </summary>
    public bool RegisterDefaultHelpers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to register format helpers.
    /// </summary>
    public bool RegisterFormatHelpers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to register type conversion helpers.
    /// </summary>
    public bool RegisterTypeConversionHelpers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to register documentation helpers.
    /// </summary>
    public bool RegisterDocumentationHelpers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to register layout helpers.
    /// </summary>
    public bool RegisterLayoutHelpers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to register code format helpers.
    /// </summary>
    public bool RegisterCodeFormatHelpers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to register relationship helpers.
    /// </summary>
    public bool RegisterRelationshipHelpers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a dictionary of custom helpers.
    /// </summary>
    public IDictionary<string, Delegate> CustomHelpers { get; set; } = new Dictionary<string, Delegate>();
    
    /// <summary>
    /// Gets or sets a dictionary of custom block helpers.
    /// </summary>
    public IDictionary<string, Delegate> CustomBlockHelpers { get; set; } = new Dictionary<string, Delegate>();
    
    /// <summary>
    /// Gets or sets a dictionary of custom partials.
    /// </summary>
    public IDictionary<string, string> CustomPartials { get; set; } = new Dictionary<string, string>();
}
