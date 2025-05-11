using System.Collections.Generic;

namespace EzDbCodeGen.Common.Interfaces.Schema.Analysis;

/// <summary>
/// Defines the interface for a relationship analyzer that analyzes relationships between tables in a database schema.
/// </summary>
public interface IRelationshipAnalyzer
{
    /// <summary>
    /// Analyzes the relationships between tables in a database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>The analyzed schema with relationships.</returns>
    IDatabaseSchema AnalyzeRelationships(IDatabaseSchema schema);
    
    /// <summary>
    /// Detects inheritance relationships between tables in a database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A dictionary mapping child tables to their parent tables.</returns>
    IDictionary<ITable, ITable> DetectInheritance(IDatabaseSchema schema);
}
