using System.Collections.Generic;

namespace EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;

/// <summary>
/// Represents a collection of relationship helpers for templates.
/// Provides specialized support for different relationship types.
/// </summary>
public interface IRelationshipHelpers : IHelperRegistration
{
    // This interface inherits from IHelperRegistration and will be implemented
    // by the HandlebarsRelationshipHelpers class in the TemplateEngine project
}
