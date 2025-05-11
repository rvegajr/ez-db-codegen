using System.Collections.Generic;
using System.Threading.Tasks;

namespace EzDbCodeGen.Common.Interfaces.TemplateEngine;

/// <summary>
/// Interface for a template processor that processes templates with a data model.
/// </summary>
public interface ITemplateProcessor
{
    /// <summary>
    /// Processes a template with the specified data model.
    /// </summary>
    /// <param name="templateContent">The template content to process.</param>
    /// <param name="dataModel">The data model to use when processing the template.</param>
    /// <returns>The result of the template processing.</returns>
    string Process(string templateContent, object dataModel);
    
    /// <summary>
    /// Processes a template file with the specified data model.
    /// </summary>
    /// <param name="templatePath">The path to the template file to process.</param>
    /// <param name="dataModel">The data model to use when processing the template.</param>
    /// <returns>The result of the template processing.</returns>
    string ProcessFile(string templatePath, object dataModel);
    
    /// <summary>
    /// Processes a template file with the specified data model and writes the result to the specified output path.
    /// </summary>
    /// <param name="templatePath">The path to the template file to process.</param>
    /// <param name="outputPath">The path to write the result to.</param>
    /// <param name="dataModel">The data model to use when processing the template.</param>
    /// <returns>The result of the template processing.</returns>
    string ProcessFile(string templatePath, string outputPath, object dataModel);
    
    /// <summary>
    /// Processes multiple template files with the specified data model.
    /// </summary>
    /// <param name="templatePaths">The paths to the template files to process.</param>
    /// <param name="dataModel">The data model to use when processing the templates.</param>
    /// <returns>A dictionary mapping template paths to their processing results.</returns>
    IDictionary<string, string> ProcessFiles(IEnumerable<string> templatePaths, object dataModel);
    
    /// <summary>
    /// Processes multiple template files with the specified data model and writes the results to the specified output directory.
    /// </summary>
    /// <param name="templatePaths">The paths to the template files to process.</param>
    /// <param name="outputDirectory">The directory to write the results to.</param>
    /// <param name="dataModel">The data model to use when processing the templates.</param>
    /// <returns>A dictionary mapping template paths to their processing results.</returns>
    IDictionary<string, string> ProcessFiles(IEnumerable<string> templatePaths, string outputDirectory, object dataModel);
    
    /// <summary>
    /// Asynchronously processes a template with the specified data model.
    /// </summary>
    /// <param name="templateContent">The template content to process.</param>
    /// <param name="dataModel">The data model to use when processing the template.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the processed template.</returns>
    Task<string> ProcessAsync(string templateContent, object dataModel);
    
    /// <summary>
    /// Asynchronously processes a template file with the specified data model.
    /// </summary>
    /// <param name="templatePath">The path to the template file to process.</param>
    /// <param name="dataModel">The data model to use when processing the template.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the processed template.</returns>
    Task<string> ProcessFileAsync(string templatePath, object dataModel);
    
    /// <summary>
    /// Asynchronously processes a template file with the specified data model and writes the result to the specified output path.
    /// </summary>
    /// <param name="templatePath">The path to the template file to process.</param>
    /// <param name="outputPath">The path to write the result to.</param>
    /// <param name="dataModel">The data model to use when processing the template.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the processed template.</returns>
    Task<string> ProcessFileAsync(string templatePath, string outputPath, object dataModel);
}
