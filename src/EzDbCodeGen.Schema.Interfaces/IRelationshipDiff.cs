namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a comparison between two relationships.
/// </summary>
public interface IRelationshipDiff
{
    /// <summary>
    /// Gets the original relationship.
    /// </summary>
    IRelationship Original { get; }
    
    /// <summary>
    /// Gets the new relationship.
    /// </summary>
    IRelationship New { get; }
    
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
}
