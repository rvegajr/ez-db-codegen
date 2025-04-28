namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for managing file output during the code generation process.
/// </summary>
public interface IFileOutputManager
{
    /// <summary>
    /// Gets or sets the base output directory for generated files.
    /// </summary>
    string OutputDirectory { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to overwrite existing files.
    /// </summary>
    bool OverwriteExisting { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to create directories that don't exist.
    /// </summary>
    bool CreateDirectories { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to use a file hash to determine if a file has changed.
    /// </summary>
    bool UseFileHashing { get; set; }
    
    /// <summary>
    /// Gets or sets the encoding to use for output files.
    /// </summary>
    string FileEncoding { get; set; }
    
    /// <summary>
    /// Writes content to a file at the specified path.
    /// </summary>
    /// <param name="relativePath">The path relative to the output directory.</param>
    /// <param name="content">The content to write.</param>
    /// <returns>An object representing the generated file.</returns>
    IGeneratedFile WriteToFile(string relativePath, string content);
    
    /// <summary>
    /// Creates a generated file object without writing it to disk.
    /// </summary>
    /// <param name="relativePath">The path relative to the output directory.</param>
    /// <param name="content">The content of the file.</param>
    /// <param name="templateName">The name of the template that generated the content.</param>
    /// <param name="model">The model used to generate the content.</param>
    /// <returns>An object representing the generated file.</returns>
    IGeneratedFile CreateGeneratedFile(string relativePath, string content, string templateName, object? model);
    
    /// <summary>
    /// Determines whether a file at the specified path already exists.
    /// </summary>
    /// <param name="relativePath">The path relative to the output directory.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    bool FileExists(string relativePath);
    
    /// <summary>
    /// Determines whether the content is different from the content of an existing file.
    /// </summary>
    /// <param name="relativePath">The path relative to the output directory.</param>
    /// <param name="content">The content to compare.</param>
    /// <returns>True if the content is different or the file does not exist; otherwise, false.</returns>
    bool ContentDiffers(string relativePath, string content);
    
    /// <summary>
    /// Gets the absolute path for a relative path.
    /// </summary>
    /// <param name="relativePath">The path relative to the output directory.</param>
    /// <returns>The absolute path.</returns>
    string GetAbsolutePath(string relativePath);
    
    /// <summary>
    /// Deletes a file at the specified path.
    /// </summary>
    /// <param name="relativePath">The path relative to the output directory.</param>
    /// <returns>True if the file was deleted; otherwise, false.</returns>
    bool DeleteFile(string relativePath);
    
    /// <summary>
    /// Creates a directory at the specified path if it doesn't exist.
    /// </summary>
    /// <param name="relativePath">The path relative to the output directory.</param>
    /// <returns>True if the directory was created; otherwise, false if it already exists.</returns>
    bool EnsureDirectoryExists(string relativePath);
}
