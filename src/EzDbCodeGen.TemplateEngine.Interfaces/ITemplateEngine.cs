using System;
using System.Threading.Tasks;

namespace EzDbCodeGen.TemplateEngine.Interfaces;

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
    /// Registers a helper function with the template engine.
    /// </summary>
    /// <param name="name">Name of the helper</param>
    /// <param name="helper">Helper function</param>
    void RegisterHelper(string name, Delegate helper);
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
