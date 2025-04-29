namespace EzDbCodeGen.Core.CodeGeneration;

using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents a code generator that orchestrates the entire code generation process.
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    /// Generates code from a database schema.
    /// </summary>
    /// <param name="schema">The database schema.</param>
    /// <param name="templatePath">The path to the template.</param>
    /// <param name="outputPath">The path to output generated code.</param>
    /// <param name="options">The code generation options.</param>
    /// <returns>A task representing the asynchronous code generation operation.</returns>
    Task GenerateAsync(IDatabaseSchema schema, string templatePath, string outputPath, CodeGenerationOptions options);
    
    /// <summary>
    /// Generates code from a database schema using a specific template for each entity.
    /// </summary>
    /// <param name="schema">The database schema.</param>
    /// <param name="templatePath">The path to the template.</param>
    /// <param name="outputPath">The path to output generated code.</param>
    /// <param name="options">The code generation options.</param>
    /// <returns>A task representing the asynchronous code generation operation.</returns>
    Task GeneratePerEntityAsync(IDatabaseSchema schema, string templatePath, string outputPath, CodeGenerationOptions options);
    
    /// <summary>
    /// Generates code from a database schema using a template string.
    /// </summary>
    /// <param name="schema">The database schema.</param>
    /// <param name="template">The template string.</param>
    /// <param name="outputPath">The path to output generated code.</param>
    /// <param name="options">The code generation options.</param>
    /// <returns>A task representing the asynchronous code generation operation.</returns>
    Task GenerateFromStringAsync(IDatabaseSchema schema, string template, string outputPath, CodeGenerationOptions options);
    
    /// <summary>
    /// Generates code incrementally, only updating files that have changed.
    /// </summary>
    /// <param name="schema">The database schema.</param>
    /// <param name="templatePath">The path to the template.</param>
    /// <param name="outputPath">The path to output generated code.</param>
    /// <param name="options">The code generation options.</param>
    /// <returns>A task representing the asynchronous code generation operation.</returns>
    Task GenerateIncrementalAsync(IDatabaseSchema schema, string templatePath, string outputPath, CodeGenerationOptions options);
    
    /// <summary>
    /// Gets the template processor used by the code generator.
    /// </summary>
    ITemplateProcessor TemplateProcessor { get; }
    
    /// <summary>
    /// Gets the template engine used by the code generator.
    /// </summary>
    ITemplateEngine TemplateEngine { get; }
    
    /// <summary>
    /// Gets the relationship detector used by the code generator.
    /// </summary>
    IRelationshipDetector RelationshipDetector { get; }
}
