namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for the context used during template rendering.
/// </summary>
public interface IRenderingContext
{
    /// <summary>
    /// Gets or sets the model object being rendered.
    /// </summary>
    object? Model { get; set; }
    
    /// <summary>
    /// Gets or sets the template that is currently being rendered.
    /// </summary>
    string? CurrentTemplate { get; set; }
    
    /// <summary>
    /// Gets or sets the output path for the rendered content.
    /// </summary>
    string? OutputPath { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to overwrite existing files.
    /// </summary>
    bool OverwriteExisting { get; set; }
    
    /// <summary>
    /// Gets or sets additional parameters for the rendering process.
    /// </summary>
    IDictionary<string, object> Parameters { get; set; }
    
    /// <summary>
    /// Gets or sets the template engine options.
    /// </summary>
    TemplateEngineOptions Options { get; set; }
    
    /// <summary>
    /// Gets or sets the naming convention to use during rendering.
    /// </summary>
    NamingConvention NamingConvention { get; set; }
    
    /// <summary>
    /// Adds a parameter to the rendering context.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    void AddParameter(string name, object value);
    
    /// <summary>
    /// Gets a parameter from the rendering context.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <returns>The parameter value, or null if not found.</returns>
    object? GetParameter(string name);
    
    /// <summary>
    /// Removes a parameter from the rendering context.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <returns>True if the parameter was removed; otherwise, false.</returns>
    bool RemoveParameter(string name);
    
    /// <summary>
    /// Clears all parameters from the rendering context.
    /// </summary>
    void ClearParameters();
    
    /// <summary>
    /// Creates a new rendering context with a different model.
    /// </summary>
    /// <param name="model">The new model object.</param>
    /// <returns>A new rendering context with the specified model.</returns>
    IRenderingContext WithModel(object model);
    
    /// <summary>
    /// Creates a new rendering context with additional parameters.
    /// </summary>
    /// <param name="parameters">The additional parameters.</param>
    /// <returns>A new rendering context with the additional parameters.</returns>
    IRenderingContext WithParameters(IDictionary<string, object> parameters);
}
