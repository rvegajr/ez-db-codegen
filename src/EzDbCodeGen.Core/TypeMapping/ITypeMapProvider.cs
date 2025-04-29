namespace EzDbCodeGen.Core.TypeMapping;

/// <summary>
/// Represents a provider of type maps for different programming languages.
/// </summary>
public interface ITypeMapProvider
{
    /// <summary>
    /// Gets a type map for a specific language.
    /// </summary>
    /// <param name="language">The target language.</param>
    /// <returns>A type map for the language.</returns>
    ITypeMap GetTypeMap(string language);
    
    /// <summary>
    /// Registers a type map for a specific language.
    /// </summary>
    /// <param name="language">The target language.</param>
    /// <param name="typeMap">The type map to register.</param>
    void RegisterTypeMap(string language, ITypeMap typeMap);
    
    /// <summary>
    /// Determines whether a type map is registered for a specific language.
    /// </summary>
    /// <param name="language">The target language.</param>
    /// <returns>True if a type map is registered for the language, false otherwise.</returns>
    bool HasTypeMap(string language);
    
    /// <summary>
    /// Gets all registered type maps.
    /// </summary>
    /// <returns>A dictionary of language-to-type map mappings.</returns>
    IReadOnlyDictionary<string, ITypeMap> GetAllTypeMaps();
}
