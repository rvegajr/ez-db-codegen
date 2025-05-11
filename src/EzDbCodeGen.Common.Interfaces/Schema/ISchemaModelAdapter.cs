namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Defines the interface for an adapter that converts a database schema to a model that can be used in templates.
/// </summary>
public interface ISchemaModelAdapter
{
    /// <summary>
    /// Adapts a database schema to a model that can be used in templates.
    /// </summary>
    /// <param name="schema">The database schema to adapt.</param>
    /// <returns>The adapted model.</returns>
    object AdaptSchema(IDatabaseSchema schema);
    
    /// <summary>
    /// Adapts a table to a model that can be used in templates.
    /// </summary>
    /// <param name="table">The table to adapt.</param>
    /// <returns>The adapted model.</returns>
    object AdaptTable(ITable table);
    
    /// <summary>
    /// Adapts a view to a model that can be used in templates.
    /// </summary>
    /// <param name="view">The view to adapt.</param>
    /// <returns>The adapted model.</returns>
    object AdaptView(IView view);
    
    /// <summary>
    /// Adapts a stored procedure to a model that can be used in templates.
    /// </summary>
    /// <param name="storedProcedure">The stored procedure to adapt.</param>
    /// <returns>The adapted model.</returns>
    object AdaptStoredProcedure(IStoredProcedure storedProcedure);
    
    /// <summary>
    /// Adapts a function to a model that can be used in templates.
    /// </summary>
    /// <param name="function">The function to adapt.</param>
    /// <returns>The adapted model.</returns>
    object AdaptFunction(IFunction function);
}
