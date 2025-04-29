namespace EzDbCodeGen.Core.TemplateEngine.Helpers;

using EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a collection of relationship helpers for templates.
/// Provides specialized support for different relationship types.
/// </summary>
public interface IRelationshipHelpers
{
    /// <summary>
    /// Determines if a table participates in a one-to-many relationship.
    /// </summary>
    /// <param name="table">The table to check.</param>
    /// <returns>True if the table participates in a one-to-many relationship, false otherwise.</returns>
    bool HasOneToManyRelationships(ITable table);
    
    /// <summary>
    /// Determines if a table participates in a many-to-one relationship.
    /// </summary>
    /// <param name="table">The table to check.</param>
    /// <returns>True if the table participates in a many-to-one relationship, false otherwise.</returns>
    bool HasManyToOneRelationships(ITable table);
    
    /// <summary>
    /// Determines if a table participates in a one-to-one relationship.
    /// </summary>
    /// <param name="table">The table to check.</param>
    /// <returns>True if the table participates in a one-to-one relationship, false otherwise.</returns>
    bool HasOneToOneRelationships(ITable table);
    
    /// <summary>
    /// Determines if a table participates in a many-to-many relationship.
    /// </summary>
    /// <param name="table">The table to check.</param>
    /// <returns>True if the table participates in a many-to-many relationship, false otherwise.</returns>
    bool HasManyToManyRelationships(ITable table);
    
    /// <summary>
    /// Determines if a table is a junction table (used in many-to-many relationships).
    /// </summary>
    /// <param name="table">The table to check.</param>
    /// <returns>True if the table is a junction table, false otherwise.</returns>
    bool IsJunctionTable(ITable table);
    
    /// <summary>
    /// Gets all child relationships for a table.
    /// </summary>
    /// <param name="table">The table to get child relationships for.</param>
    /// <returns>A list of child relationships.</returns>
    IEnumerable<IRelationship> GetChildRelationships(ITable table);
    
    /// <summary>
    /// Gets all parent relationships for a table.
    /// </summary>
    /// <param name="table">The table to get parent relationships for.</param>
    /// <returns>A list of parent relationships.</returns>
    IEnumerable<IRelationship> GetParentRelationships(ITable table);
    
    /// <summary>
    /// Gets all many-to-many relationships for a table.
    /// </summary>
    /// <param name="table">The table to get many-to-many relationships for.</param>
    /// <returns>A list of many-to-many relationships.</returns>
    IEnumerable<IRelationship> GetManyToManyRelationships(ITable table);
    
    /// <summary>
    /// Gets the navigation property name for a relationship.
    /// </summary>
    /// <param name="relationship">The relationship to get the navigation property name for.</param>
    /// <param name="pluralize">Whether to pluralize the navigation property name.</param>
    /// <returns>The navigation property name.</returns>
    string GetNavigationPropertyName(IRelationship relationship, bool pluralize);
    
    /// <summary>
    /// Gets the navigation property type for a relationship.
    /// </summary>
    /// <param name="relationship">The relationship to get the navigation property type for.</param>
    /// <param name="language">The target programming language.</param>
    /// <returns>The navigation property type.</returns>
    string GetNavigationPropertyType(IRelationship relationship, string language);
    
    /// <summary>
    /// Gets the inverse navigation property name for a relationship.
    /// </summary>
    /// <param name="relationship">The relationship to get the inverse navigation property name for.</param>
    /// <param name="pluralize">Whether to pluralize the inverse navigation property name.</param>
    /// <returns>The inverse navigation property name.</returns>
    string GetInverseNavigationPropertyName(IRelationship relationship, bool pluralize);
    
    /// <summary>
    /// Gets the collection type for a relationship.
    /// </summary>
    /// <param name="language">The target programming language.</param>
    /// <returns>The collection type.</returns>
    string GetCollectionType(string language);
    
    /// <summary>
    /// Gets the self-referencing relationships for a table.
    /// </summary>
    /// <param name="table">The table to get self-referencing relationships for.</param>
    /// <returns>A list of self-referencing relationships.</returns>
    IEnumerable<IRelationship> GetSelfReferencingRelationships(ITable table);
    
    /// <summary>
    /// Gets the inheritance relationships for a table.
    /// </summary>
    /// <param name="table">The table to get inheritance relationships for.</param>
    /// <returns>A list of inheritance relationships.</returns>
    IEnumerable<IRelationship> GetInheritanceRelationships(ITable table);
    
    /// <summary>
    /// Gets the base table for a derived table.
    /// </summary>
    /// <param name="table">The derived table.</param>
    /// <returns>The base table, or null if the table has no base.</returns>
    ITable? GetBaseTable(ITable table);
    
    /// <summary>
    /// Gets the derived tables for a base table.
    /// </summary>
    /// <param name="table">The base table.</param>
    /// <returns>A list of derived tables.</returns>
    IEnumerable<ITable> GetDerivedTables(ITable table);
}
