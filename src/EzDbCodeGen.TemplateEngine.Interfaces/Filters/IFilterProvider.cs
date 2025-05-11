namespace EzDbCodeGen.TemplateEngine.Interfaces.Filters;

/// <summary>
/// Interface for providing template filters.
/// </summary>
public interface IFilterProvider
{
    /// <summary>
    /// Gets whether the provider can create a filter for the specified name.
    /// </summary>
    /// <param name="filterName">The name of the filter.</param>
    /// <returns>True if the provider can create a filter for the name; otherwise, false.</returns>
    bool CanCreateFilter(string filterName);
    
    /// <summary>
    /// Creates a filter for the specified name.
    /// </summary>
    /// <param name="filterName">The name of the filter.</param>
    /// <returns>The created filter.</returns>
    ITemplateFilter CreateFilter(string filterName);
}
