using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Interface for detecting relationships between database tables.
/// </summary>
public interface IRelationshipDetector
{
    /// <summary>
    /// Detects relationships in a database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <param name="options">Options for relationship detection.</param>
    /// <returns>A collection of detected relationships.</returns>
    IReadOnlyCollection<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options);
}
