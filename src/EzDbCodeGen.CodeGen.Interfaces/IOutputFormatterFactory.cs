namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines a factory interface for creating and managing output formatters.
/// </summary>
public interface IOutputFormatterFactory
{
    /// <summary>
    /// Creates an output formatter of the specified type.
    /// </summary>
    /// <param name="formatterType">The type name of the formatter to create.</param>
    /// <returns>A new output formatter instance.</returns>
    IOutputFormatter CreateFormatter(string formatterType);
    
    /// <summary>
    /// Creates an output formatter of the specified type with configuration.
    /// </summary>
    /// <param name="formatterType">The type name of the formatter to create.</param>
    /// <param name="configuration">The configuration for the formatter.</param>
    /// <returns>A new configured output formatter instance.</returns>
    IOutputFormatter CreateFormatter(string formatterType, IDictionary<string, object> configuration);
    
    /// <summary>
    /// Gets all available formatter types.
    /// </summary>
    /// <returns>A collection of available formatter type names.</returns>
    IReadOnlyCollection<string> GetAvailableFormatterTypes();
    
    /// <summary>
    /// Gets a formatter for a specific file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension.</param>
    /// <returns>An output formatter that can format files with the specified extension, or null if none is available.</returns>
    IOutputFormatter? GetFormatterForExtension(string fileExtension);
    
    /// <summary>
    /// Registers an output formatter type.
    /// </summary>
    /// <param name="typeName">The name to register the formatter type under.</param>
    /// <param name="formatterType">The type of the formatter.</param>
    void RegisterFormatterType(string typeName, Type formatterType);
    
    /// <summary>
    /// Creates a composite formatter that applies multiple formatters in sequence.
    /// </summary>
    /// <param name="formatterNames">The names of the formatters to include in the composite.</param>
    /// <returns>A composite output formatter.</returns>
    IOutputFormatter CreateCompositeFormatter(params string[] formatterNames);
    
    /// <summary>
    /// Creates a composite formatter that applies multiple formatters in sequence.
    /// </summary>
    /// <param name="formatters">The formatters to include in the composite.</param>
    /// <returns>A composite output formatter.</returns>
    IOutputFormatter CreateCompositeFormatter(params IOutputFormatter[] formatters);
}
