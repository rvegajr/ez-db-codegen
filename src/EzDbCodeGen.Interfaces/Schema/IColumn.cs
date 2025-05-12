namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a column in a database table or view.
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the database type of the column.
        /// </summary>
        string DataType { get; }

        /// <summary>
        /// Gets a value indicating whether the column is nullable.
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Gets a value indicating whether the column is part of a primary key.
        /// </summary>
        bool IsPrimaryKey { get; }

        /// <summary>
        /// Gets a value indicating whether the column is an identity column.
        /// </summary>
        bool IsIdentity { get; }

        /// <summary>
        /// Gets a value indicating whether the column is a computed column.
        /// </summary>
        bool IsComputed { get; }

        /// <summary>
        /// Gets the ordinal position of the column in the table.
        /// </summary>
        int OrdinalPosition { get; }

        /// <summary>
        /// Gets the maximum length of the column (for string data types).
        /// </summary>
        int? MaxLength { get; }

        /// <summary>
        /// Gets the precision of the column (for numeric data types).
        /// </summary>
        int? Precision { get; }

        /// <summary>
        /// Gets the scale of the column (for numeric data types).
        /// </summary>
        int? Scale { get; }

        /// <summary>
        /// Gets the default value of the column.
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// Gets the description or comment of the column.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the table that contains this column.
        /// </summary>
        ITable Table { get; }
    }
}
