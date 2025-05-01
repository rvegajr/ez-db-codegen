namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a comparison between two database functions.
/// </summary>
public interface IFunctionDiff
{
    /// <summary>
    /// Gets the original function.
    /// </summary>
    IFunction? Original { get; }
    
    /// <summary>
    /// Gets the new function.
    /// </summary>
    IFunction? New { get; }
    
    /// <summary>
    /// Gets a value indicating whether the function has changed.
    /// </summary>
    bool HasChanged { get; }
    
    /// <summary>
    /// Gets the parameters that were added to the function.
    /// </summary>
    IReadOnlyCollection<IParameter> AddedParameters { get; }
    
    /// <summary>
    /// Gets the parameters that were removed from the function.
    /// </summary>
    IReadOnlyCollection<IParameter> RemovedParameters { get; }
    
    /// <summary>
    /// Gets the parameters that were changed in the function.
    /// </summary>
    IReadOnlyCollection<IParameter> ChangedParameters { get; }
    
    /// <summary>
    /// Gets a value indicating whether the return type of the function has changed.
    /// </summary>
    bool ReturnTypeChanged { get; }
    
    /// <summary>
    /// Gets the result columns that were added to the function (for table-valued functions).
    /// </summary>
    IReadOnlyCollection<IResultColumn> AddedResultColumns { get; }
    
    /// <summary>
    /// Gets the result columns that were removed from the function (for table-valued functions).
    /// </summary>
    IReadOnlyCollection<IResultColumn> RemovedResultColumns { get; }
    
    /// <summary>
    /// Gets the result columns that were changed in the function (for table-valued functions).
    /// </summary>
    IReadOnlyCollection<IResultColumn> ChangedResultColumns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the function type (scalar vs. table-valued) has changed.
    /// </summary>
    bool IsTableValuedChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the definition of the function has changed.
    /// </summary>
    bool DefinitionChanged { get; }
}
