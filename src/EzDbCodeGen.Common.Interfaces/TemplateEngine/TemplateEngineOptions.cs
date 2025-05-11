namespace EzDbCodeGen.Common.Interfaces.TemplateEngine;

/// <summary>
/// Options for configuring a template engine.
/// </summary>
public class TemplateEngineOptions
{
    /// <summary>
    /// Gets or sets whether to register standard helpers.
    /// </summary>
    public bool RegisterStandardHelpers { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to enable caching of compiled templates.
    /// </summary>
    public bool EnableCaching { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to throw exceptions on missing partials.
    /// </summary>
    public bool ThrowOnMissingPartial { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the template file extension.
    /// </summary>
    public string TemplateExtension { get; set; } = ".hbs";
}
