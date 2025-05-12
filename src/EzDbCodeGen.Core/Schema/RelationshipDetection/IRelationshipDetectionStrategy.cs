using System.Collections.Generic;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.RelationshipDetection
{
    /// <summary>
    /// Defines a strategy for detecting relationships in a database schema.
    /// This interface supports the strategy pattern for our relationship detection system.
    /// </summary>
    public interface IRelationshipDetectionStrategy
    {
        /// <summary>
        /// Gets the name of the strategy.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the strategy.
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Gets the priority of the strategy. Strategies with lower priority values run first.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Detects relationships in a database schema.
        /// </summary>
        /// <param name="schema">The database schema to detect relationships in.</param>
        /// <param name="options">The options for relationship detection.</param>
        /// <returns>A collection of detected relationships.</returns>
        IEnumerable<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options);
    }
}
