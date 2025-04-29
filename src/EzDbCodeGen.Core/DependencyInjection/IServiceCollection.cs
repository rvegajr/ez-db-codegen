namespace EzDbCodeGen.Core.DependencyInjection;

/// <summary>
/// Represents a collection of service descriptors.
/// </summary>
public interface IServiceCollection : IList<ServiceDescriptor>
{
    /// <summary>
    /// Adds a transient service of the specified service type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>The service collection.</returns>
    IServiceCollection AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService;
    
    /// <summary>
    /// Adds a transient service of the specified service type with an implementation factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="implementationFactory">The implementation factory.</param>
    /// <returns>The service collection.</returns>
    IServiceCollection AddTransient<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class;
    
    /// <summary>
    /// Adds a scoped service of the specified service type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>The service collection.</returns>
    IServiceCollection AddScoped<TService, TImplementation>() where TService : class where TImplementation : class, TService;
    
    /// <summary>
    /// Adds a scoped service of the specified service type with an implementation factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="implementationFactory">The implementation factory.</param>
    /// <returns>The service collection.</returns>
    IServiceCollection AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class;
    
    /// <summary>
    /// Adds a singleton service of the specified service type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>The service collection.</returns>
    IServiceCollection AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService;
    
    /// <summary>
    /// Adds a singleton service of the specified service type with an implementation factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="implementationFactory">The implementation factory.</param>
    /// <returns>The service collection.</returns>
    IServiceCollection AddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class;
    
    /// <summary>
    /// Adds a singleton service of the specified service type with an implementation instance.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="implementationInstance">The implementation instance.</param>
    /// <returns>The service collection.</returns>
    IServiceCollection AddSingleton<TService>(TService implementationInstance) where TService : class;
    
    /// <summary>
    /// Adds the specified services to the collection.
    /// </summary>
    /// <param name="services">The services to add.</param>
    /// <returns>The service collection.</returns>
    IServiceCollection AddRange(IEnumerable<ServiceDescriptor> services);
    
    /// <summary>
    /// Builds a service provider from the service collection.
    /// </summary>
    /// <returns>A service provider.</returns>
    IServiceProvider BuildServiceProvider();
}
