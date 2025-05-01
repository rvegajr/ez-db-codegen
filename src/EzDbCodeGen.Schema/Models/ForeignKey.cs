using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a foreign key constraint in a database table.
    /// </summary>
    public class ForeignKey : IForeignKey
    {
        private readonly List<IColumn> _columns = new();
        private readonly List<IColumn> _referencedColumns = new();
        private readonly List<IForeignKeyColumnMapping> _columnMappings = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey"/> class.
        /// </summary>
        /// <param name="name">The name of the foreign key.</param>
        /// <param name="table">The table that the foreign key belongs to.</param>
        /// <param name="referencedTable">The referenced table.</param>
        /// <param name="deleteAction">The delete action.</param>
        /// <param name="updateAction">The update action.</param>
        public ForeignKey(
            string name,
            ITable table,
            ITable referencedTable,
            ReferentialAction deleteAction = ReferentialAction.NoAction,
            ReferentialAction updateAction = ReferentialAction.NoAction)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Table = table ?? throw new ArgumentNullException(nameof(table));
            ReferencedTable = referencedTable ?? throw new ArgumentNullException(nameof(referencedTable));
            DeleteAction = deleteAction;
            UpdateAction = updateAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey"/> class with specified columns.
        /// </summary>
        /// <param name="name">The name of the foreign key.</param>
        /// <param name="table">The table that the foreign key belongs to.</param>
        /// <param name="columns">The columns that make up the foreign key.</param>
        /// <param name="referencedTable">The referenced table.</param>
        /// <param name="referencedColumns">The referenced columns.</param>
        /// <param name="deleteAction">The delete action.</param>
        /// <param name="updateAction">The update action.</param>
        public ForeignKey(
            string name,
            ITable table,
            IEnumerable<IColumn> columns,
            ITable referencedTable,
            IEnumerable<IColumn> referencedColumns,
            ReferentialAction deleteAction = ReferentialAction.NoAction,
            ReferentialAction updateAction = ReferentialAction.NoAction)
            : this(name, table, referencedTable, deleteAction, updateAction)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            if (referencedColumns == null)
            {
                throw new ArgumentNullException(nameof(referencedColumns));
            }

            var columnList = columns.ToList();
            var referencedColumnList = referencedColumns.ToList();

            if (columnList.Count != referencedColumnList.Count)
            {
                throw new ArgumentException("The number of columns must match the number of referenced columns.");
            }

            for (int i = 0; i < columnList.Count; i++)
            {
                AddColumnPair(columnList[i], referencedColumnList[i]);
            }
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public ITable Table { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IColumn> Columns => _columns.AsReadOnly();

        /// <inheritdoc/>
        public ITable ReferencedTable { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IColumn> ReferencedColumns => _referencedColumns.AsReadOnly();

        /// <inheritdoc/>
        public ReferentialAction DeleteAction { get; }

        /// <inheritdoc/>
        public ReferentialAction UpdateAction { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IForeignKeyColumnMapping> ColumnMappings => _columnMappings.AsReadOnly();

        /// <inheritdoc/>
        public string DeleteBehavior => DeleteAction.ToString();

        /// <inheritdoc/>
        public string UpdateBehavior => UpdateAction.ToString();

        /// <summary>
        /// Gets a value indicating whether this is a composite foreign key (contains multiple columns).
        /// </summary>
        public bool IsCompositeKey => Columns.Count > 1;

        /// <summary>
        /// Gets a value indicating whether this foreign key references a primary key.
        /// </summary>
        public bool ReferencesPrimaryKey
        {
            get
            {
                if (ReferencedTable.PrimaryKey == null)
                {
                    return false;
                }

                var pkColumns = ReferencedTable.PrimaryKey.Columns;
                
                // Check if the referenced columns match the primary key columns
                if (ReferencedColumns.Count != pkColumns.Count)
                {
                    return false;
                }

                return ReferencedColumns.All(rc => 
                    pkColumns.Any(pkc => string.Equals(pkc.Name, rc.Name, StringComparison.OrdinalIgnoreCase)));
            }
        }

        /// <summary>
        /// Adds a column to the foreign key.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(IColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            _columns.Add(column);
            UpdateColumnMappings();
        }

        /// <summary>
        /// Adds a referenced column to the foreign key.
        /// </summary>
        /// <param name="column">The referenced column to add.</param>
        public void AddReferencedColumn(IColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            _referencedColumns.Add(column);
            UpdateColumnMappings();
        }

        /// <summary>
        /// Adds a column pair to the foreign key.
        /// </summary>
        /// <param name="column">The column in the foreign key table.</param>
        /// <param name="referencedColumn">The referenced column in the referenced table.</param>
        public void AddColumnPair(IColumn column, IColumn referencedColumn)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            if (referencedColumn == null)
            {
                throw new ArgumentNullException(nameof(referencedColumn));
            }

            _columns.Add(column);
            _referencedColumns.Add(referencedColumn);
            _columnMappings.Add(new ForeignKeyColumnMapping(column, referencedColumn));
        }

        /// <summary>
        /// Determines the relationship type represented by this foreign key.
        /// </summary>
        /// <returns>The relationship type (OneToOne, OneToMany, ManyToMany, or SelfReferencing).</returns>
        public RelationshipType DetermineRelationshipType()
        {
            // Check for self-referencing relationship
            if (Table == ReferencedTable)
            {
                return RelationshipType.SelfReferencing;
            }

            // Check for many-to-many relationship
            if (IsManyToManyJunctionTable())
            {
                return RelationshipType.ManyToMany;
            }

            // Check for one-to-one relationship
            if (IsOneToOne())
            {
                return RelationshipType.OneToOne;
            }

            // Default to one-to-many
            return RelationshipType.OneToMany;
        }

        /// <summary>
        /// Determines if this foreign key represents a one-to-one relationship.
        /// </summary>
        /// <returns>True if this is a one-to-one relationship; otherwise, false.</returns>
        public bool IsOneToOne()
        {
            // A one-to-one relationship exists if the foreign key columns form a unique constraint
            // or if the foreign key is also the primary key
            if (IsColumnSetUnique(Columns, Table))
            {
                return true;
            }

            // Also check if this is part of the primary key
            if (Table.PrimaryKey != null && Columns.All(c => Table.PrimaryKey.Columns.Contains(c)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if this foreign key represents a many-to-many relationship through a junction table.
        /// </summary>
        /// <returns>True if this represents a junction table in a many-to-many relationship; otherwise, false.</returns>
        public bool IsManyToManyJunctionTable()
        {
            // A junction table typically has:
            // 1. Two or more foreign keys
            // 2. Few or no non-foreign key columns
            // 3. A composite primary key consisting of the foreign key columns

            // This method checks if the current table could be a junction table
            // and this foreign key is one of the relationships in the many-to-many

            // First, ensure we have a table with a primary key
            if (Table.PrimaryKey == null)
            {
                return false;
            }

            // Check if the table has at least two foreign keys
            if (Table.ForeignKeys.Count < 2)
            {
                return false;
            }

            // Check if the primary key is composite and consists of foreign key columns
            var pkColumns = Table.PrimaryKey.Columns;
            if (pkColumns.Count < 2)
            {
                return false;
            }

            // Check if all primary key columns are part of some foreign key
            foreach (var pkColumn in pkColumns)
            {
                bool isPartOfAnyFk = false;
                foreach (var fk in Table.ForeignKeys)
                {
                    if (fk.Columns.Any(c => c.Name == pkColumn.Name))
                    {
                        isPartOfAnyFk = true;
                        break;
                    }
                }

                if (!isPartOfAnyFk)
                {
                    return false;
                }
            }

            // Check if there are few non-foreign key columns (typical for junction tables)
            int nonFkColumnCount = Table.Columns.Count - Table.ForeignKeys.Sum(fk => fk.Columns.Count);
            return nonFkColumnCount <= 2; // Allow a couple of extra columns (e.g., for audit fields)
        }

        /// <summary>
        /// Generates a suggested name for a navigation property from the source table to the target table.
        /// </summary>
        /// <param name="singularCase">If true, generates a property name for a single entity reference; otherwise, generates a name for a collection.</param>
        /// <returns>A suggested navigation property name.</returns>
        public string GetNavigationPropertyName(bool singularCase = false)
        {
            var relType = DetermineRelationshipType();
            string baseName;

            if (relType == RelationshipType.ManyToMany || relType == RelationshipType.OneToMany)
            {
                // For collections going to the target table
                baseName = ReferencedTable.Name;
                return singularCase ? baseName : $"{baseName}s";
            }
            else if (relType == RelationshipType.SelfReferencing)
            {
                // For self-referencing relationships, use a more descriptive name
                baseName = "Parent";
                return singularCase ? baseName : $"Children";
            }
            else // OneToOne
            {
                // For single entity references, use the referenced table name
                return ReferencedTable.Name;
            }
        }

        /// <summary>
        /// Generates a suggested name for an inverse navigation property from the target table back to the source table.
        /// </summary>
        /// <param name="singularCase">If true, generates a property name for a single entity reference; otherwise, generates a name for a collection.</param>
        /// <returns>A suggested inverse navigation property name.</returns>
        public string GetInverseNavigationPropertyName(bool singularCase = false)
        {
            var relType = DetermineRelationshipType();
            string baseName;

            if (relType == RelationshipType.ManyToMany || relType == RelationshipType.OneToMany)
            {
                // For collections going back to the source table
                baseName = Table.Name;
                return singularCase ? baseName : $"{baseName}s";
            }
            else if (relType == RelationshipType.SelfReferencing)
            {
                // For self-referencing relationships, use a more descriptive name for the inverse
                baseName = "Child";
                return singularCase ? baseName : $"Parents";
            }
            else // OneToOne
            {
                // For single entity references, use the source table name
                return Table.Name;
            }
        }

        /// <summary>
        /// Gets a string representation of the foreign key.
        /// </summary>
        /// <returns>A string representation of the foreign key.</returns>
        public override string ToString()
        {
            var columnPairs = string.Join(", ", Enumerable.Range(0, _columns.Count)
                .Select(i => $"{_columns[i].Name} -> {_referencedColumns[i].Name}"));

            return $"{Name}: {Table.Name} -> {ReferencedTable.Name} ({columnPairs})";
        }

        private bool IsColumnSetUnique(IReadOnlyList<IColumn> columns, ITable table)
        {
            // Check if the columns form a primary key
            if (table.PrimaryKey != null)
            {
                var pkColumns = table.PrimaryKey.Columns;
                if (columns.Count == pkColumns.Count &&
                    columns.All(c =>
                        pkColumns.Any(pkc => string.Equals(pkc.Name, c.Name, StringComparison.OrdinalIgnoreCase))))
                {
                    return true;
                }
            }

            // Check if the columns form a unique constraint
            foreach (var uniqueConstraint in table.UniqueConstraints)
            {
                if (columns.Count == uniqueConstraint.Columns.Count &&
                    columns.All(c =>
                        uniqueConstraint.Columns.Any(uc => string.Equals(uc.Name, c.Name, StringComparison.OrdinalIgnoreCase))))
                {
                    return true;
                }
            }

            // Check if the columns form a unique index
            foreach (var index in table.Indexes.Where(i => i.IsUnique))
            {
                if (columns.Count == index.Columns.Count &&
                    columns.All(c =>
                        index.Columns.Any(ic => string.Equals(ic.Column.Name, c.Name, StringComparison.OrdinalIgnoreCase))))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateColumnMappings()
        {
            _columnMappings.Clear();
            
            // Create mappings for columns and referenced columns
            int count = Math.Min(_columns.Count, _referencedColumns.Count);
            for (int i = 0; i < count; i++)
            {
                _columnMappings.Add(new ForeignKeyColumnMapping(_columns[i], _referencedColumns[i]));
            }
        }
    }

    /// <summary>
    /// Defines the different types of relationships that can exist between tables.
    /// </summary>
    public enum RelationshipType
    {
        /// <summary>
        /// One row in table A is related to at most one row in table B.
        /// </summary>
        OneToOne,

        /// <summary>
        /// One row in table A is related to zero or more rows in table B.
        /// </summary>
        OneToMany,

        /// <summary>
        /// Many rows in table A are related to many rows in table B through a junction table.
        /// </summary>
        ManyToMany,

        /// <summary>
        /// Rows in a table are related to other rows in the same table.
        /// </summary>
        SelfReferencing
    }
}