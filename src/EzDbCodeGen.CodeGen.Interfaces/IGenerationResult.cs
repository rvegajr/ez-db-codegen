namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for the result of a code generation operation.
/// </summary>
public interface IGenerationResult
{
    /// <summary>
    /// Gets a value indicating whether the generation was successful.
    /// </summary>
    bool Success { get; }
    
    /// <summary>
    /// Gets a collection of errors that occurred during generation.
    /// </summary>
    IReadOnlyCollection<string> Errors { get; }
    
    /// <summary>
    /// Gets a collection of warnings that occurred during generation.
    /// </summary>
    IReadOnlyCollection<string> Warnings { get; }
    
    /// <summary>
    /// Gets a collection of informational messages from the generation process.
    /// </summary>
    IReadOnlyCollection<string> Messages { get; }
    
    /// <summary>
    /// Gets a collection of files that were generated.
    /// </summary>
    IReadOnlyCollection<IGeneratedFile> GeneratedFiles { get; }
    
    /// <summary>
    /// Gets the total number of files that were generated.
    /// </summary>
    int FilesGenerated { get; }
    
    /// <summary>
    /// Gets the time it took to complete the generation process.
    /// </summary>
    TimeSpan ElapsedTime { get; }
    
    /// <summary>
    /// Adds an error message to the result.
    /// </summary>
    /// <param name="errorMessage">The error message to add.</param>
    void AddError(string errorMessage);
    
    /// <summary>
    /// Adds a warning message to the result.
    /// </summary>
    /// <param name="warningMessage">The warning message to add.</param>
    void AddWarning(string warningMessage);
    
    /// <summary>
    /// Adds an informational message to the result.
    /// </summary>
    /// <param name="message">The message to add.</param>
    void AddMessage(string message);
    
    /// <summary>
    /// Adds a generated file to the result.
    /// </summary>
    /// <param name="file">The generated file to add.</param>
    void AddGeneratedFile(IGeneratedFile file);
    
    /// <summary>
    /// Merges another generation result into this one.
    /// </summary>
    /// <param name="other">The other generation result to merge.</param>
    void Merge(IGenerationResult other);
}
