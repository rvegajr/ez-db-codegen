namespace EzDbCodeGen.Core.TemplateEngine.Helpers;

using EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a collection of type conversion helpers for templates.
/// Implementation of the ConvertTypeEz helper mentioned in the architectural specifications.
/// </summary>
public interface ITypeConversionHelpers
{
    /// <summary>
    /// Converts a database column type to a target language type.
    /// </summary>
    /// <param name="column">The column to convert.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The converted type name.</returns>
    string ConvertType(IColumn column, string language);
    
    /// <summary>
    /// Gets the type name for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the type name for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The type name.</returns>
    string GetTypeName(IColumn column, string language);
    
    /// <summary>
    /// Gets the type name with nullability for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the type name for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The type name with nullability.</returns>
    string GetTypeNameWithNullability(IColumn column, string language);
    
    /// <summary>
    /// Gets the string type for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the string type for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The string type.</returns>
    string GetStringType(IColumn column, string language);
    
    /// <summary>
    /// Gets the numeric type for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the numeric type for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The numeric type.</returns>
    string GetNumericType(IColumn column, string language);
    
    /// <summary>
    /// Gets the decimal type for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the decimal type for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The decimal type.</returns>
    string GetDecimalType(IColumn column, string language);
    
    /// <summary>
    /// Gets the date/time type for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the date/time type for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The date/time type.</returns>
    string GetDateTimeType(IColumn column, string language);
    
    /// <summary>
    /// Gets the default value for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the default value for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The default value.</returns>
    string GetDefaultValue(IColumn column, string language);
    
    /// <summary>
    /// Gets the nullable annotation for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the nullable annotation for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The nullable annotation.</returns>
    string GetNullableAnnotation(IColumn column, string language);
    
    /// <summary>
    /// Adds nullability to a type name.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The type name with nullability.</returns>
    string WithNullability(string typeName, bool isNullable, string language);
    
    /// <summary>
    /// Gets the spatial type for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the spatial type for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The spatial type.</returns>
    string GetSpatialType(IColumn column, string language);
    
    /// <summary>
    /// Gets the JSON type for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the JSON type for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The JSON type.</returns>
    string GetJsonType(IColumn column, string language);
    
    /// <summary>
    /// Gets the XML type for a column in the target language.
    /// </summary>
    /// <param name="column">The column to get the XML type for.</param>
    /// <param name="language">The target language.</param>
    /// <returns>The XML type.</returns>
    string GetXmlType(IColumn column, string language);
}
