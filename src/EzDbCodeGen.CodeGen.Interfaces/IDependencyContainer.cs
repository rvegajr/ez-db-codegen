namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a dependency injection container that manages service registrations and resolutions.
/// </summary>
public interface IDependencyContainer
{
    /// <summary>
    /// Registers a service implementation for a service type.
    /// </summary>
    /// <typeparam name="TService">The service type to register.</typeparam>
    /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
    void Register<TService, TImplementation>() where TImplementation : class, TService;
    
    /// <summary>
    /// Registers a service implementation instance for a service type.
    /// </summary>
    /// <typeparam name="TService">The service type to register.</typeparam>
    /// <param name="instance">The implementation instance of the service.</param>
    void RegisterInstance<TService>(TService instance) where TService : class;
    
    /// <summary>
    /// Registers a service factory for a service type.
    /// </summary>
    /// <typeparam name="TService">The service type to register.</typeparam>
    /// <param name="factory">The factory function that creates the service.</param>
    void RegisterFactory<TService>(Func<IDependencyContainer, TService> factory) where TService : class;
    
    /// <summary>
    /// Registers a singleton service implementation for a service type.
    /// </summary>
    /// <typeparam name="TService">The service type to register.</typeparam>
    /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
    void RegisterSingleton<TService, TImplementation>() where TImplementation : class, TService;
    
    /// <summary>
    /// Resolves a service instance for a service type.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    TService Resolve<TService>() where TService : class;
    
    /// <summary>
    /// Tries to resolve a service instance for a service type.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <param name="service">The resolved service instance, or null if the service could not be resolved.</param>
    /// <returns>True if the service was resolved; otherwise, false.</returns>
    bool TryResolve<TService>(out TService? service) where TService : class;
    
    /// <summary>
    /// Resolves all service instances for a service type.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <returns>A collection of resolved service instances.</returns>
    IReadOnlyCollection<TService> ResolveAll<TService>() where TService : class;
    
    /// <summary>
    /// Determines whether a service type is registered.
    /// </summary>
    /// <typeparam name="TService">The service type to check.</typeparam>
    /// <returns>True if the service type is registered; otherwise, false.</returns>
    bool IsRegistered<TService>() where TService : class;
    
    /// <summary>
    /// Creates a new container scope.
    /// </summary>
    /// <returns>A new container scope.</returns>
    IDependencyContainer CreateScope();
    
    /// <summary>
    /// Disposes the container and releases any resources used by it.
    /// </summary>
    void Dispose();
    
    /// <summary>
    /// Gets a service provider that can be used to resolve services.
    /// </summary>
    /// <returns>A service provider.</returns>
    IServiceProvider GetServiceProvider();
}
