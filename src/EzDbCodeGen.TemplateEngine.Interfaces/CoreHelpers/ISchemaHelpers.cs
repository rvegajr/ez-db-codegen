using System;

namespace EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers
{
    /// <summary>
    /// Defines the contract for schema helpers that can be registered with a template engine.
    /// </summary>
    public interface ISchemaHelpers
    {
        /// <summary>
        /// Registers all schema helpers with the template engine.
        /// </summary>
        /// <param name="templateEngine">The template engine to register the helpers with.</param>
        void RegisterHelpers(ITemplateEngine templateEngine);
    }
}
