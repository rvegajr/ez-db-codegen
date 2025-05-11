namespace EzDbCodeGen.Common.Interfaces.Schema.Analysis;

/// <summary>
/// Defines the interface for a schema analyzer that analyzes database schemas.
/// </summary>
public interface ISchemaAnalyzer
{
    /// <summary>
    /// Analyzes a database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>The analyzed database schema.</returns>
    IDatabaseSchema Analyze(IDatabaseSchema schema);
}
