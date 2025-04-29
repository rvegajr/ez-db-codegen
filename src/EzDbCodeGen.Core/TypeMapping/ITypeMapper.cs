namespace EzDbCodeGen.Core.TypeMapping;

using EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a type mapper that maps database types to programming language types.
/// </summary>
public interface ITypeMapper
{
    /// <summary>
    /// Maps a database column to a programming language type.
    /// </summary>
    /// <param name="column">The column to map.</param>
    /// <param name="options">Options for type mapping.</param>
    /// <returns>The mapped type name.</returns>
    string MapType(IColumn column, TypeMappingOptions options);
    
    /// <summary>
    /// Gets the default value for a database column in the target language.
    /// </summary>
    /// <param name="column">The column to get the default value for.</param>
    /// <param name="options">Options for type mapping.</param>
    /// <returns>The default value.</returns>
    string GetDefaultValue(IColumn column, TypeMappingOptions options);
    
    /// <summary>
    /// Determines whether a column should be nullable in the target language.
    /// </summary>
    /// <param name="column">The column to check.</param>
    /// <param name="options">Options for type mapping.</param>
    /// <returns>True if the column should be nullable, false otherwise.</returns>
    bool IsNullable(IColumn column, TypeMappingOptions options);
    
    /// <summary>
    /// Gets the nullability syntax for a type in the target language.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The nullability syntax.</returns>
    string GetNullabilitySyntax(string typeName, bool isNullable);
    
    /// <summary>
    /// Gets the target language for the type mapper.
    /// </summary>
    string Language { get; }
}
