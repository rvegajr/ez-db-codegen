namespace EzDbCodeGen.Core.DependencyInjection;

/// <summary>
/// Describes a service with its service type, implementation, and lifetime.
/// </summary>
public class ServiceDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="lifetime">The service lifetime.</param>
    public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationFactory">The implementation factory.</param>
    /// <param name="lifetime">The service lifetime.</param>
    public ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> implementationFactory, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationFactory = implementationFactory;
        Lifetime = lifetime;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceDescriptor"/> class.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationInstance">The implementation instance.</param>
    public ServiceDescriptor(Type serviceType, object implementationInstance)
    {
        ServiceType = serviceType;
        ImplementationInstance = implementationInstance;
        Lifetime = ServiceLifetime.Singleton;
    }
    
    /// <summary>
    /// Gets the service type.
    /// </summary>
    public Type ServiceType { get; }
    
    /// <summary>
    /// Gets the implementation type.
    /// </summary>
    public Type? ImplementationType { get; }
    
    /// <summary>
    /// Gets the implementation factory.
    /// </summary>
    public Func<IServiceProvider, object>? ImplementationFactory { get; }
    
    /// <summary>
    /// Gets the implementation instance.
    /// </summary>
    public object? ImplementationInstance { get; }
    
    /// <summary>
    /// Gets the service lifetime.
    /// </summary>
    public ServiceLifetime Lifetime { get; }
    
    /// <summary>
    /// Creates a singleton service descriptor from the service type and implementation type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>A service descriptor.</returns>
    public static ServiceDescriptor Singleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);
    }
    
    /// <summary>
    /// Creates a singleton service descriptor from the service type and implementation factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="implementationFactory">The implementation factory.</param>
    /// <returns>A service descriptor.</returns>
    public static ServiceDescriptor Singleton<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        return new ServiceDescriptor(typeof(TService), sp => implementationFactory(sp)!, ServiceLifetime.Singleton);
    }
    
    /// <summary>
    /// Creates a singleton service descriptor from the service type and implementation instance.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="implementationInstance">The implementation instance.</param>
    /// <returns>A service descriptor.</returns>
    public static ServiceDescriptor Singleton<TService>(TService implementationInstance)
        where TService : class
    {
        return new ServiceDescriptor(typeof(TService), implementationInstance!);
    }
    
    /// <summary>
    /// Creates a scoped service descriptor from the service type and implementation type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>A service descriptor.</returns>
    public static ServiceDescriptor Scoped<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);
    }
    
    /// <summary>
    /// Creates a scoped service descriptor from the service type and implementation factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="implementationFactory">The implementation factory.</param>
    /// <returns>A service descriptor.</returns>
    public static ServiceDescriptor Scoped<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        return new ServiceDescriptor(typeof(TService), sp => implementationFactory(sp)!, ServiceLifetime.Scoped);
    }
    
    /// <summary>
    /// Creates a transient service descriptor from the service type and implementation type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>A service descriptor.</returns>
    public static ServiceDescriptor Transient<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);
    }
    
    /// <summary>
    /// Creates a transient service descriptor from the service type and implementation factory.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <param name="implementationFactory">The implementation factory.</param>
    /// <returns>A service descriptor.</returns>
    public static ServiceDescriptor Transient<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        return new ServiceDescriptor(typeof(TService), sp => implementationFactory(sp)!, ServiceLifetime.Transient);
    }
}
