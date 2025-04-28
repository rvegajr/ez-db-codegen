namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines a factory interface for creating and managing model transformers.
/// </summary>
public interface IModelTransformerFactory
{
    /// <summary>
    /// Creates a model transformer of the specified type.
    /// </summary>
    /// <param name="transformerType">The type name of the transformer to create.</param>
    /// <returns>A new model transformer instance.</returns>
    IModelTransformer CreateTransformer(string transformerType);
    
    /// <summary>
    /// Creates a model transformer of the specified type with configuration.
    /// </summary>
    /// <param name="transformerType">The type name of the transformer to create.</param>
    /// <param name="configuration">The configuration for the transformer.</param>
    /// <returns>A new configured model transformer instance.</returns>
    IModelTransformer CreateTransformer(string transformerType, IDictionary<string, object> configuration);
    
    /// <summary>
    /// Gets all available transformer types.
    /// </summary>
    /// <returns>A collection of available transformer type names.</returns>
    IReadOnlyCollection<string> GetAvailableTransformerTypes();
    
    /// <summary>
    /// Registers a model transformer type.
    /// </summary>
    /// <param name="typeName">The name to register the transformer type under.</param>
    /// <param name="transformerType">The type of the transformer.</param>
    void RegisterTransformerType(string typeName, Type transformerType);
    
    /// <summary>
    /// Creates a composite transformer that applies multiple transformers in sequence.
    /// </summary>
    /// <param name="transformerNames">The names of the transformers to include in the composite.</param>
    /// <returns>A composite model transformer.</returns>
    IModelTransformer CreateCompositeTransformer(params string[] transformerNames);
    
    /// <summary>
    /// Creates a composite transformer that applies multiple transformers in sequence.
    /// </summary>
    /// <param name="transformers">The transformers to include in the composite.</param>
    /// <returns>A composite model transformer.</returns>
    IModelTransformer CreateCompositeTransformer(params IModelTransformer[] transformers);
}
