namespace EzDbCodeGen.CodeGen.Interfaces;

using EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Defines an interface for transforming a schema model before code generation.
/// </summary>
public interface IModelTransformer
{
    /// <summary>
    /// Gets the name of the transformer.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a description of the transformer.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the order in which this transformer should be applied relative to other transformers.
    /// Lower numbers are applied first.
    /// </summary>
    int Order { get; }
    
    /// <summary>
    /// Transforms a database schema.
    /// </summary>
    /// <param name="schema">The database schema to transform.</param>
    /// <returns>The transformed database schema.</returns>
    IDatabaseSchema Transform(IDatabaseSchema schema);
    
    /// <summary>
    /// Transforms a table.
    /// </summary>
    /// <param name="table">The table to transform.</param>
    /// <returns>The transformed table.</returns>
    ITable Transform(ITable table);
    
    /// <summary>
    /// Transforms a column.
    /// </summary>
    /// <param name="column">The column to transform.</param>
    /// <returns>The transformed column.</returns>
    IColumn Transform(IColumn column);
    
    /// <summary>
    /// Transforms a relationship.
    /// </summary>
    /// <param name="relationship">The relationship to transform.</param>
    /// <returns>The transformed relationship.</returns>
    IRelationship Transform(IRelationship relationship);
    
    /// <summary>
    /// Transforms a view.
    /// </summary>
    /// <param name="view">The view to transform.</param>
    /// <returns>The transformed view.</returns>
    IView Transform(IView view);
    
    /// <summary>
    /// Transforms a stored procedure.
    /// </summary>
    /// <param name="storedProcedure">The stored procedure to transform.</param>
    /// <returns>The transformed stored procedure.</returns>
    IStoredProcedure Transform(IStoredProcedure storedProcedure);
    
    /// <summary>
    /// Transforms a function.
    /// </summary>
    /// <param name="function">The function to transform.</param>
    /// <returns>The transformed function.</returns>
    IFunction Transform(IFunction function);
    
    /// <summary>
    /// Initializes the transformer with configuration parameters.
    /// </summary>
    /// <param name="configuration">The configuration parameters.</param>
    void Initialize(IDictionary<string, object> configuration);
}
