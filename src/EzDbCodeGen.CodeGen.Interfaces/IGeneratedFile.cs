namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a file generated during the code generation process.
/// </summary>
public interface IGeneratedFile
{
    /// <summary>
    /// Gets the path of the generated file.
    /// </summary>
    string FilePath { get; }
    
    /// <summary>
    /// Gets the content of the generated file.
    /// </summary>
    string Content { get; }
    
    /// <summary>
    /// Gets the name of the template that generated this file.
    /// </summary>
    string TemplateName { get; }
    
    /// <summary>
    /// Gets a value indicating whether the file was newly created or overwritten.
    /// </summary>
    bool IsNew { get; }
    
    /// <summary>
    /// Gets the type of file that was generated (e.g., "code", "configuration", "documentation").
    /// </summary>
    string FileType { get; }
    
    /// <summary>
    /// Gets the model object that was used to generate this file.
    /// </summary>
    object? Model { get; }
    
    /// <summary>
    /// Gets the size of the generated file in bytes.
    /// </summary>
    long FileSize { get; }
    
    /// <summary>
    /// Gets the timestamp when the file was generated.
    /// </summary>
    DateTime GeneratedAt { get; }
    
    /// <summary>
    /// Saves the generated file to disk.
    /// </summary>
    /// <param name="overwriteExisting">Whether to overwrite the file if it already exists.</param>
    /// <returns>True if the file was successfully saved; otherwise, false.</returns>
    bool Save(bool overwriteExisting = true);
    
    /// <summary>
    /// Compares the generated file content with the content of an existing file at the same path.
    /// </summary>
    /// <returns>True if the contents are different; otherwise, false.</returns>
    bool HasChanged();
}
