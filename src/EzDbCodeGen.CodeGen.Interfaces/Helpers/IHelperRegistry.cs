namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines an interface for a registry that manages all template helpers.
/// </summary>
public interface IHelperRegistry
{
    /// <summary>
    /// Registers a helper function.
    /// </summary>
    /// <param name="name">The name of the helper.</param>
    /// <param name="helperFunction">The helper function.</param>
    void RegisterHelper(string name, Delegate helperFunction);
    
    /// <summary>
    /// Registers a helper function with multiple aliases.
    /// </summary>
    /// <param name="names">The names/aliases of the helper.</param>
    /// <param name="helperFunction">The helper function.</param>
    void RegisterHelper(IEnumerable<string> names, Delegate helperFunction);
    
    /// <summary>
    /// Registers a helper instance.
    /// </summary>
    /// <param name="helper">The helper to register.</param>
    void RegisterHelper(ITemplateHelper helper);
    
    /// <summary>
    /// Registers a collection of helpers.
    /// </summary>
    /// <param name="helpers">The helpers to register.</param>
    void RegisterHelpers(IEnumerable<ITemplateHelper> helpers);
    
    /// <summary>
    /// Gets a helper by name.
    /// </summary>
    /// <param name="name">The name of the helper to get.</param>
    /// <returns>The helper, or null if no helper with the specified name exists.</returns>
    ITemplateHelper? GetHelper(string name);
    
    /// <summary>
    /// Gets all registered helpers.
    /// </summary>
    /// <returns>A collection of all registered helpers.</returns>
    IReadOnlyCollection<ITemplateHelper> GetAllHelpers();
    
    /// <summary>
    /// Gets all registered helper names.
    /// </summary>
    /// <returns>A collection of all registered helper names, including aliases.</returns>
    IReadOnlyCollection<string> GetAllHelperNames();
    
    /// <summary>
    /// Removes a helper by name.
    /// </summary>
    /// <param name="name">The name of the helper to remove.</param>
    /// <returns>True if the helper was removed; otherwise, false.</returns>
    bool RemoveHelper(string name);
    
    /// <summary>
    /// Removes all helpers with the specified names.
    /// </summary>
    /// <param name="names">The names of the helpers to remove.</param>
    /// <returns>The number of helpers removed.</returns>
    int RemoveHelpers(IEnumerable<string> names);
    
    /// <summary>
    /// Clears all registered helpers.
    /// </summary>
    void ClearHelpers();
    
    /// <summary>
    /// Determines whether a helper with the specified name exists.
    /// </summary>
    /// <param name="name">The name of the helper to check.</param>
    /// <returns>True if a helper with the specified name exists; otherwise, false.</returns>
    bool HasHelper(string name);
    
    /// <summary>
    /// Creates a new helper registry with a subset of helpers.
    /// </summary>
    /// <param name="helperNames">The names of the helpers to include in the new registry.</param>
    /// <returns>A new helper registry containing only the specified helpers.</returns>
    IHelperRegistry CreateSubsetRegistry(IEnumerable<string> helperNames);
    
    /// <summary>
    /// Merges another helper registry into this one.
    /// </summary>
    /// <param name="registry">The registry to merge.</param>
    /// <param name="overwrite">Whether to overwrite existing helpers with the same name.</param>
    void MergeRegistry(IHelperRegistry registry, bool overwrite = true);
}
