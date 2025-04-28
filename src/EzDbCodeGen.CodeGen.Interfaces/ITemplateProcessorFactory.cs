namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines a factory interface for creating template processors.
/// </summary>
public interface ITemplateProcessorFactory
{
    /// <summary>
    /// Creates a template processor using the specified template engine type.
    /// </summary>
    /// <param name="engineType">The type of template engine to use.</param>
    /// <returns>A configured template processor.</returns>
    ITemplateProcessor CreateProcessor(TemplateEngineType engineType);
    
    /// <summary>
    /// Creates a template processor with the specified template engine and options.
    /// </summary>
    /// <param name="engineType">The type of template engine to use.</param>
    /// <param name="options">The options for configuring the template processor.</param>
    /// <returns>A configured template processor.</returns>
    ITemplateProcessor CreateProcessor(TemplateEngineType engineType, TemplateEngineOptions options);
    
    /// <summary>
    /// Creates a template processor with a specific template engine instance.
    /// </summary>
    /// <param name="templateEngine">The template engine instance to use.</param>
    /// <returns>A template processor using the provided template engine.</returns>
    ITemplateProcessor CreateProcessor(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Creates a template processor with filters for selective processing.
    /// </summary>
    /// <param name="engineType">The type of template engine to use.</param>
    /// <param name="filters">A collection of filters to apply during processing.</param>
    /// <returns>A filtered template processor.</returns>
    ITemplateProcessor CreateFilteredProcessor(TemplateEngineType engineType, IEnumerable<string> filters);
    
    /// <summary>
    /// Creates a template processor that processes differences between two models.
    /// </summary>
    /// <param name="engineType">The type of template engine to use.</param>
    /// <returns>A differential template processor.</returns>
    ITemplateProcessor CreateDifferentialProcessor(TemplateEngineType engineType);
}
