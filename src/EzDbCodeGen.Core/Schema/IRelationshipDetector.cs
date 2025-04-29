namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a relationship detector that analyzes a database schema for relationships.
/// </summary>
public interface IRelationshipDetector
{
    /// <summary>
    /// Detects relationships in the database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <param name="options">Options for relationship detection.</param>
    /// <returns>A collection of relationships.</returns>
    IEnumerable<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options);
}
