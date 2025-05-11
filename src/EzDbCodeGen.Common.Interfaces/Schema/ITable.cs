using System.Collections.Generic;

namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a database table.
/// </summary>
public interface ITable
{
    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the schema of the table.
    /// </summary>
    string Schema { get; }
    
    /// <summary>
    /// Gets the columns in the table.
    /// </summary>
    IReadOnlyCollection<IColumn> Columns { get; }
}
