using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Core.Schema.Models;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.RelationshipDetection
{
    /// <summary>
    /// A strategy for detecting relationships based on foreign keys.
    /// This strategy detects one-to-many and one-to-one relationships based on foreign key constraints.
    /// </summary>
    public class ForeignKeyRelationshipStrategy : IRelationshipDetectionStrategy
    {
        /// <inheritdoc/>
        public string Name => "Foreign Key Strategy";

        /// <inheritdoc/>
        public string Description => "Detects one-to-many and one-to-one relationships based on foreign key constraints.";

        /// <inheritdoc/>
        public int Priority => 10; // Run this strategy first

        /// <inheritdoc/>
        public IEnumerable<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var relationships = new List<IRelationship>();

            foreach (var table in schema.Tables)
            {
                foreach (var foreignKey in table.ForeignKeys)
                {
                    // Skip self-referencing foreign keys if not enabled
                    bool isSelfReference = foreignKey.SourceTable == foreignKey.ReferencedTable;
                    if (isSelfReference && !options.DetectSelfReferencingRelationships)
                    {
                        continue;
                    }

                    // Determine the relationship type based on the foreign key structure
                    var relationshipType = DetermineRelationshipType(foreignKey);

                    // Create a relationship 
                    var relationship = new Relationship(
                        GenerateRelationshipName(foreignKey, relationshipType),
                        relationshipType,
                        foreignKey.ReferencedTable, // "One" side (parent)
                        foreignKey.SourceTable      // "Many" side (child)
                    );

                    // Add the foreign key to the relationship
                    relationship.AddForeignKey(foreignKey);

                    // Check if the relationship is identifying (FK is part of PK)
                    relationship.IsIdentifying = foreignKey.SourceColumns.All(c => c.IsPrimaryKey);

                    relationships.Add(relationship);
                }
            }

            return relationships;
        }

        /// <summary>
        /// Determines the type of relationship based on the foreign key structure.
        /// </summary>
        /// <param name="foreignKey">The foreign key to analyze.</param>
        /// <returns>The type of relationship.</returns>
        private RelationshipType DetermineRelationshipType(IForeignKey foreignKey)
        {
            // If the foreign key is unique, it's a one-to-one relationship
            bool isUnique = IsUniqueForeignKey(foreignKey);

            return isUnique ? RelationshipType.OneToOne : RelationshipType.OneToMany;
        }

        /// <summary>
        /// Determines if a foreign key represents a unique constraint (indicating a one-to-one relationship).
        /// </summary>
        /// <param name="foreignKey">The foreign key to check.</param>
        /// <returns>True if the foreign key represents a unique constraint; otherwise, false.</returns>
        private bool IsUniqueForeignKey(IForeignKey foreignKey)
        {
            var sourceTable = foreignKey.SourceTable;
            var sourceColumns = foreignKey.SourceColumns.ToList();

            // Check if all FK columns are part of the primary key
            bool isPrimaryKey = sourceColumns.All(c => c.IsPrimaryKey) && 
                                sourceColumns.Count == sourceTable.PrimaryKeyColumns.Count;
            
            if (isPrimaryKey)
            {
                return true;
            }

            // Check if the FK columns are part of a unique constraint or unique index
            bool isUniqueConstraint = sourceTable.UniqueConstraints.Any(uc => 
                uc.Columns.Count == sourceColumns.Count && 
                sourceColumns.All(sc => uc.Columns.Contains(sc)));

            if (isUniqueConstraint)
            {
                return true;
            }

            // Check for unique indexes
            bool isUniqueIndex = sourceTable.Indexes.Any(idx => 
                idx.IsUnique && 
                idx.Columns.Count == sourceColumns.Count && 
                sourceColumns.All(sc => idx.Columns.Contains(sc)));

            return isUniqueIndex;
        }

        /// <summary>
        /// Generates a name for the relationship based on the foreign key and relationship type.
        /// </summary>
        /// <param name="foreignKey">The foreign key that defines the relationship.</param>
        /// <param name="relationshipType">The type of the relationship.</param>
        /// <returns>A name for the relationship.</returns>
        private string GenerateRelationshipName(IForeignKey foreignKey, RelationshipType relationshipType)
        {
            string prefix = relationshipType == RelationshipType.OneToOne ? "OneToOne" : "OneToMany";
            string sourceName = foreignKey.SourceTable.Name;
            string referencedName = foreignKey.ReferencedTable.Name;
            
            // For self-references, add more context
            if (sourceName == referencedName)
            {
                // Try to get meaningful context from the foreign key name or columns
                string contextName = "Self";
                
                if (!string.IsNullOrEmpty(foreignKey.Name) && 
                    !foreignKey.Name.Equals($"FK_{sourceName}_{referencedName}", StringComparison.OrdinalIgnoreCase))
                {
                    // Try to extract meaningful parts from the FK name
                    var nameParts = foreignKey.Name.Split('_');
                    if (nameParts.Length > 2)
                    {
                        contextName = nameParts[nameParts.Length - 1];
                    }
                }
                else if (foreignKey.SourceColumns.Count > 0)
                {
                    // Try to use the column name for context
                    var firstCol = foreignKey.SourceColumns.First();
                    contextName = firstCol.Name.Replace("ID", "").Replace("Id", "");
                }
                
                return $"{prefix}_{sourceName}_{contextName}";
            }
            
            return $"{prefix}_{referencedName}_{sourceName}";
        }
    }
}
