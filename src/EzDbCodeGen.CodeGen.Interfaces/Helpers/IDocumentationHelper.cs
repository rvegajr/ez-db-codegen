namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines an interface for documentation generation helpers that support XML and JSDoc formats.
/// </summary>
public interface IDocumentationHelper : ITemplateHelper
{
    /// <summary>
    /// Generates XML documentation for a property.
    /// </summary>
    /// <param name="description">The description of the property.</param>
    /// <param name="indentation">The indentation level.</param>
    /// <returns>XML documentation for the property.</returns>
    string XmlPropertyDoc(string description, int indentation = 0);
    
    /// <summary>
    /// Generates XML documentation for a class or entity.
    /// </summary>
    /// <param name="description">The description of the class or entity.</param>
    /// <param name="indentation">The indentation level.</param>
    /// <returns>XML documentation for the class or entity.</returns>
    string XmlClassDoc(string description, int indentation = 0);
    
    /// <summary>
    /// Generates XML documentation for a relationship.
    /// </summary>
    /// <param name="sourceTable">The source table name.</param>
    /// <param name="targetTable">The target table name.</param>
    /// <param name="relationType">The type of relationship.</param>
    /// <param name="indentation">The indentation level.</param>
    /// <returns>XML documentation for the relationship.</returns>
    string XmlRelationshipDoc(string sourceTable, string targetTable, string relationType, int indentation = 0);
    
    /// <summary>
    /// Generates JSDoc documentation for a property.
    /// </summary>
    /// <param name="description">The description of the property.</param>
    /// <param name="type">The type of the property.</param>
    /// <param name="indentation">The indentation level.</param>
    /// <returns>JSDoc documentation for the property.</returns>
    string JsDocPropertyDoc(string description, string type, int indentation = 0);
    
    /// <summary>
    /// Generates JSDoc documentation for a class or entity.
    /// </summary>
    /// <param name="description">The description of the class or entity.</param>
    /// <param name="indentation">The indentation level.</param>
    /// <returns>JSDoc documentation for the class or entity.</returns>
    string JsDocClassDoc(string description, int indentation = 0);
    
    /// <summary>
    /// Generates JSDoc documentation for a relationship.
    /// </summary>
    /// <param name="sourceTable">The source table name.</param>
    /// <param name="targetTable">The target table name.</param>
    /// <param name="relationType">The type of relationship.</param>
    /// <param name="indentation">The indentation level.</param>
    /// <returns>JSDoc documentation for the relationship.</returns>
    string JsDocRelationshipDoc(string sourceTable, string targetTable, string relationType, int indentation = 0);
    
    /// <summary>
    /// Generates a documentation summary from a database description.
    /// </summary>
    /// <param name="description">The database description text.</param>
    /// <param name="maxLength">The maximum length of the summary.</param>
    /// <returns>A documentation summary.</returns>
    string GenerateSummary(string description, int maxLength = 100);
    
    /// <summary>
    /// Escapes special characters in documentation text for the specified documentation format.
    /// </summary>
    /// <param name="text">The text to escape.</param>
    /// <param name="format">The documentation format (e.g., "xml", "jsdoc").</param>
    /// <returns>The escaped text.</returns>
    string EscapeDocText(string text, string format);
}
