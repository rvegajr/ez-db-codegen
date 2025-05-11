using System.Collections.Generic;

namespace EzDbCodeGen.TemplateEngine.Interfaces;

/// <summary>
/// Interface for a factory that creates template processors.
/// </summary>
public interface ITemplateProcessorFactory
{
    /// <summary>
    /// Creates a template processor with the specified engine type and default options.
    /// </summary>
    /// <param name="engineType">The type of template engine to use.</param>
    /// <returns>The created template processor.</returns>
    ITemplateProcessor CreateProcessor(TemplateEngineType engineType);
    
    /// <summary>
    /// Creates a template processor with the specified engine type and options.
    /// </summary>
    /// <param name="engineType">The type of template engine to use.</param>
    /// <param name="options">The options to use when creating the template engine.</param>
    /// <returns>The created template processor.</returns>
    ITemplateProcessor CreateProcessor(TemplateEngineType engineType, TemplateEngineOptions options);
    
    /// <summary>
    /// Creates a template processor with the specified template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to use.</param>
    /// <returns>The created template processor.</returns>
    ITemplateProcessor CreateProcessor(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Creates a filtered template processor with the specified engine type and filters.
    /// </summary>
    /// <param name="engineType">The type of template engine to use.</param>
    /// <param name="filters">The filters to apply to the template processor.</param>
    /// <returns>The created filtered template processor.</returns>
    ITemplateProcessor CreateFilteredProcessor(TemplateEngineType engineType, IEnumerable<string> filters);
    
    /// <summary>
    /// Creates a differential template processor with the specified engine type.
    /// </summary>
    /// <param name="engineType">The type of template engine to use.</param>
    /// <returns>The created differential template processor.</returns>
    ITemplateProcessor CreateDifferentialProcessor(TemplateEngineType engineType);
}
