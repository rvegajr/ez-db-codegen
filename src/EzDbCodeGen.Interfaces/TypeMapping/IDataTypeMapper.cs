namespace EzDbCodeGen.Interfaces.TypeMapping
{
    /// <summary>
    /// Defines a mapper for converting database data types to programming language types.
    /// </summary>
    public interface IDataTypeMapper
    {
        /// <summary>
        /// Maps a database type to a programming language type.
        /// </summary>
        /// <param name="databaseType">The database type to map.</param>
        /// <param name="isNullable">Whether the type is nullable.</param>
        /// <param name="precision">The precision of the type (for numeric types).</param>
        /// <param name="scale">The scale of the type (for numeric types).</param>
        /// <param name="maxLength">The maximum length of the type (for string types).</param>
        /// <returns>A type mapping containing the mapped type information.</returns>
        TypeMapping MapType(string databaseType, bool isNullable, int? precision = null, int? scale = null, int? maxLength = null);

        /// <summary>
        /// Gets the programming language for this type mapper.
        /// </summary>
        ProgrammingLanguage Language { get; }

        /// <summary>
        /// Gets the database provider for this type mapper.
        /// </summary>
        DatabaseProvider Provider { get; }
    }
}
