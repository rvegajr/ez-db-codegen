using System.Collections.Generic;
using System.Threading.Tasks;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines an interface for detecting relationships in a database schema.
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

        /// <summary>
        /// Asynchronously detects relationships in a database schema.
        /// </summary>
        /// <param name="schema">The database schema to analyze.</param>
        /// <param name="options">Options for relationship detection.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of detected relationships.</returns>
        Task<IReadOnlyCollection<IRelationship>> DetectRelationshipsAsync(IDatabaseSchema schema, RelationshipDetectionOptions options);
    }
}
