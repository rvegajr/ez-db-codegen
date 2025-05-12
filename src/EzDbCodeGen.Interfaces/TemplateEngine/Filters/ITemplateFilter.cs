using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.TemplateEngine.Filters
{
    /// <summary>
    /// Defines a filter for templates.
    /// </summary>
    public interface ITemplateFilter
    {
        /// <summary>
        /// Filters a template input and output to determine if it should be processed.
        /// </summary>
        /// <param name="inputPath">The input template path.</param>
        /// <param name="outputPath">The output path for the rendered template.</param>
        /// <param name="context">The template rendering context.</param>
        /// <param name="options">The options for filtering.</param>
        /// <returns>True if the template should be processed, false otherwise.</returns>
        bool ShouldProcess(string inputPath, string outputPath, object context, TemplateFilterOptions options);

        /// <summary>
        /// Transforms the output path for a rendered template.
        /// </summary>
        /// <param name="outputPath">The original output path.</param>
        /// <param name="context">The template rendering context.</param>
        /// <param name="options">The options for filtering.</param>
        /// <returns>The transformed output path.</returns>
        string TransformOutputPath(string outputPath, object context, TemplateFilterOptions options);

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
