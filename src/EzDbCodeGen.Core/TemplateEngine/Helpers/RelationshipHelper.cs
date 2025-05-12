using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;
using EzDbCodeGen.Interfaces.TemplateEngine.Helpers;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Core.TemplateEngine.Helpers
{
    /// <summary>
    /// Implementation of the IRelationshipHelper interface for working with relationships in templates.
    /// This helper enhances code generation with superior relationship handling.
    /// </summary>
    public class RelationshipHelper : IRelationshipHelper
    {
        private readonly ILogger<RelationshipHelper> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipHelper"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public RelationshipHelper(ILogger<RelationshipHelper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetRelationships(object tableObj, string relationshipType = null)
        {
            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get relationships for an object that is not a table.");
                return Enumerable.Empty<object>();
            }

            // We need the database schema to find relationships
            if (!(table.Columns.FirstOrDefault()?.Table as IDatabaseSchema)?.Relationships is IEnumerable<IRelationship> allRelationships)
            {
                _logger.LogWarning("Cannot determine the database schema from the provided table.");
                return Enumerable.Empty<object>();
            }

            // Get all relationships where this table is either the source or target
            var tableRelationships = allRelationships
                .Where(r => r.SourceTable == table || r.TargetTable == table);

            // Filter by relationship type if specified
            if (!string.IsNullOrEmpty(relationshipType))
            {
                if (Enum.TryParse<RelationshipType>(relationshipType, true, out var parsedType))
                {
                    tableRelationships = tableRelationships.Where(r => r.Type == parsedType);
                }
                else
                {
                    _logger.LogWarning($"Invalid relationship type specified: {relationshipType}");
                }
            }

            return tableRelationships.Cast<object>();
        }

        /// <inheritdoc/>
        public string GetNavigationPropertyName(object relationshipObj, object tableObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get navigation property name for an object that is not a relationship.");
                return string.Empty;
            }

            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get navigation property name for an object that is not a table.");
                return string.Empty;
            }

            // Determine if this table is the source or target in the relationship
            if (relationship.SourceTable == table)
            {
                return relationship.SourceNavigationProperty;
            }
            else if (relationship.TargetTable == table)
            {
                return relationship.TargetNavigationProperty;
            }
            else
            {
                _logger.LogWarning("Table is neither the source nor target in the relationship.");
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public bool IsCollectionNavigationProperty(object relationshipObj, object tableObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to check if navigation property is a collection for an object that is not a relationship.");
                return false;
            }

            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to check if navigation property is a collection for an object that is not a table.");
                return false;
            }

            // Determine if this table is the source or target in the relationship
            if (relationship.SourceTable == table)
            {
                return relationship.SourceNavigationPropertyPlural;
            }
            else if (relationship.TargetTable == table)
            {
                return relationship.TargetNavigationPropertyPlural;
            }
            else
            {
                _logger.LogWarning("Table is neither the source nor target in the relationship.");
                return false;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetPayloadProperties(object relationshipObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get payload properties for an object that is not a relationship.");
                return Enumerable.Empty<object>();
            }

            // Only many-to-many relationships can have payload properties
            if (relationship.Type != RelationshipType.ManyToMany || relationship.JunctionTable == null)
            {
                return Enumerable.Empty<object>();
            }

            // Get all foreign key columns in the junction table
            var foreignKeyColumns = relationship.ForeignKeys
                .Where(fk => fk.SourceTable == relationship.JunctionTable)
                .SelectMany(fk => fk.SourceColumns)
                .Distinct()
                .ToHashSet();

            // Return all columns that are not part of any foreign key
            return relationship.JunctionTable.Columns
                .Where(c => !foreignKeyColumns.Contains(c))
                .Cast<object>();
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetRelationshipsWithPayloadProperties(object tableObj)
        {
            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get relationships with payload properties for an object that is not a table.");
                return Enumerable.Empty<object>();
            }

            // Get all many-to-many relationships for this table
            var manyToManyRelationships = GetRelationships(table, "ManyToMany").Cast<IRelationship>();

            // Filter to only those with payload properties
            return manyToManyRelationships
                .Where(r => GetPayloadProperties(r).Any())
                .Cast<object>();
        }

        /// <inheritdoc/>
        public string GetRelationshipDirection(object relationshipObj, object tableObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get relationship direction for an object that is not a relationship.");
                return string.Empty;
            }

            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get relationship direction for an object that is not a table.");
                return string.Empty;
            }

            // Determine if this table is the source or target in the relationship
            if (relationship.SourceTable == table)
            {
                return "Outgoing";
            }
            else if (relationship.TargetTable == table)
            {
                return "Incoming";
            }
            else
            {
                _logger.LogWarning("Table is neither the source nor target in the relationship.");
                return "Unknown";
            }
        }

        /// <inheritdoc/>
        public object GetOtherTable(object relationshipObj, object tableObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get the other table for an object that is not a relationship.");
                return null;
            }

            if (!(tableObj is ITable table))
            {
                _logger.LogWarning("Attempted to get the other table for an object that is not a table.");
                return null;
            }

            // Determine if this table is the source or target in the relationship
            if (relationship.SourceTable == table)
            {
                return relationship.TargetTable;
            }
            else if (relationship.TargetTable == table)
            {
                return relationship.SourceTable;
            }
            else
            {
                _logger.LogWarning("Table is neither the source nor target in the relationship.");
                return null;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<object> GetForeignKeysForRelationship(object relationshipObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get foreign keys for an object that is not a relationship.");
                return Enumerable.Empty<object>();
            }

            return relationship.ForeignKeys.Cast<object>();
        }

        /// <inheritdoc/>
        public object GetJunctionTable(object relationshipObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get junction table for an object that is not a relationship.");
                return null;
            }

            if (relationship.Type != RelationshipType.ManyToMany)
            {
                _logger.LogWarning("Relationship is not a many-to-many relationship and does not have a junction table.");
                return null;
            }

            return relationship.JunctionTable;
        }

        /// <inheritdoc/>
        public bool IsSelfReferencing(object relationshipObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to check if an object that is not a relationship is self-referencing.");
                return false;
            }

            return relationship.IsSelfReferencing;
        }

        /// <inheritdoc/>
        public bool IsInheritanceRelationship(object relationshipObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to check if an object that is not a relationship is an inheritance relationship.");
                return false;
            }

            return relationship.Type == RelationshipType.Inheritance;
        }

        /// <inheritdoc/>
        public string GetInheritanceType(object relationshipObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get inheritance type for an object that is not a relationship.");
                return string.Empty;
            }

            if (relationship.Type != RelationshipType.Inheritance || !relationship.InheritanceType.HasValue)
            {
                _logger.LogWarning("Relationship is not an inheritance relationship or does not have an inheritance type.");
                return string.Empty;
            }

            return relationship.InheritanceType.Value.ToString();
        }

        /// <inheritdoc/>
        public string GetDiscriminatorColumn(object relationshipObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get discriminator column for an object that is not a relationship.");
                return string.Empty;
            }

            if (relationship.Type != RelationshipType.Inheritance || 
                relationship.InheritanceType != InheritanceType.TablePerHierarchy || 
                string.IsNullOrEmpty(relationship.DiscriminatorColumn))
            {
                _logger.LogWarning("Relationship is not a TPH inheritance relationship or does not have a discriminator column.");
                return string.Empty;
            }

            return relationship.DiscriminatorColumn;
        }

        /// <inheritdoc/>
        public string GetDiscriminatorValue(object relationshipObj)
        {
            if (!(relationshipObj is IRelationship relationship))
            {
                _logger.LogWarning("Attempted to get discriminator value for an object that is not a relationship.");
                return string.Empty;
            }

            if (relationship.Type != RelationshipType.Inheritance || 
                relationship.InheritanceType != InheritanceType.TablePerHierarchy || 
                string.IsNullOrEmpty(relationship.DiscriminatorValue))
            {
                _logger.LogWarning("Relationship is not a TPH inheritance relationship or does not have a discriminator value.");
                return string.Empty;
            }

            return relationship.DiscriminatorValue;
        }
    }
}
