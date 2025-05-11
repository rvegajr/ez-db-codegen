using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Common.Interfaces.Schema;
using EzDbCodeGen.Common.Interfaces.TemplateEngine;

namespace EzDbCodeGen.Common.Interfaces.CodeGen;

/// <summary>
/// Defines the interface for a code generator that processes templates against database schemas.
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    /// Generates code using a template and a database schema.
    /// </summary>
    /// <param name="templatePath">The path to the template file.</param>
    /// <param name="schema">The database schema to use for code generation.</param>
    /// <param name="outputPath">The path where generated files should be saved.</param>
    /// <param name="options">The options for code generation.</param>
    /// <returns>A list of paths to the generated files.</returns>
    Task<IReadOnlyList<string>> GenerateCodeAsync(string templatePath, IDatabaseSchema schema, string outputPath, CodeGenerationOptions options);
    
    /// <summary>
    /// Generates code using a template content and a database schema.
    /// </summary>
    /// <param name="templateContent">The content of the template.</param>
    /// <param name="schema">The database schema to use for code generation.</param>
    /// <param name="outputPath">The path where generated files should be saved.</param>
    /// <param name="options">The options for code generation.</param>
    /// <returns>A list of paths to the generated files.</returns>
    Task<IReadOnlyList<string>> GenerateCodeFromContentAsync(string templateContent, IDatabaseSchema schema, string outputPath, CodeGenerationOptions options);
    
    /// <summary>
    /// Previews the code that would be generated without writing it to disk.
    /// </summary>
    /// <param name="templatePath">The path to the template file.</param>
    /// <param name="schema">The database schema to use for code generation.</param>
    /// <param name="options">The options for code generation.</param>
    /// <returns>A dictionary mapping file paths to their generated content.</returns>
    Task<IDictionary<string, string>> PreviewCodeAsync(string templatePath, IDatabaseSchema schema, CodeGenerationOptions options);
    
    /// <summary>
    /// Gets the template processor used by this code generator.
    /// </summary>
    ITemplateProcessor TemplateProcessor { get; }
    
    /// <summary>
    /// Gets or sets the base path for resolving relative template paths.
    /// </summary>
    string BasePath { get; set; }
    
    /// <summary>
    /// Gets or sets the output path for generated files.
    /// </summary>
    string OutputPath { get; set; }
    
    /// <summary>
    /// Registers a custom helper function with the code generator.
    /// </summary>
    /// <param name="name">The name of the helper function.</param>
    /// <param name="helper">The helper function to register.</param>
    void RegisterHelper(string name, Delegate helper);
    
    /// <summary>
    /// Registers a custom block helper function with the code generator.
    /// </summary>
    /// <param name="name">The name of the block helper function.</param>
    /// <param name="helper">The block helper function to register.</param>
    void RegisterBlockHelper(string name, Delegate helper);
}
