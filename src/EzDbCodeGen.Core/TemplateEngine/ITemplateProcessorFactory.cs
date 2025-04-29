namespace EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents a factory for creating template processors.
/// </summary>
public interface ITemplateProcessorFactory
{
    /// <summary>
    /// Creates a template processor with the specified template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to use.</param>
    /// <returns>A template processor.</returns>
    ITemplateProcessor CreateTemplateProcessor(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Creates a template processor with the specified template engine and options.
    /// </summary>
    /// <param name="templateEngine">The template engine to use.</param>
    /// <param name="options">The template processor options.</param>
    /// <returns>A template processor.</returns>
    ITemplateProcessor CreateTemplateProcessor(ITemplateEngine templateEngine, TemplateProcessorOptions options);
    
    /// <summary>
    /// Creates a differential template processor with the specified template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to use.</param>
    /// <returns>A differential template processor.</returns>
    IDifferentialTemplateProcessor CreateDifferentialTemplateProcessor(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Creates a differential template processor with the specified template engine and options.
    /// </summary>
    /// <param name="templateEngine">The template engine to use.</param>
    /// <param name="options">The template processor options.</param>
    /// <returns>A differential template processor.</returns>
    IDifferentialTemplateProcessor CreateDifferentialTemplateProcessor(ITemplateEngine templateEngine, TemplateProcessorOptions options);
    
    /// <summary>
    /// Creates a filtered template processor that applies transformations to the generated output.
    /// </summary>
    /// <param name="baseProcessor">The base template processor.</param>
    /// <param name="filters">The output filters to apply.</param>
    /// <returns>A template processor with filters.</returns>
    ITemplateProcessor CreateFilteredTemplateProcessor(ITemplateProcessor baseProcessor, IEnumerable<IOutputFilter> filters);
    
    /// <summary>
    /// Registers a template processor factory method.
    /// </summary>
    /// <param name="processorType">The type of template processor.</param>
    /// <param name="factoryMethod">The factory method.</param>
    void RegisterTemplateProcessorFactory(string processorType, Func<ITemplateEngine, TemplateProcessorOptions, ITemplateProcessor> factoryMethod);
}
