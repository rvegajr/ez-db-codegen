namespace EzDbCodeGen.Schema.Interfaces
{
    /// <summary>
    /// Represents a comparison between two table relationships.
    /// </summary>
    public interface IRelationshipDiff
    {
        /// <summary>
        /// Gets the source relationship.
        /// </summary>
        IRelationship? Source { get; }

        /// <summary>
        /// Gets the target relationship.
        /// </summary>
        IRelationship? Target { get; }

        /// <summary>
        /// Gets the original relationship.
        /// </summary>
        IRelationship? Original { get; }
        
        /// <summary>
        /// Gets the new relationship.
        /// </summary>
        IRelationship? New { get; }
        
        /// <summary>
        /// Gets a value indicating whether the relationship has changed.
        /// </summary>
        bool HasChanged { get; }
        
        /// <summary>
        /// Gets a value indicating whether the type of the relationship has changed.
        /// </summary>
        bool TypeChanged { get; }
        
        /// <summary>
        /// Gets a value indicating whether the source table of the relationship has changed.
        /// </summary>
        bool SourceTableChanged { get; }
        
        /// <summary>
        /// Gets a value indicating whether the target table of the relationship has changed.
        /// </summary>
        bool TargetTableChanged { get; }
        
        /// <summary>
        /// Gets a value indicating whether the source property of the relationship has changed.
        /// </summary>
        bool SourcePropertyChanged { get; }
        
        /// <summary>
        /// Gets a value indicating whether the target property of the relationship has changed.
        /// </summary>
        bool TargetPropertyChanged { get; }
        
        /// <summary>
        /// Gets a value indicating whether the junction table of the relationship has changed.
        /// </summary>
        bool JunctionTableChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the source navigation property name has changed.
        /// </summary>
        bool SourceNavigationPropertyNameChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the target navigation property name has changed.
        /// </summary>
        bool TargetNavigationPropertyNameChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the source collection status has changed.
        /// </summary>
        bool IsSourceCollectionChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the target collection status has changed.
        /// </summary>
        bool IsTargetCollectionChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the delete behavior has changed.
        /// </summary>
        bool DeleteBehaviorChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the inheritance status has changed.
        /// </summary>
        bool InheritanceStatusChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the inheritance type has changed.
        /// </summary>
        bool InheritanceTypeChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the discriminator column has changed.
        /// </summary>
        bool DiscriminatorColumnChanged { get; }

        /// <summary>
        /// Gets a value indicating whether the discriminator value has changed.
        /// </summary>
        bool DiscriminatorValueChanged { get; }

        /// <summary>
        /// Gets a summary of the changes between the relationships.
        /// </summary>
        /// <returns>A string summarizing the changes.</returns>
        string GetSummary();
    }
}
