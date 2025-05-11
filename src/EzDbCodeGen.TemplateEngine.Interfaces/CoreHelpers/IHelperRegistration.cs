using System;
using System.Collections.Generic;

namespace EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;

/// <summary>
/// Represents a registration system for template helpers.
/// </summary>
public interface IHelperRegistration
{
    /// <summary>
    /// Registers helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register helpers with.</param>
    void RegisterHelpers(ITemplateEngine templateEngine);
}
