namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines a factory interface for creating and managing model builders.
/// </summary>
public interface IModelBuilderFactory
{
    /// <summary>
    /// Creates a model builder of the specified type.
    /// </summary>
    /// <param name="builderType">The type name of the model builder to create.</param>
    /// <returns>A new model builder instance.</returns>
    IModelBuilder CreateBuilder(string builderType);
    
    /// <summary>
    /// Creates a model builder of the specified type with configuration.
    /// </summary>
    /// <param name="builderType">The type name of the model builder to create.</param>
    /// <param name="configuration">The configuration for the model builder.</param>
    /// <returns>A new configured model builder instance.</returns>
    IModelBuilder CreateBuilder(string builderType, IDictionary<string, object> configuration);
    
    /// <summary>
    /// Gets all available builder types.
    /// </summary>
    /// <returns>A collection of available builder type names.</returns>
    IReadOnlyCollection<string> GetAvailableBuilderTypes();
    
    /// <summary>
    /// Registers a model builder type.
    /// </summary>
    /// <param name="typeName">The name to register the builder type under.</param>
    /// <param name="builderType">The type of the builder.</param>
    void RegisterBuilderType(string typeName, Type builderType);
    
    /// <summary>
    /// Creates a composite builder that combines the models from multiple builders.
    /// </summary>
    /// <param name="builderNames">The names of the builders to include in the composite.</param>
    /// <returns>A composite model builder.</returns>
    IModelBuilder CreateCompositeBuilder(params string[] builderNames);
    
    /// <summary>
    /// Creates a composite builder that combines the models from multiple builders.
    /// </summary>
    /// <param name="builders">The builders to include in the composite.</param>
    /// <returns>A composite model builder.</returns>
    IModelBuilder CreateCompositeBuilder(params IModelBuilder[] builders);
    
    /// <summary>
    /// Gets a builder for a specific model type.
    /// </summary>
    /// <param name="modelType">The model type to get a builder for.</param>
    /// <returns>A model builder that can build the specified model type, or null if none is available.</returns>
    IModelBuilder? GetBuilderForModelType(Type modelType);
}
