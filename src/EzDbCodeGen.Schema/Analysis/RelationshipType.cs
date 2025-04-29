namespace EzDbCodeGen.Schema.Analysis;

/// <summary>
/// Represents the type of relationship between database objects.
/// </summary>
public enum RelationshipType
{
    /// <summary>
    /// A one-to-many relationship.
    /// </summary>
    OneToMany,
    
    /// <summary>
    /// A one-to-one relationship.
    /// </summary>
    OneToOne,
    
    /// <summary>
    /// A many-to-many relationship.
    /// </summary>
    ManyToMany,
    
    /// <summary>
    /// A self-referencing relationship.
    /// </summary>
    SelfReferencing,
    
    /// <summary>
    /// A table-per-hierarchy inheritance relationship.
    /// </summary>
    TablePerHierarchy,
    
    /// <summary>
    /// A table-per-type inheritance relationship.
    /// </summary>
    TablePerType
}
