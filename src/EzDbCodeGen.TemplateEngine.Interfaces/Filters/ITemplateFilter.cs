namespace EzDbCodeGen.TemplateEngine.Interfaces.Filters;

/// <summary>
/// Interface for filtering template output.
/// </summary>
public interface ITemplateFilter
{
    /// <summary>
    /// Filters the template output.
    /// </summary>
    /// <param name="input">The input text to filter.</param>
    /// <returns>The filtered text.</returns>
    string Filter(string input);

    /// <summary>
    /// Gets the order in which this filter should be applied.
    /// </summary>
    int Order { get; }
}
