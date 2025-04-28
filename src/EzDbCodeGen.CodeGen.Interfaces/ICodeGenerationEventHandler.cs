namespace EzDbCodeGen.CodeGen.Interfaces;

using EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Defines an interface for handling code generation events.
/// </summary>
public interface ICodeGenerationEventHandler
{
    /// <summary>
    /// Gets the name of the event handler.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a description of the event handler.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the priority of the event handler. Lower values indicate higher priority.
    /// </summary>
    int Priority { get; }
    
    /// <summary>
    /// Handles the event fired when code generation begins.
    /// </summary>
    /// <param name="configuration">The code generation configuration.</param>
    void OnGenerationStarting(ICodeGenerationConfiguration configuration);
    
    /// <summary>
    /// Handles the event fired when code generation completes.
    /// </summary>
    /// <param name="result">The generation result.</param>
    void OnGenerationCompleted(IGenerationResult result);
    
    /// <summary>
    /// Handles the event fired before a template is processed.
    /// </summary>
    /// <param name="templateName">The name of the template being processed.</param>
    /// <param name="model">The model being used for rendering.</param>
    /// <param name="context">The rendering context.</param>
    void OnBeforeTemplateProcessed(string templateName, object? model, IRenderingContext context);
    
    /// <summary>
    /// Handles the event fired after a template is processed.
    /// </summary>
    /// <param name="templateName">The name of the template that was processed.</param>
    /// <param name="model">The model that was used for rendering.</param>
    /// <param name="context">The rendering context.</param>
    /// <param name="result">The template processing result.</param>
    void OnAfterTemplateProcessed(string templateName, object? model, IRenderingContext context, string result);
    
    /// <summary>
    /// Handles the event fired before an entity is processed.
    /// </summary>
    /// <param name="entity">The entity being processed.</param>
    /// <param name="schema">The database schema containing the entity.</param>
    void OnBeforeEntityProcessed(object entity, IDatabaseSchema schema);
    
    /// <summary>
    /// Handles the event fired after an entity is processed.
    /// </summary>
    /// <param name="entity">The entity that was processed.</param>
    /// <param name="schema">The database schema containing the entity.</param>
    /// <param name="generatedFiles">The files that were generated for the entity.</param>
    void OnAfterEntityProcessed(object entity, IDatabaseSchema schema, IReadOnlyCollection<IGeneratedFile> generatedFiles);
    
    /// <summary>
    /// Handles the event fired when an error occurs during generation.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <param name="exception">The exception that occurred, if any.</param>
    void OnGenerationError(string error, Exception? exception);
    
    /// <summary>
    /// Handles the event fired before a file is written.
    /// </summary>
    /// <param name="filePath">The path of the file being written.</param>
    /// <param name="content">The content being written to the file.</param>
    /// <param name="context">The rendering context.</param>
    /// <returns>The content to write to the file, potentially modified by the handler.</returns>
    string OnBeforeFileWritten(string filePath, string content, IRenderingContext context);
    
    /// <summary>
    /// Handles the event fired after a file is written.
    /// </summary>
    /// <param name="generatedFile">The file that was generated.</param>
    void OnAfterFileWritten(IGeneratedFile generatedFile);
    
    /// <summary>
    /// Initializes the event handler with configuration parameters.
    /// </summary>
    /// <param name="configuration">The configuration parameters.</param>
    void Initialize(IDictionary<string, object> configuration);
}
