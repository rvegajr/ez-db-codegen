using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IRelationship interface representing a relationship between database tables.
    /// This is a key component of our superior relationship detection capabilities.
    /// </summary>
    public class Relationship : IRelationship
    {
        private readonly List<IForeignKey> _foreignKeys = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Relationship"/> class.
        /// </summary>
        /// <param name="name">The name of the relationship.</param>
        /// <param name="type">The type of the relationship.</param>
        /// <param name="sourceTable">The source table in the relationship.</param>
        /// <param name="targetTable">The target table in the relationship.</param>
        public Relationship(string name, RelationshipType type, ITable sourceTable, ITable targetTable)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            SourceTable = sourceTable ?? throw new ArgumentNullException(nameof(sourceTable));
            TargetTable = targetTable ?? throw new ArgumentNullException(nameof(targetTable));
            SourceNavigationProperty = string.Empty;
            TargetNavigationProperty = string.Empty;
            SourceNavigationPropertyPlural = false;
            TargetNavigationPropertyPlural = false;
            IsSelfReferencing = sourceTable == targetTable;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public RelationshipType Type { get; }

        /// <inheritdoc/>
        public ITable SourceTable { get; }

        /// <inheritdoc/>
        public ITable TargetTable { get; }

        /// <inheritdoc/>
        public ITable? JunctionTable { get; set; }

        /// <inheritdoc/>
        public bool IsSelfReferencing { get; }

        /// <inheritdoc/>
        public string SourceNavigationProperty { get; set; }

        /// <inheritdoc/>
        public string TargetNavigationProperty { get; set; }

        /// <inheritdoc/>
        public bool SourceNavigationPropertyPlural { get; set; }

        /// <inheritdoc/>
        public bool TargetNavigationPropertyPlural { get; set; }

        /// <inheritdoc/>
        public bool IsIdentifying { get; set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IForeignKey> ForeignKeys => _foreignKeys.AsReadOnly();

        /// <inheritdoc/>
        public InheritanceType? InheritanceType { get; set; }

        /// <inheritdoc/>
        public string? DiscriminatorColumn { get; set; }

        /// <inheritdoc/>
        public string? DiscriminatorValue { get; set; }

        /// <summary>
        /// Adds a foreign key to the relationship.
        /// </summary>
        /// <param name="foreignKey">The foreign key to add.</param>
        public void AddForeignKey(IForeignKey foreignKey)
        {
            if (foreignKey == null)
            {
                throw new ArgumentNullException(nameof(foreignKey));
            }

            // Verify that the foreign key connects the source and target tables
            bool isValidSourceToTarget = foreignKey.SourceTable == SourceTable && foreignKey.ReferencedTable == TargetTable;
            bool isValidTargetToSource = foreignKey.SourceTable == TargetTable && foreignKey.ReferencedTable == SourceTable;
            bool isValidJunction = JunctionTable != null && 
                ((foreignKey.SourceTable == JunctionTable && (foreignKey.ReferencedTable == SourceTable || foreignKey.ReferencedTable == TargetTable)) ||
                (foreignKey.ReferencedTable == JunctionTable && (foreignKey.SourceTable == SourceTable || foreignKey.SourceTable == TargetTable)));

            if (!isValidSourceToTarget && !isValidTargetToSource && !isValidJunction)
            {
                throw new ArgumentException($"Foreign key '{foreignKey.Name}' does not connect the source table '{SourceTable.FullName}' and target table '{TargetTable.FullName}'.", nameof(foreignKey));
            }

            if (_foreignKeys.Any(fk => fk.Name == foreignKey.Name))
            {
                throw new InvalidOperationException($"Foreign key '{foreignKey.Name}' is already part of the relationship '{Name}'.");
            }

            _foreignKeys.Add(foreignKey);
        }
        
        /// <summary>
        /// Gets a collection of payload columns for a many-to-many relationship.
        /// Payload columns are columns in the junction table that are not part of the foreign keys.
        /// </summary>
        /// <returns>A collection of payload columns.</returns>
        public IEnumerable<IColumn> GetPayloadColumns()
        {
            if (Type != RelationshipType.ManyToMany || JunctionTable == null)
            {
                return Enumerable.Empty<IColumn>();
            }

            // Get all columns that are part of foreign keys
            var fkColumns = _foreignKeys
                .Where(fk => fk.SourceTable == JunctionTable || fk.ReferencedTable == JunctionTable)
                .SelectMany(fk => 
                {
                    if (fk.SourceTable == JunctionTable)
                    {
                        return fk.SourceColumns;
                    }
                    else
                    {
                        return fk.ReferencedColumns;
                    }
                })
                .Distinct();

            // Return all columns that are not part of foreign keys
            return JunctionTable.Columns.Except(fkColumns);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string typeStr = Type.ToString();
            string junctionStr = JunctionTable != null ? $" via {JunctionTable.FullName}" : string.Empty;
            string inheritanceStr = InheritanceType.HasValue ? $" ({InheritanceType})" : string.Empty;
            
            return $"{Name}: {SourceTable.FullName} {typeStr} {TargetTable.FullName}{junctionStr}{inheritanceStr}";
        }
    }
}
