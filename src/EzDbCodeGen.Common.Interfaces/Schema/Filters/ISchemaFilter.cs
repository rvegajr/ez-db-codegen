namespace EzDbCodeGen.Common.Interfaces.Schema.Filters;

/// <summary>
/// Defines the interface for a filter that filters database schema objects.
/// </summary>
public interface ISchemaFilter
{
    /// <summary>
    /// Filters a database schema.
    /// </summary>
    /// <param name="schema">The database schema to filter.</param>
    /// <returns>The filtered database schema.</returns>
    IDatabaseSchema Filter(IDatabaseSchema schema);
    
    /// <summary>
    /// Sets the pattern to use for filtering.
    /// </summary>
    /// <param name="pattern">The pattern to use for filtering.</param>
    void SetPattern(string pattern);
}
