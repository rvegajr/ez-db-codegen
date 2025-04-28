namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Represents the types of template engines supported by the system.
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
    /// T4 template engine.
    /// </summary>
    T4,
    
    /// <summary>
    /// Scriban template engine.
    /// </summary>
    Scriban,
    
    /// <summary>
    /// Custom template engine.
    /// </summary>
    Custom
}
