namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for tracking progress during the code generation process.
/// </summary>
public interface IGenerationProgressTracker
{
    /// <summary>
    /// Gets the total number of items to be processed.
    /// </summary>
    int TotalItems { get; }
    
    /// <summary>
    /// Gets the number of items that have been processed.
    /// </summary>
    int ProcessedItems { get; }
    
    /// <summary>
    /// Gets the number of items that have been successfully processed.
    /// </summary>
    int SuccessfulItems { get; }
    
    /// <summary>
    /// Gets the number of items that have failed processing.
    /// </summary>
    int FailedItems { get; }
    
    /// <summary>
    /// Gets the current progress as a percentage (0-100).
    /// </summary>
    double ProgressPercentage { get; }
    
    /// <summary>
    /// Gets the elapsed time since the start of the generation process.
    /// </summary>
    TimeSpan ElapsedTime { get; }
    
    /// <summary>
    /// Gets the estimated time remaining for the generation process.
    /// </summary>
    TimeSpan? EstimatedTimeRemaining { get; }
    
    /// <summary>
    /// Gets the current status message.
    /// </summary>
    string StatusMessage { get; }
    
    /// <summary>
    /// Sets the total number of items to be processed.
    /// </summary>
    /// <param name="totalItems">The total number of items.</param>
    void SetTotalItems(int totalItems);
    
    /// <summary>
    /// Increments the number of processed items by one.
    /// </summary>
    /// <param name="success">Whether the item was processed successfully.</param>
    void IncrementProcessedItems(bool success = true);
    
    /// <summary>
    /// Updates the status message.
    /// </summary>
    /// <param name="message">The new status message.</param>
    void UpdateStatus(string message);
    
    /// <summary>
    /// Starts tracking progress.
    /// </summary>
    void Start();
    
    /// <summary>
    /// Pauses tracking progress.
    /// </summary>
    void Pause();
    
    /// <summary>
    /// Resumes tracking progress.
    /// </summary>
    void Resume();
    
    /// <summary>
    /// Completes tracking progress.
    /// </summary>
    void Complete();
    
    /// <summary>
    /// Adds an item that was processed.
    /// </summary>
    /// <param name="itemName">The name of the processed item.</param>
    /// <param name="success">Whether the item was processed successfully.</param>
    /// <param name="message">An optional message about the processing result.</param>
    void AddProcessedItem(string itemName, bool success, string? message = null);
    
    /// <summary>
    /// Gets a summary of the progress.
    /// </summary>
    /// <returns>A formatted summary of the progress.</returns>
    string GetProgressSummary();
}
