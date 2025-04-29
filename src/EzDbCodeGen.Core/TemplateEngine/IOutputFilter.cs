namespace EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents a filter that can transform the output of a template processor.
/// </summary>
public interface IOutputFilter
{
    /// <summary>
    /// Applies the filter to the output of a template processor.
    /// </summary>
    /// <param name="outputPath">The path to the output file.</param>
    /// <param name="content">The content of the output file.</param>
    /// <returns>The transformed content.</returns>
    string ApplyFilter(string outputPath, string content);
    
    /// <summary>
    /// Gets a value indicating whether this filter should be applied to the specified output path.
    /// </summary>
    /// <param name="outputPath">The path to the output file.</param>
    /// <returns>True if the filter should be applied, false otherwise.</returns>
    bool ShouldApply(string outputPath);
    
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the description of the filter.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the order in which this filter should be applied.
    /// Lower values are applied first.
    /// </summary>
    int Order { get; }
}
