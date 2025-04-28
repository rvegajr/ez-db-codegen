namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for validating templates.
/// </summary>
public interface ITemplateValidator
{
    /// <summary>
    /// Validates a template string.
    /// </summary>
    /// <param name="templateContent">The template content to validate.</param>
    /// <returns>A validation result indicating whether the template is valid and any errors found.</returns>
    IValidationResult ValidateTemplate(string templateContent);
    
    /// <summary>
    /// Validates a template file.
    /// </summary>
    /// <param name="templatePath">The path to the template file to validate.</param>
    /// <returns>A validation result indicating whether the template is valid and any errors found.</returns>
    IValidationResult ValidateTemplateFile(string templatePath);
    
    /// <summary>
    /// Validates a named template.
    /// </summary>
    /// <param name="templateName">The name of the template to validate.</param>
    /// <returns>A validation result indicating whether the template is valid and any errors found.</returns>
    IValidationResult ValidateNamedTemplate(string templateName);
    
    /// <summary>
    /// Validates all templates in a directory.
    /// </summary>
    /// <param name="directoryPath">The path to the directory containing templates.</param>
    /// <param name="recursive">Whether to search subdirectories.</param>
    /// <param name="fileExtension">The file extension of templates to validate (e.g., ".hbs").</param>
    /// <returns>A collection of validation results for all templates.</returns>
    IReadOnlyCollection<IValidationResult> ValidateDirectory(string directoryPath, bool recursive = false, string fileExtension = ".hbs");
    
    /// <summary>
    /// Validates all templates in the template repository.
    /// </summary>
    /// <returns>A collection of validation results for all templates.</returns>
    IReadOnlyCollection<IValidationResult> ValidateAllTemplates();
}
