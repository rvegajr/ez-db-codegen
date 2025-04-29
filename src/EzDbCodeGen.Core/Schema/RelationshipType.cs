namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents the type of relationship between database tables.
/// </summary>
public enum RelationshipType
{
    /// <summary>
    /// A one-to-one relationship.
    /// </summary>
    OneToOne,
    
    /// <summary>
    /// A one-to-many relationship.
    /// </summary>
    OneToMany,
    
    /// <summary>
    /// A many-to-one relationship.
    /// </summary>
    ManyToOne,
    
    /// <summary>
    /// A many-to-many relationship.
    /// </summary>
    ManyToMany,
    
    /// <summary>
    /// An inheritance relationship.
    /// </summary>
    Inheritance
}
