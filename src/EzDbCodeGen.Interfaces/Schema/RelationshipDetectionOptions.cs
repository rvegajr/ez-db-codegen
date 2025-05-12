namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines options for relationship detection.
    /// </summary>
    public class RelationshipDetectionOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to detect one-to-many relationships.
        /// </summary>
        public bool DetectOneToManyRelationships { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect one-to-one relationships.
        /// </summary>
        public bool DetectOneToOneRelationships { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect many-to-many relationships.
        /// </summary>
        public bool DetectManyToManyRelationships { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect inheritance relationships.
        /// </summary>
        public bool DetectInheritanceRelationships { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use intelligent naming for navigation properties.
        /// </summary>
        public bool UseIntelligentNavigationNaming { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect self-referencing relationships.
        /// </summary>
        public bool DetectSelfReferencingRelationships { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect relationships between tables in different schemas.
        /// </summary>
        public bool DetectCrossSchemaRelationships { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect TPH inheritance pattern.
        /// </summary>
        public bool DetectTPHInheritance { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect TPT inheritance pattern.
        /// </summary>
        public bool DetectTPTInheritance { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to detect payload columns in many-to-many relationships.
        /// </summary>
        public bool DetectPayloadColumns { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of payload columns allowed for a table to still be considered as a junction table.
        /// </summary>
        public int MaxPayloadColumnsForJunctionTable { get; set; } = 5;

        /// <summary>
        /// Gets or sets a value indicating whether to apply heuristics for better detection.
        /// </summary>
        public bool ApplyDetectionHeuristics { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use case-insensitive name matching.
        /// </summary>
        public bool UseCaseInsensitiveMatching { get; set; } = true;

        /// <summary>
        /// Creates a new instance of RelationshipDetectionOptions with default settings.
        /// </summary>
        /// <returns>A RelationshipDetectionOptions instance with all detection options enabled.</returns>
        public static RelationshipDetectionOptions Default() => new RelationshipDetectionOptions();

        /// <summary>
        /// Creates a new instance of RelationshipDetectionOptions with minimal settings (one-to-many only).
        /// </summary>
        /// <returns>A RelationshipDetectionOptions instance with only one-to-many detection enabled.</returns>
        public static RelationshipDetectionOptions Minimal() => new RelationshipDetectionOptions
        {
            DetectOneToManyRelationships = true,
            DetectOneToOneRelationships = false,
            DetectManyToManyRelationships = false,
            DetectInheritanceRelationships = false,
            UseIntelligentNavigationNaming = false,
            DetectPayloadColumns = false,
            ApplyDetectionHeuristics = false
        };

        /// <summary>
        /// Creates a new instance of RelationshipDetectionOptions with settings optimized for performance.
        /// </summary>
        /// <returns>A RelationshipDetectionOptions instance with performance-optimized settings.</returns>
        public static RelationshipDetectionOptions PerformanceOptimized() => new RelationshipDetectionOptions
        {
            DetectOneToManyRelationships = true,
            DetectOneToOneRelationships = true,
            DetectManyToManyRelationships = true,
            DetectInheritanceRelationships = false,
            UseIntelligentNavigationNaming = true,
            DetectPayloadColumns = false,
            ApplyDetectionHeuristics = false
        };

        /// <summary>
        /// Creates a new instance of RelationshipDetectionOptions with settings optimized for accuracy.
        /// </summary>
        /// <returns>A RelationshipDetectionOptions instance with accuracy-optimized settings.</returns>
        public static RelationshipDetectionOptions AccuracyOptimized() => new RelationshipDetectionOptions
        {
            DetectOneToManyRelationships = true,
            DetectOneToOneRelationships = true,
            DetectManyToManyRelationships = true,
            DetectInheritanceRelationships = true,
            UseIntelligentNavigationNaming = true,
            DetectPayloadColumns = true,
            ApplyDetectionHeuristics = true,
            MaxPayloadColumnsForJunctionTable = 10
        };
    }
}
