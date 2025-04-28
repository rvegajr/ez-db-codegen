namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for analyzing templates and extracting metadata.
/// </summary>
public interface ITemplateAnalyzer
{
    /// <summary>
    /// Analyzes a template and extracts its metadata.
    /// </summary>
    /// <param name="templateContent">The template content to analyze.</param>
    /// <returns>The extracted template metadata.</returns>
    TemplateMetadata AnalyzeTemplate(string templateContent);
    
    /// <summary>
    /// Analyzes a template file and extracts its metadata.
    /// </summary>
    /// <param name="templatePath">The path to the template file to analyze.</param>
    /// <returns>The extracted template metadata.</returns>
    TemplateMetadata AnalyzeTemplateFile(string templatePath);
    
    /// <summary>
    /// Analyzes a named template and extracts its metadata.
    /// </summary>
    /// <param name="templateName">The name of the template to analyze.</param>
    /// <returns>The extracted template metadata.</returns>
    TemplateMetadata AnalyzeNamedTemplate(string templateName);
    
    /// <summary>
    /// Analyzes all templates in a directory and extracts their metadata.
    /// </summary>
    /// <param name="directoryPath">The path to the directory containing templates.</param>
    /// <param name="recursive">Whether to search subdirectories.</param>
    /// <param name="fileExtension">The file extension of templates to analyze (e.g., ".hbs").</param>
    /// <returns>A dictionary mapping template names to their metadata.</returns>
    IDictionary<string, TemplateMetadata> AnalyzeDirectory(string directoryPath, bool recursive = false, string fileExtension = ".hbs");
    
    /// <summary>
    /// Analyzes all templates in the template repository and extracts their metadata.
    /// </summary>
    /// <returns>A dictionary mapping template names to their metadata.</returns>
    IDictionary<string, TemplateMetadata> AnalyzeAllTemplates();
    
    /// <summary>
    /// Identifies all helpers used in a template.
    /// </summary>
    /// <param name="templateContent">The template content to analyze.</param>
    /// <returns>A collection of helper names used in the template.</returns>
    IReadOnlyCollection<string> GetUsedHelpers(string templateContent);
    
    /// <summary>
    /// Identifies all partials included in a template.
    /// </summary>
    /// <param name="templateContent">The template content to analyze.</param>
    /// <returns>A collection of partial names included in the template.</returns>
    IReadOnlyCollection<string> GetIncludedPartials(string templateContent);
    
    /// <summary>
    /// Extracts all model property references from a template.
    /// </summary>
    /// <param name="templateContent">The template content to analyze.</param>
    /// <returns>A collection of model property paths referenced in the template.</returns>
    IReadOnlyCollection<string> GetModelReferences(string templateContent);
    
    /// <summary>
    /// Gets dependencies between templates based on partial inclusion and helper usage.
    /// </summary>
    /// <param name="templateNames">The names of the templates to analyze dependencies for.</param>
    /// <returns>A dependency manager containing the template dependencies.</returns>
    ITemplateDependencyManager GetTemplateDependencies(IEnumerable<string> templateNames);
}
