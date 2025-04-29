namespace EzDbCodeGen.Core.DependencyInjection;

/// <summary>
/// Defines a mechanism for retrieving a service object.
/// </summary>
public interface IServiceProvider
{
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of the service to get.</param>
    /// <returns>A service object of the specified type, or null if there is no service registration for the specified type.</returns>
    object? GetService(Type serviceType);
    
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service to get.</typeparam>
    /// <returns>A service object of the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the service is not registered.</exception>
    T GetRequiredService<T>() where T : class;
    
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service to get.</typeparam>
    /// <returns>A service object of the specified type, or null if there is no service registration for the specified type.</returns>
    T? GetService<T>() where T : class;
    
    /// <summary>
    /// Gets an enumeration of service objects of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service to get.</typeparam>
    /// <returns>An enumeration of service objects of the specified type.</returns>
    IEnumerable<T> GetServices<T>() where T : class;
    
    /// <summary>
    /// Creates a new scope.
    /// </summary>
    /// <returns>A service scope.</returns>
    IServiceScope CreateScope();
}
