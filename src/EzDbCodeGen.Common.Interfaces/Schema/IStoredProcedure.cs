using System.Collections.Generic;

namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a database stored procedure.
/// </summary>
public interface IStoredProcedure
{
    /// <summary>
    /// Gets the name of the stored procedure.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the schema of the stored procedure.
    /// </summary>
    string Schema { get; }
    
    /// <summary>
    /// Gets the parameters of the stored procedure.
    /// </summary>
    IReadOnlyCollection<IParameter> Parameters { get; }
    
    /// <summary>
    /// Gets the result columns of the stored procedure.
    /// </summary>
    IReadOnlyCollection<IResultColumn> ResultColumns { get; }
}
