namespace EzDbCodeGen.Core.TemplateEngine.Helpers;

using EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a collection of documentation helpers for templates.
/// Implementation of the DocEz helper mentioned in the architectural specifications.
/// </summary>
public interface IDocumentationHelpers
{
    /// <summary>
    /// Generates a summary documentation comment for an entity.
    /// </summary>
    /// <param name="table">The table to document.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GenerateEntitySummary(ITable table, string format);
    
    /// <summary>
    /// Generates a summary documentation comment for a property.
    /// </summary>
    /// <param name="column">The column to document.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GeneratePropertySummary(IColumn column, string format);
    
    /// <summary>
    /// Generates a summary documentation comment for a relationship.
    /// </summary>
    /// <param name="relationship">The relationship to document.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GenerateRelationshipSummary(IRelationship relationship, string format);
    
    /// <summary>
    /// Generates a parameter documentation comment.
    /// </summary>
    /// <param name="parameter">The parameter to document.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GenerateParameterDoc(IParameter parameter, string format);
    
    /// <summary>
    /// Generates a return value documentation comment.
    /// </summary>
    /// <param name="returnType">The return type to document.</param>
    /// <param name="description">The description of the return value.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GenerateReturnDoc(string returnType, string description, string format);
    
    /// <summary>
    /// Generates a complete class documentation comment.
    /// </summary>
    /// <param name="table">The table to document.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GenerateClassDoc(ITable table, string format);
    
    /// <summary>
    /// Generates a complete property documentation comment.
    /// </summary>
    /// <param name="column">The column to document.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GeneratePropertyDoc(IColumn column, string format);
    
    /// <summary>
    /// Adds validation documentation to a property documentation comment.
    /// </summary>
    /// <param name="column">The column to document.</param>
    /// <param name="existingDoc">The existing documentation comment.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment with validation documentation.</returns>
    string AddValidationDoc(IColumn column, string existingDoc, string format);
    
    /// <summary>
    /// Generates a documentation comment for a primary key.
    /// </summary>
    /// <param name="key">The key to document.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GeneratePrimaryKeyDoc(IKey key, string format);
    
    /// <summary>
    /// Generates a documentation comment for a foreign key.
    /// </summary>
    /// <param name="foreignKey">The foreign key to document.</param>
    /// <param name="format">The documentation format (e.g., "xml" for C#, "jsdoc" for JavaScript).</param>
    /// <returns>The documentation comment.</returns>
    string GenerateForeignKeyDoc(IForeignKey foreignKey, string format);
}
