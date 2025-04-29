namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a stored procedure in a database.
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
    /// Gets the columns returned by the stored procedure, if any.
    /// </summary>
    IReadOnlyCollection<IResultColumn>? ResultColumns { get; }
    
    /// <summary>
    /// Gets the definition of the stored procedure.
    /// </summary>
    string? Definition { get; }
}
