namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a factory for creating relationship detectors.
/// </summary>
public interface IRelationshipDetectorFactory
{
    /// <summary>
    /// Creates a relationship detector with default options.
    /// </summary>
    /// <returns>A relationship detector.</returns>
    IRelationshipDetector CreateRelationshipDetector();
    
    /// <summary>
    /// Creates a relationship detector with the specified options.
    /// </summary>
    /// <param name="options">The relationship detection options.</param>
    /// <returns>A relationship detector.</returns>
    IRelationshipDetector CreateRelationshipDetector(RelationshipDetectionOptions options);
    
    /// <summary>
    /// Creates a relationship detector for a specific database provider.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <returns>A relationship detector.</returns>
    IRelationshipDetector CreateRelationshipDetectorForProvider(string providerName);
    
    /// <summary>
    /// Creates a relationship detector for a specific database provider with the specified options.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="options">The relationship detection options.</param>
    /// <returns>A relationship detector.</returns>
    IRelationshipDetector CreateRelationshipDetectorForProvider(string providerName, RelationshipDetectionOptions options);
    
    /// <summary>
    /// Registers a relationship detector factory method.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="factoryMethod">The factory method.</param>
    void RegisterRelationshipDetectorFactory(string providerName, Func<RelationshipDetectionOptions, IRelationshipDetector> factoryMethod);
    
    /// <summary>
    /// Gets all available database provider names.
    /// </summary>
    /// <returns>A list of available database provider names.</returns>
    IReadOnlyList<string> GetAvailableProviders();
}
