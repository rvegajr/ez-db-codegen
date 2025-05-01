using System;
using System.Threading.Tasks;

namespace EzDbCodeGen.Core.Interfaces;

/// <summary>
/// Interface for code generation.
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    /// Generates code based on the provided options.
    /// </summary>
    /// <param name="options">Code generation options</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task GenerateAsync(CodeGenerationOptions options);
}

/// <summary>
/// Options for code generation.
/// </summary>
public class CodeGenerationOptions
{
    /// <summary>
    /// Gets or sets the connection string for the database.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the output directory for generated code.
    /// </summary>
    public string OutputDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the template directory.
    /// </summary>
    public string TemplateDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to overwrite existing files.
    /// </summary>
    public bool Overwrite { get; set; }
}
