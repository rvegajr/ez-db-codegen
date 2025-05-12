using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines an index in a database schema.
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the table that contains the index.
        /// </summary>
        ITable Table { get; }

        /// <summary>
        /// Gets the columns that make up the index.
        /// </summary>
        IReadOnlyCollection<IColumn> Columns { get; }

        /// <summary>
        /// Gets a value indicating whether this index is unique.
        /// </summary>
        bool IsUnique { get; }

        /// <summary>
        /// Gets a value indicating whether this index is clustered.
        /// </summary>
        bool IsClustered { get; }

        /// <summary>
        /// Gets a value indicating whether this index is the primary key of the table.
        /// </summary>
        bool IsPrimaryKey { get; }
    }
}
