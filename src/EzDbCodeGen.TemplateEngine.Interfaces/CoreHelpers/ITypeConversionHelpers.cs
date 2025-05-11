namespace EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;

/// <summary>
/// Represents a collection of type conversion helpers for templates.
/// Implementation of the ConvertTypeEz helper mentioned in the architectural specifications.
/// </summary>
public interface ITypeConversionHelpers : IHelperRegistration
{
    /// <summary>
    /// Converts a database column type to a target language type.
    /// </summary>
    /// <param name="column">The column to convert.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The converted type name.</returns>
    string ConvertType(string dbType, string language);
    
    /// <summary>
    /// Makes a type nullable in the target language.
    /// </summary>
    /// <param name="type">The type to make nullable.</param>
    /// <param name="isNullable">Whether the type should be nullable.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The nullable type.</returns>
    string MakeNullable(string type, bool isNullable, string language);
}
