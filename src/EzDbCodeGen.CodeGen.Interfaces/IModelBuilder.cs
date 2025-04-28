namespace EzDbCodeGen.CodeGen.Interfaces;

using EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Defines an interface for building models for code generation.
/// </summary>
public interface IModelBuilder
{
    /// <summary>
    /// Gets the name of the model builder.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a description of the model builder.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Builds a model for code generation from a database schema.
    /// </summary>
    /// <param name="schema">The database schema to build a model from.</param>
    /// <returns>The built model object.</returns>
    object BuildFromSchema(IDatabaseSchema schema);
    
    /// <summary>
    /// Builds a model for code generation from a table.
    /// </summary>
    /// <param name="table">The table to build a model from.</param>
    /// <param name="schema">The database schema that contains the table.</param>
    /// <returns>The built model object.</returns>
    object BuildFromTable(ITable table, IDatabaseSchema schema);
    
    /// <summary>
    /// Builds a model for code generation from a view.
    /// </summary>
    /// <param name="view">The view to build a model from.</param>
    /// <param name="schema">The database schema that contains the view.</param>
    /// <returns>The built model object.</returns>
    object BuildFromView(IView view, IDatabaseSchema schema);
    
    /// <summary>
    /// Builds a model for code generation from a stored procedure.
    /// </summary>
    /// <param name="storedProcedure">The stored procedure to build a model from.</param>
    /// <param name="schema">The database schema that contains the stored procedure.</param>
    /// <returns>The built model object.</returns>
    object BuildFromStoredProcedure(IStoredProcedure storedProcedure, IDatabaseSchema schema);
    
    /// <summary>
    /// Builds a model for code generation from a function.
    /// </summary>
    /// <param name="function">The function to build a model from.</param>
    /// <param name="schema">The database schema that contains the function.</param>
    /// <returns>The built model object.</returns>
    object BuildFromFunction(IFunction function, IDatabaseSchema schema);
    
    /// <summary>
    /// Builds a model for code generation for a many-to-many relationship.
    /// </summary>
    /// <param name="relationship">The relationship to build a model from.</param>
    /// <param name="schema">The database schema that contains the relationship.</param>
    /// <returns>The built model object.</returns>
    object BuildFromManyToManyRelationship(IRelationship relationship, IDatabaseSchema schema);
    
    /// <summary>
    /// Initializes the model builder with configuration parameters.
    /// </summary>
    /// <param name="configuration">The configuration parameters.</param>
    void Initialize(IDictionary<string, object> configuration);
    
    /// <summary>
    /// Gets the object type created by this model builder.
    /// </summary>
    /// <returns>The type of object created by this model builder.</returns>
    Type GetModelType();
}
