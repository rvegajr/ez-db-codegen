namespace EzDbCodeGen.CodeGen.Interfaces.TypeMapping;

/// <summary>
/// Defines an interface for mapping database types to programming language types.
/// </summary>
public interface IDataTypeMap
{
    /// <summary>
    /// Maps a database type to a programming language type.
    /// </summary>
    /// <param name="databaseType">The database type to map.</param>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <param name="precision">The precision for numeric types.</param>
    /// <param name="scale">The scale for numeric types.</param>
    /// <param name="maxLength">The maximum length for string types.</param>
    /// <returns>The corresponding programming language type.</returns>
    string MapType(string databaseType, string targetLanguage, bool isNullable = false, int? precision = null, int? scale = null, int? maxLength = null);
    
    /// <summary>
    /// Gets the default value for a type in the specified language.
    /// </summary>
    /// <param name="databaseType">The database type.</param>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The default value for the type.</returns>
    string GetDefaultValue(string databaseType, string targetLanguage, bool isNullable = false);
    
    /// <summary>
    /// Adds a custom type mapping.
    /// </summary>
    /// <param name="databaseType">The database type.</param>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <param name="languageType">The programming language type.</param>
    void AddTypeMapping(string databaseType, string targetLanguage, string languageType);
    
    /// <summary>
    /// Gets all supported database types.
    /// </summary>
    /// <returns>A collection of supported database types.</returns>
    IReadOnlyCollection<string> GetSupportedDatabaseTypes();
    
    /// <summary>
    /// Gets all supported target languages.
    /// </summary>
    /// <returns>A collection of supported target languages.</returns>
    IReadOnlyCollection<string> GetSupportedTargetLanguages();
    
    /// <summary>
    /// Formats a type with nullability syntax for the specified language.
    /// </summary>
    /// <param name="type">The type name.</param>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The type formatted with appropriate nullability syntax.</returns>
    string FormatWithNullability(string type, string targetLanguage, bool isNullable);
    
    /// <summary>
    /// Loads type mappings from a configuration.
    /// </summary>
    /// <param name="configurationPath">The path to the configuration file.</param>
    void LoadMappingsFromConfiguration(string configurationPath);
}
