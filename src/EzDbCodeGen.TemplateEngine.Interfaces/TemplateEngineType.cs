namespace EzDbCodeGen.TemplateEngine.Interfaces;

/// <summary>
/// Defines the available template engine types.
/// </summary>
public enum TemplateEngineType
{
    /// <summary>
    /// Handlebars template engine.
    /// </summary>
    Handlebars,
    
    /// <summary>
    /// Razor template engine.
    /// </summary>
    Razor,
    
    /// <summary>
    /// Liquid template engine.
    /// </summary>
    Liquid,
    
    /// <summary>
    /// Mustache template engine.
    /// </summary>
    Mustache,
    
    /// <summary>
    /// Custom template engine.
    /// </summary>
    Custom
}
