namespace EzDbCodeGen.Schema.Interfaces.Filters;

/// <summary>
/// Interface for filtering database schema elements.
/// </summary>
public interface ISchemaFilter
{
    /// <summary>
    /// Filters a database schema based on specific criteria.
    /// </summary>
    /// <param name="schema">The database schema to filter.</param>
    /// <returns>A filtered database schema.</returns>
    IDatabaseSchema Filter(IDatabaseSchema schema);
}
