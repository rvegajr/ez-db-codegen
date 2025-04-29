namespace EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents a template engine that compiles and renders templates.
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Registers a helper function with the template engine.
    /// </summary>
    /// <param name="name">The name of the helper.</param>
    /// <param name="helper">The helper function.</param>
    void RegisterHelper(string name, Delegate helper);
    
    /// <summary>
    /// Registers a block helper with the template engine.
    /// </summary>
    /// <param name="name">The name of the helper.</param>
    /// <param name="helper">The helper function.</param>
    void RegisterBlockHelper(string name, Delegate helper);
    
    /// <summary>
    /// Registers a partial template with the template engine.
    /// </summary>
    /// <param name="name">The name of the partial.</param>
    /// <param name="template">The partial template.</param>
    void RegisterPartial(string name, string template);
    
    /// <summary>
    /// Compiles and renders a template with the provided data.
    /// </summary>
    /// <param name="template">The template to compile.</param>
    /// <param name="data">The data to render the template with.</param>
    /// <returns>The rendered template.</returns>
    string Compile(string template, object data);
    
    /// <summary>
    /// Compiles a template for later use.
    /// </summary>
    /// <param name="template">The template to compile.</param>
    /// <returns>A function that can render the template with data.</returns>
    Func<object, string> Compile(string template);
    
    /// <summary>
    /// Gets the engine type.
    /// </summary>
    string EngineType { get; }
}
