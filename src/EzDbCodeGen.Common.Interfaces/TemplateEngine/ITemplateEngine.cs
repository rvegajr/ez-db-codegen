using System;

namespace EzDbCodeGen.Common.Interfaces.TemplateEngine;

/// <summary>
/// Interface for template processing.
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Compiles a template string.
    /// </summary>
    /// <param name="templateContent">The template content to compile</param>
    /// <returns>A compiled template</returns>
    ICompiledTemplate Compile(string templateContent);

    /// <summary>
    /// Executes a compiled template with the given data model.
    /// </summary>
    /// <param name="template">The compiled template to execute.</param>
    /// <param name="dataModel">The data model to use when executing the template.</param>
    /// <returns>The result of the template execution.</returns>
    string Execute(ICompiledTemplate template, object dataModel);
    
    /// <summary>
    /// Processes a template string with the given data model.
    /// </summary>
    /// <param name="templateContent">The template content to process.</param>
    /// <param name="dataModel">The data model to use when processing the template.</param>
    /// <returns>The result of the template processing.</returns>
    string Process(string templateContent, object dataModel);

    /// <summary>
    /// Registers a helper function with the template engine.
    /// </summary>
    /// <param name="name">Name of the helper</param>
    /// <param name="helper">Helper function</param>
    void RegisterHelper(string name, Delegate helper);
    
    /// <summary>
    /// Registers a block helper function with the template engine.
    /// </summary>
    /// <param name="name">Name of the block helper</param>
    /// <param name="helper">Block helper function</param>
    void RegisterBlockHelper(string name, Delegate helper);
    
    /// <summary>
    /// Registers a partial template with the template engine.
    /// </summary>
    /// <param name="name">Name of the partial</param>
    /// <param name="partialContent">Partial template content</param>
    void RegisterPartial(string name, string partialContent);
}

/// <summary>
/// Interface for a compiled template.
/// </summary>
public interface ICompiledTemplate
{
    /// <summary>
    /// Renders the template with the provided data.
    /// </summary>
    /// <param name="data">Data to use in rendering</param>
    /// <returns>The rendered template</returns>
    string Render(object data);
}
