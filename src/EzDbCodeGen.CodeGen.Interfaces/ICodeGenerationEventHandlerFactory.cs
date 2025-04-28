namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines a factory interface for creating and managing code generation event handlers.
/// </summary>
public interface ICodeGenerationEventHandlerFactory
{
    /// <summary>
    /// Creates an event handler of the specified type.
    /// </summary>
    /// <param name="handlerType">The type name of the event handler to create.</param>
    /// <returns>A new event handler instance.</returns>
    ICodeGenerationEventHandler CreateHandler(string handlerType);
    
    /// <summary>
    /// Creates an event handler of the specified type with configuration.
    /// </summary>
    /// <param name="handlerType">The type name of the event handler to create.</param>
    /// <param name="configuration">The configuration for the event handler.</param>
    /// <returns>A new configured event handler instance.</returns>
    ICodeGenerationEventHandler CreateHandler(string handlerType, IDictionary<string, object> configuration);
    
    /// <summary>
    /// Gets all available handler types.
    /// </summary>
    /// <returns>A collection of available handler type names.</returns>
    IReadOnlyCollection<string> GetAvailableHandlerTypes();
    
    /// <summary>
    /// Registers an event handler type.
    /// </summary>
    /// <param name="typeName">The name to register the handler type under.</param>
    /// <param name="handlerType">The type of the handler.</param>
    void RegisterHandlerType(string typeName, Type handlerType);
    
    /// <summary>
    /// Creates a composite handler that delegates to multiple handlers.
    /// </summary>
    /// <param name="handlerNames">The names of the handlers to include in the composite.</param>
    /// <returns>A composite event handler.</returns>
    ICodeGenerationEventHandler CreateCompositeHandler(params string[] handlerNames);
    
    /// <summary>
    /// Creates a composite handler that delegates to multiple handlers.
    /// </summary>
    /// <param name="handlers">The handlers to include in the composite.</param>
    /// <returns>A composite event handler.</returns>
    ICodeGenerationEventHandler CreateCompositeHandler(params ICodeGenerationEventHandler[] handlers);
    
    /// <summary>
    /// Gets all registered handlers.
    /// </summary>
    /// <returns>A collection of all registered handlers.</returns>
    IReadOnlyCollection<ICodeGenerationEventHandler> GetAllHandlers();
}
