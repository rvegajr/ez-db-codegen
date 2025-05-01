using System;

namespace EzDbCodeGen.TypeMapping.Interfaces;

/// <summary>
/// Interface for type mapping between database and target language types.
/// </summary>
public interface ITypeMapper
{
    /// <summary>
    /// Maps a database type to a target language type.
    /// </summary>
    /// <param name="databaseType">The database type name</param>
    /// <param name="isNullable">Whether the type is nullable</param>
    /// <returns>The mapped type name in the target language</returns>
    string MapType(string databaseType, bool isNullable);

    /// <summary>
    /// Gets the default value for a mapped type.
    /// </summary>
    /// <param name="mappedType">The mapped type name</param>
    /// <returns>The default value as a string</returns>
    string GetDefaultValue(string mappedType);
}
