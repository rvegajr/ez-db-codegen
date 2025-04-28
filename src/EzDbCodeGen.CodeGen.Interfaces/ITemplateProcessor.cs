namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines the interface for a template processor that handles template loading, processing, and output generation.
/// </summary>
public interface ITemplateProcessor
{
    /// <summary>
    /// Processes a template file with the specified data model.
    /// </summary>
    /// <param name="templatePath">The path to the template file.</param>
    /// <param name="dataModel">The data model to use for template processing.</param>
    /// <returns>The processed template output.</returns>
    string ProcessTemplate(string templatePath, object dataModel);
    
    /// <summary>
    /// Processes a template string with the specified data model.
    /// </summary>
    /// <param name="templateContent">The content of the template.</param>
    /// <param name="dataModel">The data model to use for template processing.</param>
    /// <returns>The processed template output.</returns>
    string ProcessTemplateContent(string templateContent, object dataModel);
    
    /// <summary>
    /// Processes a layout template with the specified body content and data model.
    /// </summary>
    /// <param name="layoutTemplatePath">The path to the layout template file.</param>
    /// <param name="bodyContent">The content to insert into the layout.</param>
    /// <param name="dataModel">The data model to use for template processing.</param>
    /// <returns>The processed layout with body content included.</returns>
    string ProcessLayout(string layoutTemplatePath, string bodyContent, object dataModel);
    
    /// <summary>
    /// Gets the template engine used by this processor.
    /// </summary>
    ITemplateEngine TemplateEngine { get; }
    
    /// <summary>
    /// Gets or sets the base path for resolving relative template paths.
    /// </summary>
    string BasePath { get; set; }
    
    /// <summary>
    /// Registers a custom helper function with the template processor.
    /// </summary>
    /// <param name="name">The name of the helper function.</param>
    /// <param name="helper">The helper function to register.</param>
    void RegisterHelper(string name, Delegate helper);
    
    /// <summary>
    /// Registers a custom block helper function with the template processor.
    /// </summary>
    /// <param name="name">The name of the block helper function.</param>
    /// <param name="helper">The block helper function to register.</param>
    void RegisterBlockHelper(string name, Delegate helper);
    
    /// <summary>
    /// Registers a partial template with the template processor.
    /// </summary>
    /// <param name="name">The name of the partial template.</param>
    /// <param name="templatePath">The path to the partial template file.</param>
    void RegisterPartial(string name, string templatePath);
}
