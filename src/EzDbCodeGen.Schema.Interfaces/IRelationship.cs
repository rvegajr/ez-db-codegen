namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a relationship between database tables.
/// </summary>
public interface IRelationship
{
    /// <summary>
    /// Gets the source table of the relationship.
    /// </summary>
    ITable SourceTable { get; }
    
    /// <summary>
    /// Gets the target table of the relationship.
    /// </summary>
    ITable TargetTable { get; }
    
    /// <summary>
    /// Gets the type of the relationship.
    /// </summary>
    RelationshipType Type { get; }
    
    /// <summary>
    /// Gets the foreign key that defines the relationship.
    /// </summary>
    IForeignKey? ForeignKey { get; }
    
    /// <summary>
    /// Gets the join table for many-to-many relationships.
    /// </summary>
    ITable? JoinTable { get; }
    
    /// <summary>
    /// Gets the source navigation property name.
    /// </summary>
    string SourceNavigationPropertyName { get; }
    
    /// <summary>
    /// Gets the target navigation property name.
    /// </summary>
    string TargetNavigationPropertyName { get; }
    
    /// <summary>
    /// Gets a value indicating whether the source navigation property is a collection.
    /// </summary>
    bool IsSourceCollection { get; }
    
    /// <summary>
    /// Gets a value indicating whether the target navigation property is a collection.
    /// </summary>
    bool IsTargetCollection { get; }
    
    /// <summary>
    /// Gets the cascade delete behavior for this relationship.
    /// </summary>
    ReferentialAction DeleteBehavior { get; }
    
    /// <summary>
    /// Gets a value indicating whether this is a self-referencing relationship.
    /// </summary>
    bool IsSelfReferencing { get; }
    
    /// <summary>
    /// Gets a value indicating whether this is an inheritance relationship.
    /// </summary>
    bool IsInheritance { get; }
    
    /// <summary>
    /// Gets the inheritance type for inheritance relationships.
    /// </summary>
    InheritanceType? InheritanceType { get; }
    
    /// <summary>
    /// Gets the discriminator column for TPH inheritance relationships.
    /// </summary>
    IColumn? DiscriminatorColumn { get; }
    
    /// <summary>
    /// Gets the discriminator value for the derived table in TPH inheritance relationships.
    /// </summary>
    string? DiscriminatorValue { get; }
}
