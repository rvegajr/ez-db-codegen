using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a stored procedure in a database schema.
    /// </summary>
    public interface IStoredProcedure
    {
        /// <summary>
        /// Gets the name of the stored procedure.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the schema name of the stored procedure.
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// Gets the full name of the stored procedure including the schema (e.g., "dbo.GetCustomers").
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the description or comment of the stored procedure.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the SQL definition of the stored procedure.
        /// </summary>
        string Definition { get; }

        /// <summary>
        /// Gets the parameters of the stored procedure.
        /// </summary>
        IReadOnlyCollection<IParameter> Parameters { get; }

        /// <summary>
        /// Gets the return columns of the stored procedure, if it returns a result set.
        /// </summary>
        IReadOnlyCollection<IColumn> ReturnColumns { get; }

        /// <summary>
        /// Gets a value indicating whether the stored procedure returns a result set.
        /// </summary>
        bool HasResultSet { get; }
    }
}
