namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents the type of inheritance strategy used in an inheritance relationship.
/// </summary>
public enum InheritanceType
{
    /// <summary>
    /// Table-Per-Hierarchy (TPH) inheritance, where all types in a hierarchy are mapped to a single table.
    /// </summary>
    TablePerHierarchy,
    
    /// <summary>
    /// Table-Per-Type (TPT) inheritance, where each type in a hierarchy is mapped to its own table.
    /// </summary>
    TablePerType,
    
    /// <summary>
    /// Table-Per-Concrete-Class (TPC) inheritance, where each concrete class is mapped to its own table.
    /// </summary>
    TablePerConcreteClass
}
