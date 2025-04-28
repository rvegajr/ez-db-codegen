namespace EzDbCodeGen.CodeGen.Interfaces;

using EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Defines an interface for managing the lifecycle of the code generation process.
/// </summary>
public interface ICodeGenerationLifecycleManager
{
    /// <summary>
    /// Gets or sets the code generation configuration.
    /// </summary>
    ICodeGenerationConfiguration Configuration { get; set; }
    
    /// <summary>
    /// Gets or sets the code generation event handler to use for lifecycle events.
    /// </summary>
    ICodeGenerationEventHandler EventHandler { get; set; }
    
    /// <summary>
    /// Gets or sets the logger to use for lifecycle logging.
    /// </summary>
    ICodeGenerationLogger Logger { get; set; }
    
    /// <summary>
    /// Gets or sets the progress tracker to use for lifecycle progress tracking.
    /// </summary>
    IGenerationProgressTracker ProgressTracker { get; set; }
    
    /// <summary>
    /// Initializes the code generation process.
    /// </summary>
    /// <param name="configuration">The code generation configuration to use.</param>
    void Initialize(ICodeGenerationConfiguration configuration);
    
    /// <summary>
    /// Loads the database schema to be used for code generation.
    /// </summary>
    /// <param name="schemaProviderOptions">The options for loading the schema.</param>
    /// <returns>The loaded database schema.</returns>
    IDatabaseSchema LoadSchema(SchemaProviderOptions schemaProviderOptions);
    
    /// <summary>
    /// Prepares the model for code generation.
    /// </summary>
    /// <param name="schema">The database schema to prepare a model from.</param>
    /// <returns>The prepared model.</returns>
    object PrepareModel(IDatabaseSchema schema);
    
    /// <summary>
    /// Applies transformations to the model before code generation.
    /// </summary>
    /// <param name="model">The model to transform.</param>
    /// <returns>The transformed model.</returns>
    object TransformModel(object model);
    
    /// <summary>
    /// Generates code from the model.
    /// </summary>
    /// <param name="model">The model to generate code from.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateCode(object model);
    
    /// <summary>
    /// Performs post-generation processing.
    /// </summary>
    /// <param name="result">The generation result.</param>
    /// <returns>The processed generation result.</returns>
    IGenerationResult PostProcess(IGenerationResult result);
    
    /// <summary>
    /// Finalizes the code generation process.
    /// </summary>
    /// <param name="result">The generation result.</param>
    void Finalize(IGenerationResult result);
    
    /// <summary>
    /// Executes the complete code generation lifecycle.
    /// </summary>
    /// <param name="configuration">The code generation configuration to use.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult Execute(ICodeGenerationConfiguration configuration);
    
    /// <summary>
    /// Handles any errors that occur during the code generation process.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <param name="exception">The exception that occurred, if any.</param>
    void HandleError(string error, Exception? exception);
    
    /// <summary>
    /// Cancels the ongoing code generation process.
    /// </summary>
    void Cancel();
}
