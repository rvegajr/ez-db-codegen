using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EzDbCodeGen.Interfaces.TemplateEngine
{
    /// <summary>
    /// Defines a template processor that can render templates using a template engine.
    /// </summary>
    public interface ITemplateProcessor
    {
        /// <summary>
        /// Gets the template engine used by this processor.
        /// </summary>
        ITemplateEngine TemplateEngine { get; }

        /// <summary>
        /// Processes a template file with the provided model.
        /// </summary>
        /// <param name="templatePath">The path to the template file.</param>
        /// <param name="model">The model to use for template rendering.</param>
        /// <returns>The rendered template content.</returns>
        string ProcessTemplateFile(string templatePath, object model);

        /// <summary>
        /// Asynchronously processes a template file with the provided model.
        /// </summary>
        /// <param name="templatePath">The path to the template file.</param>
        /// <param name="model">The model to use for template rendering.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the rendered template content.</returns>
        Task<string> ProcessTemplateFileAsync(string templatePath, object model);

        /// <summary>
        /// Processes template content with the provided model.
        /// </summary>
        /// <param name="templateContent">The template content.</param>
        /// <param name="model">The model to use for template rendering.</param>
        /// <returns>The rendered template content.</returns>
        string ProcessTemplate(string templateContent, object model);

        /// <summary>
        /// Asynchronously processes template content with the provided model.
        /// </summary>
        /// <param name="templateContent">The template content.</param>
        /// <param name="model">The model to use for template rendering.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the rendered template content.</returns>
        Task<string> ProcessTemplateAsync(string templateContent, object model);

        /// <summary>
        /// Compiles a template for repeated rendering.
        /// </summary>
        /// <param name="templateContent">The template content to compile.</param>
        /// <returns>A delegate that can be used to render the template with different models.</returns>
        Func<object, string> CompileTemplate(string templateContent);

        /// <summary>
        /// Registers a helper function with the template processor.
        /// </summary>
        /// <param name="name">The name of the helper function.</param>
        /// <param name="helper">The helper function to register.</param>
        void RegisterHelper(string name, Delegate helper);

        /// <summary>
        /// Registers a block helper function with the template processor.
        /// </summary>
        /// <param name="name">The name of the block helper function.</param>
        /// <param name="helper">The block helper function to register.</param>
        void RegisterBlockHelper(string name, Delegate helper);

        /// <summary>
        /// Registers a partial template with the template processor.
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
