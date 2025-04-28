namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a comparison between two stored procedures.
/// </summary>
public interface IStoredProcedureDiff
{
    /// <summary>
    /// Gets the original stored procedure.
    /// </summary>
    IStoredProcedure Original { get; }
    
    /// <summary>
    /// Gets the new stored procedure.
    /// </summary>
    IStoredProcedure New { get; }
    
    /// <summary>
    /// Gets a value indicating whether the stored procedure has changed.
    /// </summary>
    bool HasChanged { get; }
    
    /// <summary>
    /// Gets the parameters that were added to the stored procedure.
    /// </summary>
    IReadOnlyCollection<IParameter> AddedParameters { get; }
    
    /// <summary>
    /// Gets the parameters that were removed from the stored procedure.
    /// </summary>
    IReadOnlyCollection<IParameter> RemovedParameters { get; }
    
    /// <summary>
    /// Gets the parameters that were changed in the stored procedure.
    /// </summary>
    IReadOnlyCollection<IParameter> ChangedParameters { get; }
    
    /// <summary>
    /// Gets the result columns that were added to the stored procedure.
    /// </summary>
    IReadOnlyCollection<IResultColumn> AddedResultColumns { get; }
    
    /// <summary>
    /// Gets the result columns that were removed from the stored procedure.
    /// </summary>
    IReadOnlyCollection<IResultColumn> RemovedResultColumns { get; }
    
    /// <summary>
    /// Gets the result columns that were changed in the stored procedure.
    /// </summary>
    IReadOnlyCollection<IResultColumn> ChangedResultColumns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the definition of the stored procedure has changed.
    /// </summary>
    bool DefinitionChanged { get; }
}
