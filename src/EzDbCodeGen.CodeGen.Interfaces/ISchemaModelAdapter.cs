using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Adapts database schema objects into template-friendly models.
/// </summary>
public interface ISchemaModelAdapter
{
    /// <summary>
    /// Converts a database schema into a template model.
    /// </summary>
    /// <param name="schema">The database schema to convert.</param>
    /// <returns>A dictionary with the schema model.</returns>
    Task<IDictionary<string, object>> ConvertSchemaToTemplateModelAsync(IDatabaseSchema schema);
    
    /// <summary>
    /// Converts a table into a template model.
    /// </summary>
    /// <param name="table">The table to convert.</param>
    /// <returns>A dictionary with the table model.</returns>
    Task<IDictionary<string, object>> ConvertTableToTemplateModelAsync(ITable table);
    
    /// <summary>
    /// Converts a view into a template model.
    /// </summary>
    /// <param name="view">The view to convert.</param>
    /// <returns>A dictionary with the view model.</returns>
    Task<IDictionary<string, object>> ConvertViewToTemplateModelAsync(IView view);
    
    /// <summary>
    /// Converts a stored procedure into a template model.
    /// </summary>
    /// <param name="storedProcedure">The stored procedure to convert.</param>
    /// <returns>A dictionary with the stored procedure model.</returns>
    Task<IDictionary<string, object>> ConvertStoredProcedureToTemplateModelAsync(IStoredProcedure storedProcedure);
    
    /// <summary>
    /// Converts a function into a template model.
    /// </summary>
    /// <param name="function">The function to convert.</param>
    /// <returns>A dictionary with the function model.</returns>
    Task<IDictionary<string, object>> ConvertFunctionToTemplateModelAsync(IFunction function);
}
