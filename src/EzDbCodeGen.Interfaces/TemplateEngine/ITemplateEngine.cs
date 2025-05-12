using System;
using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.TemplateEngine
{
    /// <summary>
    /// Defines a template engine that can compile and render templates.
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// Compiles a template for repeated rendering.
        /// </summary>
        /// <param name="templateContent">The template content to compile.</param>
        /// <returns>A delegate that can be used to render the template with different models.</returns>
        Func<object, string> Compile(string templateContent);

        /// <summary>
        /// Renders a template with the provided model.
        /// </summary>
        /// <param name="template">The compiled template function.</param>
        /// <param name="model">The model to use for template rendering.</param>
        /// <returns>The rendered template content.</returns>
        string Render(Func<object, string> template, object model);

        /// <summary>
        /// Renders a template with the provided model.
        /// </summary>
        /// <param name="templateContent">The template content.</param>
        /// <param name="model">The model to use for template rendering.</param>
        /// <returns>The rendered template content.</returns>
        string Render(string templateContent, object model);

        /// <summary>
        /// Registers a helper function with the template engine.
        /// </summary>
        /// <param name="name">The name of the helper function.</param>
        /// <param name="helper">The helper function to register.</param>
        void RegisterHelper(string name, Delegate helper);

        /// <summary>
        /// Registers a block helper function with the template engine.
        /// </summary>
        /// <param name="name">The name of the block helper function.</param>
        /// <param name="helper">The block helper function to register.</param>
        void RegisterBlockHelper(string name, Delegate helper);

        /// <summary>
        /// Registers a partial template with the template engine.
        /// </summary>
        /// <param name="name">The name of the partial template.</param>
        /// <param name="template">The partial template content.</param>
        void RegisterPartial(string name, string template);

        /// <summary>
        /// Gets all registered helpers.
        /// </summary>
        /// <returns>A dictionary of helper names to their implementations.</returns>
        IDictionary<string, Delegate> GetHelpers();

        /// <summary>
        /// Gets all registered block helpers.
        /// </summary>
        /// <returns>A dictionary of block helper names to their implementations.</returns>
        IDictionary<string, Delegate> GetBlockHelpers();

        /// <summary>
        /// Gets all registered partials.
        /// </summary>
        /// <returns>A dictionary of partial names to their content.</returns>
        IDictionary<string, string> GetPartials();
    }
}
