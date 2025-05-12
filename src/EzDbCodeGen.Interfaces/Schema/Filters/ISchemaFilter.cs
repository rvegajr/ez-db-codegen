namespace EzDbCodeGen.Interfaces.Schema.Filters
{
    /// <summary>
    /// Defines a filter for database schemas.
    /// </summary>
    public interface ISchemaFilter
    {
        /// <summary>
        /// Filters a database schema.
        /// </summary>
        /// <param name="schema">The database schema to filter.</param>
        /// <param name="options">The options for filtering.</param>
        /// <returns>The filtered database schema.</returns>
        IDatabaseSchema Filter(IDatabaseSchema schema, SchemaFilterOptions options);

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the filter.
        /// </summary>
        string Description { get; }
    }
}
