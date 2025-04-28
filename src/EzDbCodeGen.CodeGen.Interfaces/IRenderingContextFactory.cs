namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines a factory interface for creating rendering contexts.
/// </summary>
public interface IRenderingContextFactory
{
    /// <summary>
    /// Creates a new rendering context.
    /// </summary>
    /// <returns>A new rendering context.</returns>
    IRenderingContext CreateContext();
    
    /// <summary>
    /// Creates a new rendering context with the specified model.
    /// </summary>
    /// <param name="model">The model to use in the rendering context.</param>
    /// <returns>A new rendering context with the specified model.</returns>
    IRenderingContext CreateContext(object model);
    
    /// <summary>
    /// Creates a new rendering context with the specified parameters.
    /// </summary>
    /// <param name="parameters">The parameters to include in the rendering context.</param>
    /// <returns>A new rendering context with the specified parameters.</returns>
    IRenderingContext CreateContext(IDictionary<string, object> parameters);
    
    /// <summary>
    /// Creates a new rendering context with the specified model and parameters.
    /// </summary>
    /// <param name="model">The model to use in the rendering context.</param>
    /// <param name="parameters">The parameters to include in the rendering context.</param>
    /// <returns>A new rendering context with the specified model and parameters.</returns>
    IRenderingContext CreateContext(object model, IDictionary<string, object> parameters);
    
    /// <summary>
    /// Creates a new rendering context with the specified options.
    /// </summary>
    /// <param name="options">The template engine options to use.</param>
    /// <returns>A new rendering context with the specified options.</returns>
    IRenderingContext CreateContext(TemplateEngineOptions options);
    
    /// <summary>
    /// Creates a new rendering context with the specified naming convention.
    /// </summary>
    /// <param name="namingConvention">The naming convention to use.</param>
    /// <returns>A new rendering context with the specified naming convention.</returns>
    IRenderingContext CreateContext(NamingConvention namingConvention);
    
    /// <summary>
    /// Creates a new rendering context with full configuration.
    /// </summary>
    /// <param name="model">The model to use in the rendering context.</param>
    /// <param name="parameters">The parameters to include in the rendering context.</param>
    /// <param name="options">The template engine options to use.</param>
    /// <param name="namingConvention">The naming convention to use.</param>
    /// <returns>A fully configured rendering context.</returns>
    IRenderingContext CreateContext(object? model, IDictionary<string, object>? parameters, TemplateEngineOptions? options, NamingConvention? namingConvention);
}
