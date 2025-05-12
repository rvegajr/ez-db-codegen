namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a parameter for a stored procedure or function.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the database type of the parameter.
        /// </summary>
        string DataType { get; }

        /// <summary>
        /// Gets a value indicating whether the parameter is an output parameter.
        /// </summary>
        bool IsOutput { get; }

        /// <summary>
        /// Gets a value indicating whether the parameter is nullable.
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Gets the maximum length of the parameter (for string data types).
        /// </summary>
        int? MaxLength { get; }

        /// <summary>
        /// Gets the precision of the parameter (for numeric data types).
        /// </summary>
        int? Precision { get; }

        /// <summary>
        /// Gets the scale of the parameter (for numeric data types).
        /// </summary>
        int? Scale { get; }

        /// <summary>
        /// Gets the default value of the parameter.
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// Gets the ordinal position of the parameter.
        /// </summary>
        int OrdinalPosition { get; }
    }
}
