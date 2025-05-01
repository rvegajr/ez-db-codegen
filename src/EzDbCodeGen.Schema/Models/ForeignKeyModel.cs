using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a foreign key constraint in a database table.
    /// </summary>
    public class ForeignKeyModel : IForeignKey
    {
        private readonly List<IColumn> _columns = new List<IColumn>();
        private readonly List<IColumn> _referencedColumns = new List<IColumn>();
        private readonly List<IForeignKeyColumnMapping> _columnMappings = new List<IForeignKeyColumnMapping>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyModel"/> class.
        /// </summary>
        public ForeignKeyModel()
        {
            Name = string.Empty;
            Table = null!;
            ReferencedTable = null!;
        }

        /// <summary>
        /// Gets or sets the name of the foreign key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the table that the foreign key belongs to.
        /// </summary>
        public ITable Table { get; set; }

        /// <summary>
        /// Gets the columns that make up the foreign key.
        /// </summary>
        public IReadOnlyList<IColumn> Columns => _columns.AsReadOnly();

        /// <summary>
        /// Gets or sets the referenced table.
        /// </summary>
        public ITable ReferencedTable { get; set; }

        /// <summary>
        /// Gets the referenced columns.
        /// </summary>
        public IReadOnlyList<IColumn> ReferencedColumns => _referencedColumns.AsReadOnly();

        /// <summary>
        /// Gets or sets the delete action for the foreign key.
        /// </summary>
        public ReferentialAction DeleteAction { get; set; }

        /// <summary>
        /// Gets or sets the update action for the foreign key.
        /// </summary>
        public ReferentialAction UpdateAction { get; set; }

        /// <summary>
        /// Gets the column mappings for the foreign key.
        /// </summary>
        public IReadOnlyCollection<IForeignKeyColumnMapping> ColumnMappings => _columnMappings.AsReadOnly();

        /// <summary>
        /// Gets or sets the delete behavior for the foreign key (legacy property).
        /// </summary>
        public string DeleteBehavior 
        { 
            get => DeleteAction.ToString(); 
            set 
            {
                if (Enum.TryParse<ReferentialAction>(value, out var action))
                {
                    DeleteAction = action;
                }
            } 
        }

        /// <summary>
        /// Gets or sets the update behavior for the foreign key (legacy property).
        /// </summary>
        public string UpdateBehavior 
        { 
            get => UpdateAction.ToString(); 
            set 
            {
                if (Enum.TryParse<ReferentialAction>(value, out var action))
                {
                    UpdateAction = action;
                }
            } 
        }

        /// <summary>
        /// Gets a value indicating whether this is a composite foreign key (multiple columns).
        /// </summary>
        public bool IsComposite => Columns.Count > 1;

        /// <summary>
        /// Gets a value indicating whether this foreign key references a primary key.
        /// </summary>
        public bool ReferencesPrimaryKey
        {
            get
            {
                if (ReferencedTable?.PrimaryKey == null ||
                    ReferencedTable.PrimaryKey.Columns.Count != ReferencedColumns.Count)
                    return false;

                for (int i = 0; i < ReferencedColumns.Count; i++)
                {
                    if (!ReferencedTable.PrimaryKey.Columns.Contains(ReferencedColumns[i]))
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the foreign key is part of the primary key.
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                if (Table?.PrimaryKey == null || Columns.Count == 0)
                {
                    return false;
                }

                var pkColumns = Table.PrimaryKey.Columns;
                return Columns.All(c => pkColumns.Any(pkc => pkc.Name == c.Name));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the foreign key is part of a unique constraint.
        /// </summary>
        public bool IsUnique
        {
            get
            {
                if (Table == null || Columns.Count == 0)
                {
                    return false;
                }

                foreach (var uniqueConstraint in Table.UniqueConstraints)
                {
                    var uniqueColumns = uniqueConstraint.Columns;
                    if (Columns.All(c => uniqueColumns.Any(uc => uc.Name == c.Name)))
                    {
                        return true;
                    }
                }

                foreach (var index in Table.Indexes.Where(i => i.IsUnique))
                {
                    var indexColumns = index.Columns.Select(ic => ic.Column);
                    if (Columns.All(c => indexColumns.Any(ic => ic.Name == c.Name)))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the on delete rule.
        /// </summary>
        public string OnDelete
        {
            get => DeleteAction.ToString();
            set
            {
                if (Enum.TryParse<ReferentialAction>(value, out var action))
                {
                    DeleteAction = action;
                }
            }
        }

        /// <summary>
        /// Gets or sets the on update rule.
        /// </summary>
        public string OnUpdate
        {
            get => UpdateAction.ToString();
            set
            {
                if (Enum.TryParse<ReferentialAction>(value, out var action))
                {
                    UpdateAction = action;
                }
            }
        }

        /// <summary>
        /// Gets the mutable collection of columns for backward compatibility.
        /// </summary>
        public IList<IColumn> MutableColumns => _columns;

        /// <summary>
        /// Gets the mutable collection of referenced columns for backward compatibility.
        /// </summary>
        public IList<IColumn> MutableReferencedColumns => _referencedColumns;

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
}