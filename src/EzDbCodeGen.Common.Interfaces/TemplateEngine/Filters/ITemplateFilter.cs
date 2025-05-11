namespace EzDbCodeGen.Common.Interfaces.TemplateEngine.Filters;

/// <summary>
/// Defines the interface for a filter that filters template output.
/// </summary>
public interface ITemplateFilter
{
    /// <summary>
    /// Filters the output of a template.
    /// </summary>
    /// <param name="input">The input to filter.</param>
    /// <returns>The filtered output.</returns>
    string Filter(string input);
    
    /// <summary>
    /// Sets the pattern to use for filtering.
    /// </summary>
    /// <param name="pattern">The pattern to use for filtering.</param>
    void SetPattern(string pattern);
}
