namespace EzDbCodeGen.Core.CodeGeneration;

using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents a factory for creating code generators.
/// </summary>
public interface ICodeGeneratorFactory
{
    /// <summary>
    /// Creates a code generator with the specified template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to use.</param>
    /// <returns>A code generator.</returns>
    ICodeGenerator CreateCodeGenerator(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Creates a code generator with the specified template engine and relationship detector.
    /// </summary>
    /// <param name="templateEngine">The template engine to use.</param>
    /// <param name="relationshipDetector">The relationship detector to use.</param>
    /// <returns>A code generator.</returns>
    ICodeGenerator CreateCodeGenerator(ITemplateEngine templateEngine, IRelationshipDetector relationshipDetector);
    
    /// <summary>
    /// Creates a code generator with the specified template processor and relationship detector.
    /// </summary>
    /// <param name="templateProcessor">The template processor to use.</param>
    /// <param name="relationshipDetector">The relationship detector to use.</param>
    /// <returns>A code generator.</returns>
    ICodeGenerator CreateCodeGenerator(ITemplateProcessor templateProcessor, IRelationshipDetector relationshipDetector);
    
    /// <summary>
    /// Creates a code generator with the specified template processor.
    /// </summary>
    /// <param name="templateProcessor">The template processor to use.</param>
    /// <returns>A code generator.</returns>
    ICodeGenerator CreateCodeGenerator(ITemplateProcessor templateProcessor);
    
    /// <summary>
    /// Creates a code generator with the specified options.
    /// </summary>
    /// <param name="options">The code generator options.</param>
    /// <returns>A code generator.</returns>
    ICodeGenerator CreateCodeGenerator(CodeGeneratorOptions options);
    
    /// <summary>
    /// Registers a code generator factory method.
    /// </summary>
    /// <param name="generatorType">The type of code generator.</param>
    /// <param name="factoryMethod">The factory method.</param>
    void RegisterCodeGeneratorFactory(string generatorType, Func<CodeGeneratorOptions, ICodeGenerator> factoryMethod);
}
