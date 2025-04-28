namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines an interface for relationship helpers that support different relationship types and navigation.
/// </summary>
public interface IRelationshipHelper : ITemplateHelper
{
    /// <summary>
    /// Gets the one-to-many relationships for a table.
    /// </summary>
    /// <param name="table">The table to get relationships for.</param>
    /// <returns>A list of one-to-many relationships.</returns>
    IReadOnlyList<IRelationship> GetOneToManyRelationships(ITable table);
    
    /// <summary>
    /// Gets the one-to-one relationships for a table.
    /// </summary>
    /// <param name="table">The table to get relationships for.</param>
    /// <returns>A list of one-to-one relationships.</returns>
    IReadOnlyList<IRelationship> GetOneToOneRelationships(ITable table);
    
    /// <summary>
    /// Gets the many-to-many relationships for a table.
    /// </summary>
    /// <param name="table">The table to get relationships for.</param>
    /// <returns>A list of many-to-many relationships.</returns>
    IReadOnlyList<IRelationship> GetManyToManyRelationships(ITable table);
    
    /// <summary>
    /// Gets the self-referencing relationships for a table.
    /// </summary>
    /// <param name="table">The table to get relationships for.</param>
    /// <returns>A list of self-referencing relationships.</returns>
    IReadOnlyList<IRelationship> GetSelfReferencingRelationships(ITable table);
    
    /// <summary>
    /// Gets the inheritance relationships for a table.
    /// </summary>
    /// <param name="table">The table to get relationships for.</param>
    /// <returns>A list of inheritance relationships.</returns>
    IReadOnlyList<IRelationship> GetInheritanceRelationships(ITable table);
    
    /// <summary>
    /// Determines if a table is a base class in an inheritance hierarchy.
    /// </summary>
    /// <param name="table">The table to check.</param>
    /// <returns>True if the table is a base class; otherwise, false.</returns>
    bool IsBaseClass(ITable table);
    
    /// <summary>
    /// Determines if a table is a derived class in an inheritance hierarchy.
    /// </summary>
    /// <param name="table">The table to check.</param>
    /// <returns>True if the table is a derived class; otherwise, false.</returns>
    bool IsDerivedClass(ITable table);
    
    /// <summary>
    /// Gets the base class for a derived table in an inheritance hierarchy.
    /// </summary>
    /// <param name="derivedTable">The derived table.</param>
    /// <returns>The base class table, or null if the table has no base class.</returns>
    ITable? GetBaseClass(ITable derivedTable);
    
    /// <summary>
    /// Gets all derived classes for a base table in an inheritance hierarchy.
    /// </summary>
    /// <param name="baseTable">The base table.</param>
    /// <returns>A list of derived class tables.</returns>
    IReadOnlyList<ITable> GetDerivedClasses(ITable baseTable);
    
    /// <summary>
    /// Gets the navigation property name for a relationship.
    /// </summary>
    /// <param name="relationship">The relationship.</param>
    /// <param name="isSourceNavigation">True to get the source navigation property name; false to get the target navigation property name.</param>
    /// <returns>The navigation property name.</returns>
    string GetNavigationPropertyName(IRelationship relationship, bool isSourceNavigation);
    
    /// <summary>
    /// Determines if a relationship property should be a collection.
    /// </summary>
    /// <param name="relationship">The relationship.</param>
    /// <param name="isSourceNavigation">True to check the source navigation property; false to check the target navigation property.</param>
    /// <returns>True if the navigation property should be a collection; otherwise, false.</returns>
    bool IsCollection(IRelationship relationship, bool isSourceNavigation);
}
