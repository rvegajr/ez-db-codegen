namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines a factory interface for creating and managing template context providers.
/// </summary>
public interface ITemplateContextProviderFactory
{
    /// <summary>
    /// Creates a template context provider of the specified type.
    /// </summary>
    /// <param name="providerType">The type name of the context provider to create.</param>
    /// <returns>A new template context provider instance.</returns>
    ITemplateContextProvider CreateProvider(string providerType);
    
    /// <summary>
    /// Creates a template context provider of the specified type with configuration.
    /// </summary>
    /// <param name="providerType">The type name of the context provider to create.</param>
    /// <param name="configuration">The configuration for the context provider.</param>
    /// <returns>A new configured template context provider instance.</returns>
    ITemplateContextProvider CreateProvider(string providerType, IDictionary<string, object> configuration);
    
    /// <summary>
    /// Gets all available provider types.
    /// </summary>
    /// <returns>A collection of available provider type names.</returns>
    IReadOnlyCollection<string> GetAvailableProviderTypes();
    
    /// <summary>
    /// Registers a template context provider type.
    /// </summary>
    /// <param name="typeName">The name to register the provider type under.</param>
    /// <param name="providerType">The type of the provider.</param>
    void RegisterProviderType(string typeName, Type providerType);
    
    /// <summary>
    /// Creates a composite provider that combines the context from multiple providers.
    /// </summary>
    /// <param name="providerNames">The names of the providers to include in the composite.</param>
    /// <returns>A composite template context provider.</returns>
    ITemplateContextProvider CreateCompositeProvider(params string[] providerNames);
    
    /// <summary>
    /// Creates a composite provider that combines the context from multiple providers.
    /// </summary>
    /// <param name="providers">The providers to include in the composite.</param>
    /// <returns>A composite template context provider.</returns>
    ITemplateContextProvider CreateCompositeProvider(params ITemplateContextProvider[] providers);
    
    /// <summary>
    /// Gets all registered providers.
    /// </summary>
    /// <returns>A collection of all registered providers.</returns>
    IReadOnlyCollection<ITemplateContextProvider> GetAllProviders();
}
