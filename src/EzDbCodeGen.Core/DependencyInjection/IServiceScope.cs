namespace EzDbCodeGen.Core.DependencyInjection;

/// <summary>
/// Represents a scope for resolving scoped services.
/// </summary>
public interface IServiceScope : IDisposable
{
    /// <summary>
    /// Gets the service provider for this scope.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
}
