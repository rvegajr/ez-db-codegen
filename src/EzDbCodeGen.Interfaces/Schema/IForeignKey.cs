using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a foreign key constraint in a database schema.
    /// </summary>
    public interface IForeignKey
    {
        /// <summary>
        /// Gets the name of the foreign key constraint.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the table that contains the foreign key.
        /// </summary>
        ITable Table { get; }

        /// <summary>
        /// Gets the referenced table.
        /// </summary>
        ITable ReferencedTable { get; }

        /// <summary>
        /// Gets the columns that make up the foreign key.
        /// </summary>
        IReadOnlyCollection<IColumn> Columns { get; }

        /// <summary>
        /// Gets the referenced columns in the referenced table.
        /// </summary>
        IReadOnlyCollection<IColumn> ReferencedColumns { get; }

        /// <summary>
        /// Gets the update action for the foreign key.
        /// </summary>
        ReferentialAction UpdateAction { get; }

        /// <summary>
        /// Gets the delete action for the foreign key.
        /// </summary>
        ReferentialAction DeleteAction { get; }

        /// <summary>
        /// Gets a value indicating whether this foreign key forms a circular reference
        /// (the table references itself directly or indirectly).
        /// </summary>
        bool IsCircularReference { get; }

        /// <summary>
        /// Gets a value indicating whether this foreign key is part of the primary key of the table.
        /// </summary>
        bool IsPrimaryKey { get; }

        /// <summary>
        /// Gets a value indicating whether this foreign key is enforced by the database.
        /// </summary>
        bool IsEnforced { get; }
    }
}
