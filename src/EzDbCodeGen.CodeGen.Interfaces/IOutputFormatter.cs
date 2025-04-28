namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for formatting the output of code generation.
/// </summary>
public interface IOutputFormatter
{
    /// <summary>
    /// Gets the name of the formatter.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a description of the formatter.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the supported file extensions for this formatter.
    /// </summary>
    IReadOnlyCollection<string> SupportedExtensions { get; }
    
    /// <summary>
    /// Gets the order in which this formatter should be applied relative to other formatters.
    /// Lower numbers are applied first.
    /// </summary>
    int Order { get; }
    
    /// <summary>
    /// Formats the content of a generated file.
    /// </summary>
    /// <param name="content">The content to format.</param>
    /// <param name="fileExtension">The file extension of the content.</param>
    /// <param name="options">Optional formatting options.</param>
    /// <returns>The formatted content.</returns>
    string Format(string content, string fileExtension, IDictionary<string, object>? options = null);
    
    /// <summary>
    /// Determines whether this formatter can format content with the specified file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension to check.</param>
    /// <returns>True if this formatter can format content with the specified file extension; otherwise, false.</returns>
    bool CanFormat(string fileExtension);
    
    /// <summary>
    /// Gets the default formatting options for this formatter.
    /// </summary>
    /// <returns>The default formatting options.</returns>
    IDictionary<string, object> GetDefaultOptions();
    
    /// <summary>
    /// Initializes the formatter with configuration parameters.
    /// </summary>
    /// <param name="configuration">The configuration parameters.</param>
    void Initialize(IDictionary<string, object> configuration);
}
