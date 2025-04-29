namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Options for relationship detection.
/// </summary>
public class RelationshipDetectionOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to detect one-to-one relationships.
    /// </summary>
    public bool DetectOneToOneRelationships { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to detect one-to-many relationships.
    /// </summary>
    public bool DetectOneToManyRelationships { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to detect many-to-many relationships.
    /// </summary>
    public bool DetectManyToManyRelationships { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to detect self-referencing relationships.
    /// </summary>
    public bool DetectSelfReferencingRelationships { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to detect inheritance relationships.
    /// </summary>
    public bool DetectInheritanceRelationships { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use junction table naming heuristics for many-to-many detection.
    /// </summary>
    public bool UseJunctionTableNamingHeuristics { get; set; } = true;
    
    /// <summary>
    /// Gets or sets common junction table naming patterns.
    /// </summary>
    public ICollection<string> JunctionTablePatterns { get; set; } = new List<string>
    {
        "{Entity1}{Entity2}",
        "{Entity1}To{Entity2}",
        "{Entity1}{Entity2}Junction",
        "{Entity1}{Entity2}Link",
        "{Entity1}{Entity2}Map"
    };
    
    /// <summary>
    /// Gets or sets a value indicating whether to apply intelligent navigation property naming.
    /// </summary>
    public bool UseIntelligentNavigationNaming { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate navigation properties for both sides of relationships.
    /// </summary>
    public bool GenerateBidirectionalNavigationProperties { get; set; } = true;
    
    /// <summary>
    /// Creates a new instance of the <see cref="RelationshipDetectionOptions"/> class with default values.
    /// </summary>
    /// <returns>A new instance of the <see cref="RelationshipDetectionOptions"/> class with default values.</returns>
    public static RelationshipDetectionOptions Default => new RelationshipDetectionOptions();
}
