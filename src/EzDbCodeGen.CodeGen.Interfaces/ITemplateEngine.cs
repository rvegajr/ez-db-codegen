namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines the interface for a template engine that processes templates and generates output.
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Compiles a template from the given template content.
    /// </summary>
    /// <param name="templateContent">The content of the template to compile.</param>
    /// <returns>A reference to the compiled template that can be executed.</returns>
    ICompiledTemplate Compile(string templateContent);

    /// <summary>
    /// Executes a compiled template with the given data model.
    /// </summary>
    /// <param name="template">The compiled template to execute.</param>
    /// <param name="dataModel">The data model to use when executing the template.</param>
    /// <returns>The result of the template execution.</returns>
    string Execute(ICompiledTemplate template, object dataModel);

    /// <summary>
    /// Compiles and executes a template in a single step.
    /// </summary>
    /// <param name="templateContent">The content of the template to compile and execute.</param>
    /// <param name="dataModel">The data model to use when executing the template.</param>
    /// <returns>The result of the template execution.</returns>
    string Process(string templateContent, object dataModel);

    /// <summary>
    /// Registers a custom helper function with the template engine.
    /// </summary>
    /// <param name="name">The name of the helper function.</param>
    /// <param name="helper">The helper function to register.</param>
    void RegisterHelper(string name, Delegate helper);

    /// <summary>
    /// Registers a custom block helper function with the template engine.
    /// </summary>
    /// <param name="name">The name of the block helper function.</param>
    /// <param name="helper">The block helper function to register.</param>
    void RegisterBlockHelper(string name, Delegate helper);

    /// <summary>
    /// Registers a partial template with the template engine.
    /// </summary>
    /// <param name="name">The name of the partial template.</param>
    /// <param name="template">The content of the partial template.</param>
    void RegisterPartial(string name, string template);
}
