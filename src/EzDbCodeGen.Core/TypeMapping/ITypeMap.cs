namespace EzDbCodeGen.Core.TypeMapping;

/// <summary>
/// Represents a type map that maps database types to programming language types.
/// </summary>
public interface ITypeMap
{
    /// <summary>
    /// Maps a database type to a programming language type.
    /// </summary>
    /// <param name="databaseType">The database type.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="precision">The precision.</param>
    /// <param name="scale">The scale.</param>
    /// <returns>The programming language type.</returns>
    string MapDatabaseType(string databaseType, int? maxLength, int? precision, int? scale);
    
    /// <summary>
    /// Gets the nullability syntax for a type.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The nullability syntax.</returns>
    string GetNullabilitySyntax(string typeName, bool isNullable);
    
    /// <summary>
    /// Gets the default value for a type.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The default value.</returns>
    string GetDefaultValue(string typeName, bool isNullable);
    
    /// <summary>
    /// Gets the language of the type map.
    /// </summary>
    string Language { get; }
    
    /// <summary>
    /// Determines whether a type is a reference type.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <returns>True if the type is a reference type, false otherwise.</returns>
    bool IsReferenceType(string typeName);
    
    /// <summary>
    /// Determines whether a type is a value type.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <returns>True if the type is a value type, false otherwise.</returns>
    bool IsValueType(string typeName);
    
    /// <summary>
    /// Determines whether a value type should be nullable.
    /// </summary>
    /// <param name="databaseType">The database type.</param>
    /// <param name="isNullable">Whether the database column is nullable.</param>
    /// <returns>True if the value type should be nullable, false otherwise.</returns>
    bool ShouldBeNullableValueType(string databaseType, bool isNullable);
    
    /// <summary>
    /// Determines whether a reference type should be nullable.
    /// </summary>
    /// <param name="databaseType">The database type.</param>
    /// <param name="isNullable">Whether the database column is nullable.</param>
    /// <returns>True if the reference type should be nullable, false otherwise.</returns>
    bool ShouldBeNullableReferenceType(string databaseType, bool isNullable);
}
