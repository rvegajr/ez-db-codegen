using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Interfaces.CodeGen.Adapters
{
    /// <summary>
    /// Defines an adapter for converting database schema objects to template-friendly models.
    /// </summary>
    public interface ISchemaModelAdapter
    {
        /// <summary>
        /// Converts a database schema to a template model.
        /// </summary>
        /// <param name="schema">The database schema to convert.</param>
        /// <returns>A dictionary containing the template model.</returns>
        Dictionary<string, object> ConvertSchemaToTemplateModel(IDatabaseSchema schema);

        /// <summary>
        /// Asynchronously converts a database schema to a template model.
        /// </summary>
        /// <param name="schema">The database schema to convert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the template model.</returns>
        Task<Dictionary<string, object>> ConvertSchemaToTemplateModelAsync(IDatabaseSchema schema);

        /// <summary>
        /// Converts a table to a template model.
        /// </summary>
        /// <param name="table">The table to convert.</param>
        /// <returns>A dictionary containing the template model.</returns>
        Dictionary<string, object> ConvertTableToTemplateModel(ITable table);

        /// <summary>
        /// Asynchronously converts a table to a template model.
        /// </summary>
        /// <param name="table">The table to convert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the template model.</returns>
        Task<Dictionary<string, object>> ConvertTableToTemplateModelAsync(ITable table);

        /// <summary>
        /// Converts a column to a template model.
        /// </summary>
        /// <param name="column">The column to convert.</param>
        /// <returns>A dictionary containing the template model.</returns>
        Dictionary<string, object> ConvertColumnToTemplateModel(IColumn column);

        /// <summary>
        /// Asynchronously converts a column to a template model.
        /// </summary>
        /// <param name="column">The column to convert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the template model.</returns>
        Task<Dictionary<string, object>> ConvertColumnToTemplateModelAsync(IColumn column);

        /// <summary>
        /// Converts a relationship to a template model.
        /// </summary>
        /// <param name="relationship">The relationship to convert.</param>
        /// <returns>A dictionary containing the template model.</returns>
        Dictionary<string, object> ConvertRelationshipToTemplateModel(IRelationship relationship);

        /// <summary>
        /// Asynchronously converts a relationship to a template model.
        /// </summary>
        /// <param name="relationship">The relationship to convert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the template model.</returns>
        Task<Dictionary<string, object>> ConvertRelationshipToTemplateModelAsync(IRelationship relationship);

        /// <summary>
        /// Converts a stored procedure to a template model.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure to convert.</param>
        /// <returns>A dictionary containing the template model.</returns>
        Dictionary<string, object> ConvertStoredProcedureToTemplateModel(IStoredProcedure storedProcedure);

        /// <summary>
        /// Asynchronously converts a stored procedure to a template model.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure to convert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the template model.</returns>
        Task<Dictionary<string, object>> ConvertStoredProcedureToTemplateModelAsync(IStoredProcedure storedProcedure);

        /// <summary>
        /// Converts a function to a template model.
        /// </summary>
        /// <param name="function">The function to convert.</param>
        /// <returns>A dictionary containing the template model.</returns>
        Dictionary<string, object> ConvertFunctionToTemplateModel(IFunction function);

        /// <summary>
        /// Asynchronously converts a function to a template model.
        /// </summary>
        /// <param name="function">The function to convert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the template model.</returns>
        Task<Dictionary<string, object>> ConvertFunctionToTemplateModelAsync(IFunction function);
    }
}
