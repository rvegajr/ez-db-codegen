using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a unique constraint in a database schema.
    /// </summary>
    public interface IUniqueConstraint
    {
        /// <summary>
        /// Gets the name of the unique constraint.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the table that contains the unique constraint.
        /// </summary>
        ITable Table { get; }

        /// <summary>
        /// Gets the columns that make up the unique constraint.
        /// </summary>
        IReadOnlyCollection<IColumn> Columns { get; }

        /// <summary>
        /// Gets a value indicating whether this unique constraint is the primary key of the table.
        /// </summary>
        bool IsPrimaryKey { get; }
    }
}
