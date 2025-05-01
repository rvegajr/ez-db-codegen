namespace EzDbCodeGen.Schema.Interfaces
{
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
        /// Gets or sets a value indicating whether to detect inheritance relationships.
        /// </summary>
        public bool DetectInheritanceRelationships { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the naming convention for join tables in many-to-many relationships.
        /// </summary>
        public string JoinTableNamingPattern { get; set; } = "{Table1}_{Table2}";
        
        /// <summary>
        /// Gets or sets a value indicating whether to detect relationships using naming conventions.
        /// </summary>
        public bool UseNamingConventions { get; set; } = true;
        
        /// <summary>
        /// Gets or sets a value indicating whether to detect relationships using foreign key constraints.
        /// </summary>
        public bool UseForeignKeyConstraints { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use intelligent navigation property naming.
        /// </summary>
        public bool UseIntelligentNavigationNaming { get; set; } = true;

        // Legacy properties for backward compatibility
        
        /// <summary>
        /// Gets or sets a value indicating whether to detect one-to-one relationships.
        /// </summary>
        public bool DetectOneToOne
        {
            get => DetectOneToOneRelationships;
            set => DetectOneToOneRelationships = value;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to detect many-to-many relationships.
        /// </summary>
        public bool DetectManyToMany
        {
            get => DetectManyToManyRelationships;
            set => DetectManyToManyRelationships = value;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to detect inheritance relationships.
        /// </summary>
        public bool DetectInheritance
        {
            get => DetectInheritanceRelationships;
            set => DetectInheritanceRelationships = value;
        }
    }
}
