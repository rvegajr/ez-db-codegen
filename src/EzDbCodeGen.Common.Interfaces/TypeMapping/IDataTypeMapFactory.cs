namespace EzDbCodeGen.Common.Interfaces.TypeMapping;

/// <summary>
/// Defines the interface for a factory that creates data type maps.
/// </summary>
public interface IDataTypeMapFactory
{
    /// <summary>
    /// Creates a data type map for the specified database type.
    /// </summary>
    /// <param name="databaseType">The type of database to create a data type map for.</param>
    /// <returns>The created data type map.</returns>
    IDataTypeMap CreateMap(string databaseType);
    
    /// <summary>
    /// Gets a SQL Server data type map.
    /// </summary>
    /// <returns>A SQL Server data type map.</returns>
    IDataTypeMap GetSqlServerMap();
}
