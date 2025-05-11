namespace EzDbCodeGen.Common.Interfaces.TypeMapping;

/// <summary>
/// Defines the interface for a data type mapping provider.
/// </summary>
public interface IDataTypeMap
{
    /// <summary>
    /// Gets the target language type for the specified database type.
    /// </summary>
    /// <param name="databaseType">The database type to map.</param>
    /// <param name="isNullable">Whether the type is nullable.</param>
    /// <returns>The mapped target language type.</returns>
    string GetTargetType(string databaseType, bool isNullable);
    
    /// <summary>
    /// Gets the default value for the specified database type.
    /// </summary>
    /// <param name="databaseType">The database type to get the default value for.</param>
    /// <returns>The default value for the specified database type.</returns>
    string GetDefaultValue(string databaseType);
}
