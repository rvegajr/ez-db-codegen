namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents the type of relationship between two database tables.
/// </summary>
public enum RelationshipType
{
    /// <summary>
    /// Represents a one-to-many relationship.
    /// </summary>
    OneToMany,
    
    /// <summary>
    /// Represents a one-to-one relationship.
    /// </summary>
    OneToOne,
    
    /// <summary>
    /// Represents a many-to-many relationship.
    /// </summary>
    ManyToMany,
    
    /// <summary>
    /// Represents an inheritance relationship.
    /// </summary>
    Inheritance
}
