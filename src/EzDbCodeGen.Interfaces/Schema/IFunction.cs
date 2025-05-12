using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a function in a database schema.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the schema name of the function.
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// Gets the full name of the function including the schema (e.g., "dbo.CalculateTotal").
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the description or comment of the function.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the SQL definition of the function.
        /// </summary>
        string Definition { get; }

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        IReadOnlyCollection<IParameter> Parameters { get; }

        /// <summary>
        /// Gets the return type of the function for scalar functions.
        /// </summary>
        string ReturnType { get; }

        /// <summary>
        /// Gets the return columns of the function for table-valued functions.
        /// </summary>
        IReadOnlyCollection<IColumn> ReturnColumns { get; }

        /// <summary>
        /// Gets a value indicating whether the function is a scalar function.
        /// </summary>
        bool IsScalar { get; }

        /// <summary>
        /// Gets a value indicating whether the function is a table-valued function.
        /// </summary>
        bool IsTableValued { get; }
    }
}
