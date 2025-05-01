namespace EzDbCodeGen.Schema.Interfaces.Analysis;

/// <summary>
/// Interface for analyzing relationships between database tables.
/// </summary>
public interface IRelationshipAnalyzer
{
    /// <summary>
    /// Analyzes relationships in a database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A collection of detected relationships.</returns>
    IReadOnlyCollection<IRelationship> AnalyzeRelationships(IDatabaseSchema schema);
}
