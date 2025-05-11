namespace EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;

/// <summary>
/// Represents a collection of test helpers for templates.
/// </summary>
public interface ITestHelpers : IHelperRegistration
{
    /// <summary>
    /// Generates a test value for the specified type.
    /// </summary>
    /// <param name="type">The type to generate a test value for.</param>
    /// <param name="seed">A seed value to generate unique test values.</param>
    /// <returns>A string representation of a test value.</returns>
    string GenerateTestValue(string type, int seed);
    
    /// <summary>
    /// Pluralizes an English word.
    /// </summary>
    /// <param name="word">The word to pluralize.</param>
    /// <returns>The pluralized word.</returns>
    string Pluralize(string word);
    
    /// <summary>
    /// Generates a navigation property name based on the table name.
    /// </summary>
    /// <param name="tableName">The name of the related table.</param>
    /// <param name="isCollection">Whether the navigation property is a collection.</param>
    /// <param name="suffix">Optional suffix to add to the property name.</param>
    /// <returns>A properly formatted navigation property name.</returns>
    string GenerateNavigationPropertyName(string tableName, bool isCollection, string suffix = null);
}
