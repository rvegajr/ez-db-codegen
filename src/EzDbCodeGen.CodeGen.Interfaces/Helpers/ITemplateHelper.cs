namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines the base interface for all template helpers.
/// </summary>
public interface ITemplateHelper
{
    /// <summary>
    /// Gets the name of the helper.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the aliases for the helper.
    /// </summary>
    IReadOnlyList<string> Aliases { get; }
    
    /// <summary>
    /// Gets a description of the helper.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the delegate representing the helper function.
    /// </summary>
    Delegate HelperFunction { get; }
    
    /// <summary>
    /// Registers this helper with the provided template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register with.</param>
    void RegisterWith(ITemplateEngine templateEngine);
}
