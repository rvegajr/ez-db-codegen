using EzDbCodeGen.CodeGen.Interfaces.TypeMapping;

namespace EzDbCodeGen.TypeMapping;

/// <summary>
/// Base implementation of the IDataTypeMap interface.
/// </summary>
public abstract class DataTypeMap : IDataTypeMap
{
    private readonly Dictionary<string, Dictionary<string, string>> _typeMappings = new();
    private readonly Dictionary<string, Dictionary<string, string>> _defaultValues = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeMap"/> class.
    /// </summary>
    protected DataTypeMap()
    {
        InitializeTypeMappings();
        InitializeDefaultValues();
    }

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
    public virtual string MapType(string databaseType, string targetLanguage, bool isNullable = false, int? precision = null, int? scale = null, int? maxLength = null)
    {
        if (string.IsNullOrEmpty(databaseType))
        {
            throw new ArgumentNullException(nameof(databaseType));
        }

        if (string.IsNullOrEmpty(targetLanguage))
        {
            throw new ArgumentNullException(nameof(targetLanguage));
        }

        if (!_typeMappings.ContainsKey(targetLanguage))
        {
            throw new ArgumentException($"Unsupported target language: {targetLanguage}", nameof(targetLanguage));
        }

        var normalizedType = NormalizeTypeName(databaseType);
        
        if (!_typeMappings[targetLanguage].ContainsKey(normalizedType))
        {
            // Default to string if type is not mapped
            normalizedType = "string";
        }

        var mappedType = _typeMappings[targetLanguage][normalizedType];

        // Apply custom transformations based on type-specific parameters
        mappedType = ApplyTypeSpecificTransformations(mappedType, databaseType, targetLanguage, precision, scale, maxLength);

        // Apply nullability formatting
        return FormatWithNullability(mappedType, targetLanguage, isNullable);
    }

    /// <summary>
    /// Gets the default value for a type in the specified language.
    /// </summary>
    /// <param name="databaseType">The database type.</param>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The default value for the type.</returns>
    public virtual string GetDefaultValue(string databaseType, string targetLanguage, bool isNullable = false)
    {
        if (string.IsNullOrEmpty(databaseType))
        {
            throw new ArgumentNullException(nameof(databaseType));
        }

        if (string.IsNullOrEmpty(targetLanguage))
        {
            throw new ArgumentNullException(nameof(targetLanguage));
        }

        if (!_defaultValues.ContainsKey(targetLanguage))
        {
            throw new ArgumentException($"Unsupported target language: {targetLanguage}", nameof(targetLanguage));
        }

        var normalizedType = NormalizeTypeName(databaseType);
        
        if (isNullable)
        {
            return GetNullLiteralForLanguage(targetLanguage);
        }

        if (!_defaultValues[targetLanguage].ContainsKey(normalizedType))
        {
            // Default to string default if type is not mapped
            normalizedType = "string";
        }

        return _defaultValues[targetLanguage][normalizedType];
    }

    /// <summary>
    /// Adds a custom type mapping.
    /// </summary>
    /// <param name="databaseType">The database type.</param>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <param name="languageType">The programming language type.</param>
    public virtual void AddTypeMapping(string databaseType, string targetLanguage, string languageType)
    {
        if (string.IsNullOrEmpty(databaseType))
        {
            throw new ArgumentNullException(nameof(databaseType));
        }

        if (string.IsNullOrEmpty(targetLanguage))
        {
            throw new ArgumentNullException(nameof(targetLanguage));
        }

        if (string.IsNullOrEmpty(languageType))
        {
            throw new ArgumentNullException(nameof(languageType));
        }

        if (!_typeMappings.ContainsKey(targetLanguage))
        {
            _typeMappings[targetLanguage] = new Dictionary<string, string>();
        }

        var normalizedType = NormalizeTypeName(databaseType);
        _typeMappings[targetLanguage][normalizedType] = languageType;
    }

    /// <summary>
    /// Gets all supported database types.
    /// </summary>
    /// <returns>A collection of supported database types.</returns>
    public virtual IReadOnlyCollection<string> GetSupportedDatabaseTypes()
    {
        // Get the first language's supported types as a representative sample
        var firstLanguage = _typeMappings.Keys.FirstOrDefault();
        return firstLanguage != null 
            ? _typeMappings[firstLanguage].Keys.ToList() 
            : new List<string>();
    }

    /// <summary>
    /// Gets all supported target languages.
    /// </summary>
    /// <returns>A collection of supported target languages.</returns>
    public virtual IReadOnlyCollection<string> GetSupportedTargetLanguages()
    {
        return _typeMappings.Keys.ToList();
    }

    /// <summary>
    /// Formats a type with nullability syntax for the specified language.
    /// </summary>
    /// <param name="type">The type name.</param>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The type formatted with appropriate nullability syntax.</returns>
    public virtual string FormatWithNullability(string type, string targetLanguage, bool isNullable)
    {
        if (string.IsNullOrEmpty(type))
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (string.IsNullOrEmpty(targetLanguage))
        {
            throw new ArgumentNullException(nameof(targetLanguage));
        }

        if (!isNullable)
        {
            return type;
        }

        return targetLanguage.ToLowerInvariant() switch
        {
            "csharp" or "c#" => IsValueType(type, targetLanguage) ? $"{type}?" : type,
            "typescript" or "ts" => $"{type} | null",
            "java" => IsValueType(type, targetLanguage) ? $"Optional<{type}>" : type,
            "python" or "py" => $"Optional[{type}]",
            _ => type
        };
    }

    /// <summary>
    /// Loads type mappings from a configuration.
    /// </summary>
    /// <param name="configurationPath">The path to the configuration file.</param>
    public virtual void LoadMappingsFromConfiguration(string configurationPath)
    {
        // This method would load mappings from a configuration file
        // For now, we'll rely on the built-in mappings
        throw new NotImplementedException("Loading mappings from configuration is not yet implemented.");
    }

    /// <summary>
    /// Initializes the type mappings for supported languages.
    /// </summary>
    protected abstract void InitializeTypeMappings();

    /// <summary>
    /// Initializes the default values for supported languages.
    /// </summary>
    protected abstract void InitializeDefaultValues();

    /// <summary>
    /// Normalizes a database type name by removing length/precision information and standardizing capitalization.
    /// </summary>
    /// <param name="typeName">The type name to normalize.</param>
    /// <returns>The normalized type name.</returns>
    protected virtual string NormalizeTypeName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return string.Empty;
        }

        // Remove any parameters from the type (e.g., varchar(255) -> varchar)
        var parenIndex = typeName.IndexOf('(');
        var baseType = parenIndex > 0 ? typeName.Substring(0, parenIndex) : typeName;
        
        // Standardize capitalization to lowercase
        return baseType.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Applies type-specific transformations based on precision, scale, and length.
    /// </summary>
    /// <param name="mappedType">The already mapped type.</param>
    /// <param name="databaseType">The original database type.</param>
    /// <param name="targetLanguage">The target language.</param>
    /// <param name="precision">The precision for numeric types.</param>
    /// <param name="scale">The scale for numeric types.</param>
    /// <param name="maxLength">The maximum length for string types.</param>
    /// <returns>The transformed type.</returns>
    protected virtual string ApplyTypeSpecificTransformations(string mappedType, string databaseType, string targetLanguage, int? precision, int? scale, int? maxLength)
    {
        return mappedType; // Base implementation does no transformations
    }

    /// <summary>
    /// Gets the null literal for a specific target language.
    /// </summary>
    /// <param name="targetLanguage">The target language.</param>
    /// <returns>The null literal in the target language.</returns>
    protected virtual string GetNullLiteralForLanguage(string targetLanguage)
    {
        return targetLanguage.ToLowerInvariant() switch
        {
            "csharp" or "c#" => "null",
            "typescript" or "ts" => "null",
            "java" => "null",
            "python" or "py" => "None",
            _ => "null"
        };
    }

    /// <summary>
    /// Determines if a type is a value type in the specified language.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="language">The target language.</param>
    /// <returns>True if the type is a value type; otherwise, false.</returns>
    protected virtual bool IsValueType(string type, string language)
    {
        if (language.ToLowerInvariant() == "csharp" || language.ToLowerInvariant() == "c#")
        {
            var valueTypes = new[] 
            { 
                "int", "long", "short", "byte", "float", "double", "decimal", 
                "bool", "char", "DateTime", "DateTimeOffset", "TimeSpan", "Guid" 
            };
            
            return valueTypes.Contains(type);
        }
        
        return false;
    }

    /// <summary>
    /// Adds a default value for a type in a specific language.
    /// </summary>
    /// <param name="databaseType">The database type.</param>
    /// <param name="targetLanguage">The target language.</param>
    /// <param name="defaultValue">The default value.</param>
    protected virtual void AddDefaultValue(string databaseType, string targetLanguage, string defaultValue)
    {
        if (string.IsNullOrEmpty(databaseType))
        {
            throw new ArgumentNullException(nameof(databaseType));
        }

        if (string.IsNullOrEmpty(targetLanguage))
        {
            throw new ArgumentNullException(nameof(targetLanguage));
        }

        if (!_defaultValues.ContainsKey(targetLanguage))
        {
            _defaultValues[targetLanguage] = new Dictionary<string, string>();
        }

        var normalizedType = NormalizeTypeName(databaseType);
        _defaultValues[targetLanguage][normalizedType] = defaultValue;
    }
}
