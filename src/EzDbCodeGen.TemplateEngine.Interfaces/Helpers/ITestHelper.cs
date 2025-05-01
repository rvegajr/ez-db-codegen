namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Interface for Handlebars helpers that generate test-related content.
/// </summary>
public interface ITestHelper : IHandlebarsHelper
{
    /// <summary>
    /// Generates test data for a specific type.
    /// </summary>
    /// <param name="type">The type to generate test data for.</param>
    /// <returns>The generated test data.</returns>
    object GenerateTestData(string type);

    /// <summary>
    /// Generates a test method name based on the provided context.
    /// </summary>
    /// <param name="context">The test context.</param>
    /// <returns>The generated test method name.</returns>
    string GenerateTestMethodName(object context);
}
