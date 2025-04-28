namespace EzDbCodeGen.CodeGen.Interfaces;

using EzDbCodeGen.CodeGen.Interfaces.Helpers;
using EzDbCodeGen.CodeGen.Interfaces.TypeMapping;

/// <summary>
/// Defines an interface for a service locator that provides access to the various services and components of the code generation system.
/// </summary>
public interface ICodeGenServiceLocator
{
    /// <summary>
    /// Gets the template engine factory.
    /// </summary>
    /// <returns>The template engine factory.</returns>
    ITemplateEngineFactory GetTemplateEngineFactory();
    
    /// <summary>
    /// Gets the template processor factory.
    /// </summary>
    /// <returns>The template processor factory.</returns>
    ITemplateProcessorFactory GetTemplateProcessorFactory();
    
    /// <summary>
    /// Gets the code generator.
    /// </summary>
    /// <returns>The code generator.</returns>
    ICodeGenerator GetCodeGenerator();
    
    /// <summary>
    /// Gets the file output manager.
    /// </summary>
    /// <returns>The file output manager.</returns>
    IFileOutputManager GetFileOutputManager();
    
    /// <summary>
    /// Gets the template repository.
    /// </summary>
    /// <returns>The template repository.</returns>
    ITemplateRepository GetTemplateRepository();
    
    /// <summary>
    /// Gets the template cache.
    /// </summary>
    /// <returns>The template cache.</returns>
    ITemplateCache GetTemplateCache();
    
    /// <summary>
    /// Gets the helper registry.
    /// </summary>
    /// <returns>The helper registry.</returns>
    IHelperRegistry GetHelperRegistry();
    
    /// <summary>
    /// Gets the template registry.
    /// </summary>
    /// <returns>The template registry.</returns>
    ITemplateRegistry GetTemplateRegistry();
    
    /// <summary>
    /// Gets the template resolver.
    /// </summary>
    /// <returns>The template resolver.</returns>
    ITemplateResolver GetTemplateResolver();
    
    /// <summary>
    /// Gets the data type map factory.
    /// </summary>
    /// <returns>The data type map factory.</returns>
    IDataTypeMapFactory GetDataTypeMapFactory();
    
    /// <summary>
    /// Gets the model builder factory.
    /// </summary>
    /// <returns>The model builder factory.</returns>
    IModelBuilderFactory GetModelBuilderFactory();
    
    /// <summary>
    /// Gets the model transformer factory.
    /// </summary>
    /// <returns>The model transformer factory.</returns>
    IModelTransformerFactory GetModelTransformerFactory();
    
    /// <summary>
    /// Gets the template context provider factory.
    /// </summary>
    /// <returns>The template context provider factory.</returns>
    ITemplateContextProviderFactory GetTemplateContextProviderFactory();
    
    /// <summary>
    /// Gets the code generation event handler factory.
    /// </summary>
    /// <returns>The code generation event handler factory.</returns>
    ICodeGenerationEventHandlerFactory GetCodeGenerationEventHandlerFactory();
    
    /// <summary>
    /// Gets the output formatter factory.
    /// </summary>
    /// <returns>The output formatter factory.</returns>
    IOutputFormatterFactory GetOutputFormatterFactory();
    
    /// <summary>
    /// Gets the template dependency manager.
    /// </summary>
    /// <returns>The template dependency manager.</returns>
    ITemplateDependencyManager GetTemplateDependencyManager();
    
    /// <summary>
    /// Gets the template analyzer.
    /// </summary>
    /// <returns>The template analyzer.</returns>
    ITemplateAnalyzer GetTemplateAnalyzer();
    
    /// <summary>
    /// Gets the documentation generator.
    /// </summary>
    /// <returns>The documentation generator.</returns>
    IDocumentationGenerator GetDocumentationGenerator();
    
    /// <summary>
    /// Gets the code generation logger.
    /// </summary>
    /// <returns>The code generation logger.</returns>
    ICodeGenerationLogger GetCodeGenerationLogger();
    
    /// <summary>
    /// Gets the plugin manager.
    /// </summary>
    /// <returns>The plugin manager.</returns>
    IPluginManager GetPluginManager();
    
    /// <summary>
    /// Gets the settings provider.
    /// </summary>
    /// <returns>The settings provider.</returns>
    ISettingsProvider GetSettingsProvider();
    
    /// <summary>
    /// Gets the configuration provider.
    /// </summary>
    /// <returns>The configuration provider.</returns>
    IConfigurationProvider GetConfigurationProvider();
    
    /// <summary>
    /// Gets the code generation service.
    /// </summary>
    /// <returns>The code generation service.</returns>
    ICodeGenerationService GetCodeGenerationService();
    
    /// <summary>
    /// Gets the code generation lifecycle manager.
    /// </summary>
    /// <returns>The code generation lifecycle manager.</returns>
    ICodeGenerationLifecycleManager GetCodeGenerationLifecycleManager();
    
    /// <summary>
    /// Gets the code snippet repository.
    /// </summary>
    /// <returns>The code snippet repository.</returns>
    ICodeSnippetRepository GetCodeSnippetRepository();
    
    /// <summary>
    /// Gets the generation progress tracker.
    /// </summary>
    /// <returns>The generation progress tracker.</returns>
    IGenerationProgressTracker GetGenerationProgressTracker();
    
    /// <summary>
    /// Gets the template validator.
    /// </summary>
    /// <returns>The template validator.</returns>
    ITemplateValidator GetTemplateValidator();
    
    /// <summary>
    /// Gets a service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>The service instance.</returns>
    T GetService<T>() where T : class;
    
    /// <summary>
    /// Registers a service instance.
    /// </summary>
    /// <typeparam name="T">The type of service to register.</typeparam>
    /// <param name="service">The service instance to register.</param>
    void RegisterService<T>(T service) where T : class;
}
