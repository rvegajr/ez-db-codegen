namespace EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents a template processor that performs differential processing.
/// </summary>
public interface IDifferentialTemplateProcessor : ITemplateProcessor
{
    /// <summary>
    /// Processes a template with new and old data, generating only changed output.
    /// </summary>
    /// <param name="template">The template to process.</param>
    /// <param name="newData">The new data.</param>
    /// <param name="oldData">The old data.</param>
    /// <returns>A dictionary of file paths and content.</returns>
    Task<IDictionary<string, string>> ProcessIncrementalAsync(string template, object newData, object oldData);
    
    /// <summary>
    /// Processes a template file with new and old data, generating only changed output.
    /// </summary>
    /// <param name="templatePath">The path to the template file.</param>
    /// <param name="newData">The new data.</param>
    /// <param name="oldData">The old data.</param>
    /// <returns>A dictionary of file paths and content.</returns>
    Task<IDictionary<string, string>> ProcessFileIncrementalAsync(string templatePath, object newData, object oldData);
    
    /// <summary>
    /// Takes a snapshot of data for later comparison.
    /// </summary>
    /// <param name="data">The data to snapshot.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task TakeSnapshotAsync(object data);
    
    /// <summary>
    /// Gets the snapshot data.
    /// </summary>
    /// <returns>The snapshot data, or null if no snapshot has been taken.</returns>
    Task<object?> GetSnapshotAsync();
}
