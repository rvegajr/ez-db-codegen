namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Represents a compiled template that can be executed with a data model.
/// </summary>
public interface ICompiledTemplate
{
    /// <summary>
    /// Gets the original template content.
    /// </summary>
    string TemplateContent { get; }
    
    /// <summary>
    /// Gets the name of the template, if available.
    /// </summary>
    string? Name { get; }
    
    /// <summary>
    /// Gets a value indicating whether the template was successfully compiled.
    /// </summary>
    bool IsValid { get; }
    
    /// <summary>
    /// Gets any compilation errors that occurred during template compilation.
    /// </summary>
    IReadOnlyList<string> Errors { get; }
}
