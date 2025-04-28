namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines an interface for data type conversion helpers that support mapping database types to programming language types.
/// </summary>
public interface ITypeConversionHelper : ITemplateHelper
{
    /// <summary>
    /// Converts a SQL data type to a C# type.
    /// </summary>
    /// <param name="sqlType">The SQL data type.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <param name="precision">The precision for numeric types.</param>
    /// <param name="scale">The scale for numeric types.</param>
    /// <param name="maxLength">The maximum length for string types.</param>
    /// <returns>The corresponding C# type.</returns>
    string ToCSharpType(string sqlType, bool isNullable = false, int? precision = null, int? scale = null, int? maxLength = null);
    
    /// <summary>
    /// Converts a SQL data type to a TypeScript type.
    /// </summary>
    /// <param name="sqlType">The SQL data type.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <param name="precision">The precision for numeric types.</param>
    /// <param name="scale">The scale for numeric types.</param>
    /// <param name="maxLength">The maximum length for string types.</param>
    /// <returns>The corresponding TypeScript type.</returns>
    string ToTypeScriptType(string sqlType, bool isNullable = false, int? precision = null, int? scale = null, int? maxLength = null);
    
    /// <summary>
    /// Converts a SQL data type to a Java type.
    /// </summary>
    /// <param name="sqlType">The SQL data type.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <param name="precision">The precision for numeric types.</param>
    /// <param name="scale">The scale for numeric types.</param>
    /// <param name="maxLength">The maximum length for string types.</param>
    /// <returns>The corresponding Java type.</returns>
    string ToJavaType(string sqlType, bool isNullable = false, int? precision = null, int? scale = null, int? maxLength = null);
    
    /// <summary>
    /// Converts a SQL data type to a Python type.
    /// </summary>
    /// <param name="sqlType">The SQL data type.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <param name="precision">The precision for numeric types.</param>
    /// <param name="scale">The scale for numeric types.</param>
    /// <param name="maxLength">The maximum length for string types.</param>
    /// <returns>The corresponding Python type.</returns>
    string ToPythonType(string sqlType, bool isNullable = false, int? precision = null, int? scale = null, int? maxLength = null);
    
    /// <summary>
    /// Gets the default value for a type in the specified language.
    /// </summary>
    /// <param name="type">The type name.</param>
    /// <param name="language">The target programming language.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The default value for the type.</returns>
    string GetDefaultValue(string type, string language, bool isNullable = false);
    
    /// <summary>
    /// Formats a type with nullability syntax for the specified language.
    /// </summary>
    /// <param name="type">The type name.</param>
    /// <param name="language">The target programming language.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The type formatted with appropriate nullability syntax.</returns>
    string FormatWithNullability(string type, string language, bool isNullable);
}
