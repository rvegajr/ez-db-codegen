using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Core.Schema.Models;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.RelationshipDetection
{
    /// <summary>
    /// Implementation of the IRelationshipDetector interface with superior relationship detection capabilities.
    /// This class implements advanced heuristics to detect relationships that EF Core can't identify.
    /// </summary>
    public class RelationshipDetector : IRelationshipDetector
    {
        private readonly IEnumerable<IRelationshipDetectionStrategy> _strategies;
        private readonly IStringUtility _stringUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipDetector"/> class.
        /// </summary>
        /// <param name="strategies">The relationship detection strategies to use.</param>
        /// <param name="stringUtility">The string utility for naming relationships and navigation properties.</param>
        public RelationshipDetector(
            IEnumerable<IRelationshipDetectionStrategy> strategies,
            IStringUtility stringUtility)
        {
            _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
            _stringUtility = stringUtility ?? throw new ArgumentNullException(nameof(stringUtility));
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options)
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

            // Execute each strategy in order
            foreach (var strategy in _strategies)
            {
                var strategyRelationships = strategy.DetectRelationships(schema, options);
                relationships.AddRange(strategyRelationships);
            }

            // Post-process the relationships to assign navigation property names
            ProcessNavigationPropertyNames(relationships);

            return relationships.AsReadOnly();
        }

        /// <summary>
        /// Processes navigation property names for the detected relationships.
        /// This is where we apply our superior naming conventions that are more semantic than EF Core.
        /// </summary>
        /// <param name="relationships">The relationships to process.</param>
        private void ProcessNavigationPropertyNames(List<IRelationship> relationships)
        {
            foreach (var relationship in relationships)
            {
                // Skip if navigation properties are already set
                if (!string.IsNullOrEmpty(relationship.SourceNavigationProperty) &&
                    !string.IsNullOrEmpty(relationship.TargetNavigationProperty))
                {
                    continue;
                }

                switch (relationship.Type)
                {
                    case RelationshipType.OneToMany:
                        GenerateOneToManyNavigationNames(relationship);
                        break;

                    case RelationshipType.OneToOne:
                        GenerateOneToOneNavigationNames(relationship);
                        break;

                    case RelationshipType.ManyToMany:
                        GenerateManyToManyNavigationNames(relationship);
                        break;

                    case RelationshipType.Inheritance:
                        GenerateInheritanceNavigationNames(relationship);
                        break;
                }
            }

            // Resolve any name collisions
            ResolveNavigationNameCollisions(relationships);
        }

        /// <summary>
        /// Generates navigation property names for a one-to-many relationship.
        /// </summary>
        /// <param name="relationship">The relationship to process.</param>
        private void GenerateOneToManyNavigationNames(IRelationship relationship)
        {
            // For the "one" side, we use plural of the "many" table name
            var manyTableName = relationship.TargetTable.Name;
            relationship.SourceNavigationProperty = _stringUtility.ToPlural(manyTableName);
            relationship.SourceNavigationPropertyPlural = true;

            // For the "many" side, we use singular of the "one" table name
            var oneTableName = relationship.SourceTable.Name;
            relationship.TargetNavigationProperty = _stringUtility.ToSingular(oneTableName);
            relationship.TargetNavigationPropertyPlural = false;

            // Check for self-referencing relationship
            if (relationship.IsSelfReferencing)
            {
                // We need more specific names for self-references
                if (relationship.ForeignKeys.Any())
                {
                    var firstFk = relationship.ForeignKeys.First();
                    var fkCol = firstFk.SourceColumns.First();
                    var prefix = fkCol.Name.Replace("ID", "").Replace("Id", "");
                    
                    relationship.SourceNavigationProperty = _stringUtility.ToPlural(prefix);
                    relationship.TargetNavigationProperty = prefix + "Of";
                }
                else
                {
                    relationship.SourceNavigationProperty = "Children";
                    relationship.TargetNavigationProperty = "Parent";
                }
            }
        }

        /// <summary>
        /// Generates navigation property names for a one-to-one relationship.
        /// </summary>
        /// <param name="relationship">The relationship to process.</param>
        private void GenerateOneToOneNavigationNames(IRelationship relationship)
        {
            // For one-to-one, we use the singular table name on both sides
            relationship.SourceNavigationProperty = _stringUtility.ToSingular(relationship.TargetTable.Name);
            relationship.SourceNavigationPropertyPlural = false;
            
            relationship.TargetNavigationProperty = _stringUtility.ToSingular(relationship.SourceTable.Name);
            relationship.TargetNavigationPropertyPlural = false;

            // For self-referencing one-to-one, we need more specific names
            if (relationship.IsSelfReferencing)
            {
                if (relationship.ForeignKeys.Any())
                {
                    var firstFk = relationship.ForeignKeys.First();
                    var fkCol = firstFk.SourceColumns.First();
                    var prefix = fkCol.Name.Replace("ID", "").Replace("Id", "");
                    
                    relationship.SourceNavigationProperty = prefix;
                    relationship.TargetNavigationProperty = "InverseOf" + prefix;
                }
                else
                {
                    relationship.SourceNavigationProperty = "Related";
                    relationship.TargetNavigationProperty = "InverseRelated";
                }
            }
        }

        /// <summary>
        /// Generates navigation property names for a many-to-many relationship.
        /// </summary>
        /// <param name="relationship">The relationship to process.</param>
        private void GenerateManyToManyNavigationNames(IRelationship relationship)
        {
            // For many-to-many, we use plural table names on both sides
            relationship.SourceNavigationProperty = _stringUtility.ToPlural(relationship.TargetTable.Name);
            relationship.SourceNavigationPropertyPlural = true;
            
            relationship.TargetNavigationProperty = _stringUtility.ToPlural(relationship.SourceTable.Name);
            relationship.TargetNavigationPropertyPlural = true;

            // Check for payload properties in the junction table
            var payloadColumns = relationship.GetPayloadColumns?.Invoke();
            if (payloadColumns != null && payloadColumns.Any())
            {
                // If we have payload columns, use a more descriptive name that indicates the relationship type
                var junctionName = relationship.JunctionTable?.Name;
                if (!string.IsNullOrEmpty(junctionName))
                {
                    // Try to derive meaningful names from the junction table name
                    // Example: If junction is "StudentCourseGrade", we have Student-Course with Grade payload
                    junctionName = junctionName
                        .Replace(relationship.SourceTable.Name, "")
                        .Replace(relationship.TargetTable.Name, "");
                    
                    if (!string.IsNullOrEmpty(junctionName))
                    {
                        relationship.SourceNavigationProperty = junctionName + _stringUtility.ToPlural(relationship.TargetTable.Name);
                        relationship.TargetNavigationProperty = junctionName + _stringUtility.ToPlural(relationship.SourceTable.Name);
                    }
                }
            }

            // For self-referencing many-to-many, we need more specific names
            if (relationship.IsSelfReferencing)
            {
                // Try to infer the relationship type from the junction table or foreign key names
                string relationship1 = "Related";
                string relationship2 = "InverseRelated";
                
                if (relationship.JunctionTable != null)
                {
                    var junction = relationship.JunctionTable;
                    var fkColumns = junction.ForeignKeys
                        .SelectMany(fk => fk.SourceColumns)
                        .ToList();
                    
                    if (fkColumns.Count >= 2)
                    {
                        relationship1 = fkColumns[0].Name.Replace("ID", "").Replace("Id", "");
                        relationship2 = fkColumns[1].Name.Replace("ID", "").Replace("Id", "");
                    }
                }
                
                relationship.SourceNavigationProperty = _stringUtility.ToPlural(relationship1);
                relationship.TargetNavigationProperty = _stringUtility.ToPlural(relationship2);
            }
        }

        /// <summary>
        /// Generates navigation property names for an inheritance relationship.
        /// </summary>
        /// <param name="relationship">The relationship to process.</param>
        private void GenerateInheritanceNavigationNames(IRelationship relationship)
        {
            // For inheritance, typically the base class doesn't have a navigation property to derived classes
            // But derived classes have a navigation property to the base class
            relationship.SourceNavigationProperty = string.Empty; // Base class doesn't navigate to derived
            relationship.SourceNavigationPropertyPlural = false;
            
            // Derived class navigates to base class
            relationship.TargetNavigationProperty = _stringUtility.ToSingular(relationship.SourceTable.Name);
            relationship.TargetNavigationPropertyPlural = false;
        }

        /// <summary>
        /// Resolves navigation property name collisions within the entity.
        /// </summary>
        /// <param name="relationships">The relationships to process.</param>
        private void ResolveNavigationNameCollisions(List<IRelationship> relationships)
        {
            // Group relationships by source table to check for collisions within the same entity
            var relationshipsBySourceTable = relationships.GroupBy(r => r.SourceTable);
            
            foreach (var tableGroup in relationshipsBySourceTable)
            {
                ResolveDuplicateNames(tableGroup, r => r.SourceNavigationProperty, (r, name) => r.SourceNavigationProperty = name);
            }
            
            // Group relationships by target table to check for collisions within the same entity
            var relationshipsByTargetTable = relationships.GroupBy(r => r.TargetTable);
            
            foreach (var tableGroup in relationshipsByTargetTable)
            {
                ResolveDuplicateNames(tableGroup, r => r.TargetNavigationProperty, (r, name) => r.TargetNavigationProperty = name);
            }
        }

        /// <summary>
        /// Resolves duplicate names within a group of relationships.
        /// </summary>
        /// <param name="relationships">The relationships to process.</param>
        /// <param name="nameSelector">A function to select the name to check for duplicates.</param>
        /// <param name="nameSetter">A function to set the updated name.</param>
        private void ResolveDuplicateNames(
            IEnumerable<IRelationship> relationships,
            Func<IRelationship, string> nameSelector,
            Action<IRelationship, string> nameSetter)
        {
            // Group by navigation property name to find duplicates
            var groupsByName = relationships
                .Where(r => !string.IsNullOrEmpty(nameSelector(r)))
                .GroupBy(r => nameSelector(r), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1);
            
            foreach (var group in groupsByName)
            {
                var duplicates = group.ToList();
                
                // Skip the first one (keep the original name)
                for (int i = 1; i < duplicates.Count; i++)
                {
                    var relationship = duplicates[i];
                    var currentName = nameSelector(relationship);
                    
                    // Try to make the name more specific based on the other side of the relationship
                    string specificName;
                    
                    if (relationship.Type == RelationshipType.OneToMany || relationship.Type == RelationshipType.ManyToMany)
                    {
                        // For collections, try to be more specific about what kind of collection it is
                        string otherTableName;
                        if (nameSelector == (r => r.SourceNavigationProperty))
                        {
                            otherTableName = relationship.TargetTable.Name;
                        }
                        else
                        {
                            otherTableName = relationship.SourceTable.Name;
                        }
                        
                        // Try to find a more specific name using the foreign key names if available
                        if (relationship.ForeignKeys.Any())
                        {
                            var firstFk = relationship.ForeignKeys.First();
                            var fkCol = firstFk.SourceColumns.First();
                            var prefix = fkCol.Name.Replace("ID", "").Replace("Id", "");
                            
                            if (!string.IsNullOrEmpty(prefix) && !prefix.Equals(otherTableName, StringComparison.OrdinalIgnoreCase))
                            {
                                specificName = prefix + _stringUtility.ToPlural(otherTableName);
                            }
                            else
                            {
                                // Fall back to using the relationship name
                                specificName = currentName + i;
                            }
                        }
                        else
                        {
                            // Fall back to using the relationship name
                            specificName = currentName + i;
                        }
                    }
                    else
                    {
                        // For non-collections, just add a suffix with an index
                        specificName = currentName + i;
                    }
                    
                    nameSetter(relationship, specificName);
                }
            }
        }
    }
}
