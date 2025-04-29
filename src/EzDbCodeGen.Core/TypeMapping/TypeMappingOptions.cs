namespace EzDbCodeGen.Core.TypeMapping;

/// <summary>
/// Options for type mapping.
/// </summary>
public class TypeMappingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to use nullable reference types.
    /// </summary>
    public bool UseNullableReferenceTypes { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use nullable annotations.
    /// </summary>
    public bool UseNullableAnnotations { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to map string columns to non-nullable types.
    /// </summary>
    public bool StringsAreNullable { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use specialized types for spatial data.
    /// </summary>
    public bool UseSpatialTypes { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use specialized types for JSON data.
    /// </summary>
    public bool UseJsonTypes { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use specialized types for XML data.
    /// </summary>
    public bool UseXmlTypes { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include precision and scale for decimal types.
    /// </summary>
    public bool IncludePrecisionAndScale { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include max length for string types.
    /// </summary>
    public bool IncludeMaxLength { get; set; } = true;
    
    /// <summary>
    /// Gets or sets custom type mappings.
    /// </summary>
    public IDictionary<string, string> CustomTypeMappings { get; set; } = new Dictionary<string, string>();
    
    /// <summary>
    /// Creates a new instance of the <see cref="TypeMappingOptions"/> class with default values.
    /// </summary>
    /// <returns>A new instance of the <see cref="TypeMappingOptions"/> class with default values.</returns>
    public static TypeMappingOptions Default => new TypeMappingOptions();
}
