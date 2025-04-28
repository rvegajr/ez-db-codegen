namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a validation result.
/// </summary>
public interface IValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    bool IsValid { get; }
    
    /// <summary>
    /// Gets the name of the item that was validated.
    /// </summary>
    string ItemName { get; }
    
    /// <summary>
    /// Gets a collection of validation errors.
    /// </summary>
    IReadOnlyCollection<string> Errors { get; }
    
    /// <summary>
    /// Gets a collection of validation warnings.
    /// </summary>
    IReadOnlyCollection<string> Warnings { get; }
    
    /// <summary>
    /// Gets the path to the validated item.
    /// </summary>
    string? ItemPath { get; }
    
    /// <summary>
    /// Gets the type of the validated item.
    /// </summary>
    string ItemType { get; }
    
    /// <summary>
    /// Adds an error to the validation result.
    /// </summary>
    /// <param name="error">The error message to add.</param>
    void AddError(string error);
    
    /// <summary>
    /// Adds a warning to the validation result.
    /// </summary>
    /// <param name="warning">The warning message to add.</param>
    void AddWarning(string warning);
    
    /// <summary>
    /// Gets a formatted error message including all errors and warnings.
    /// </summary>
    /// <returns>A formatted error message.</returns>
    string GetFormattedErrorMessage();
    
    /// <summary>
    /// Determines whether the validation result contains a specific error.
    /// </summary>
    /// <param name="errorMessage">The error message to check for.</param>
    /// <returns>True if the validation result contains the specified error; otherwise, false.</returns>
    bool ContainsError(string errorMessage);
    
    /// <summary>
    /// Determines whether the validation result contains a specific warning.
    /// </summary>
    /// <param name="warningMessage">The warning message to check for.</param>
    /// <returns>True if the validation result contains the specified warning; otherwise, false.</returns>
    bool ContainsWarning(string warningMessage);
}
