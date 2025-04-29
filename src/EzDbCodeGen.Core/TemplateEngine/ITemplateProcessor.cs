namespace EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents a template processor that processes templates with schema data.
/// </summary>
public interface ITemplateProcessor
{
    /// <summary>
    /// Processes a template with the provided data.
    /// </summary>
    /// <param name="template">The template to process.</param>
    /// <param name="data">The data to process the template with.</param>
    /// <returns>A dictionary of file paths and content.</returns>
    Task<IDictionary<string, string>> ProcessAsync(string template, object data);
    
    /// <summary>
    /// Processes a template file with the provided data.
    /// </summary>
    /// <param name="templatePath">The path to the template file.</param>
    /// <param name="data">The data to process the template with.</param>
    /// <returns>A dictionary of file paths and content.</returns>
    Task<IDictionary<string, string>> ProcessFileAsync(string templatePath, object data);
    
    /// <summary>
    /// Gets the template engine used by the processor.
    /// </summary>
    ITemplateEngine TemplateEngine { get; }
}
