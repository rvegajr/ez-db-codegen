namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a repository that manages reusable code snippets.
/// </summary>
public interface ICodeSnippetRepository
{
    /// <summary>
    /// Gets a code snippet by its name.
    /// </summary>
    /// <param name="snippetName">The name of the snippet to retrieve.</param>
    /// <param name="language">The programming language of the snippet.</param>
    /// <returns>The snippet content, or null if the snippet does not exist.</returns>
    string? GetSnippet(string snippetName, string language);
    
    /// <summary>
    /// Determines whether a snippet with the specified name and language exists in the repository.
    /// </summary>
    /// <param name="snippetName">The name of the snippet to check.</param>
    /// <param name="language">The programming language of the snippet.</param>
    /// <returns>True if the snippet exists; otherwise, false.</returns>
    bool SnippetExists(string snippetName, string language);
    
    /// <summary>
    /// Adds or updates a snippet in the repository.
    /// </summary>
    /// <param name="snippetName">The name of the snippet to add or update.</param>
    /// <param name="language">The programming language of the snippet.</param>
    /// <param name="snippetContent">The content of the snippet.</param>
    /// <param name="description">The description of the snippet.</param>
    void SaveSnippet(string snippetName, string language, string snippetContent, string? description = null);
    
    /// <summary>
    /// Removes a snippet from the repository.
    /// </summary>
    /// <param name="snippetName">The name of the snippet to remove.</param>
    /// <param name="language">The programming language of the snippet.</param>
    /// <returns>True if the snippet was removed; otherwise, false.</returns>
    bool RemoveSnippet(string snippetName, string language);
    
    /// <summary>
    /// Gets all snippet names in the repository for a specific language.
    /// </summary>
    /// <param name="language">The programming language to filter by.</param>
    /// <returns>A collection of snippet names.</returns>
    IReadOnlyCollection<string> GetAllSnippetNames(string language);
    
    /// <summary>
    /// Gets all programming languages that have snippets in the repository.
    /// </summary>
    /// <returns>A collection of programming language names.</returns>
    IReadOnlyCollection<string> GetAllLanguages();
    
    /// <summary>
    /// Gets snippet names that match a specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match against snippet names.</param>
    /// <param name="language">The programming language to filter by, or null for all languages.</param>
    /// <returns>A collection of matching snippet names.</returns>
    IReadOnlyCollection<string> FindSnippets(string pattern, string? language = null);
    
    /// <summary>
    /// Gets metadata about a snippet.
    /// </summary>
    /// <param name="snippetName">The name of the snippet.</param>
    /// <param name="language">The programming language of the snippet.</param>
    /// <returns>A dictionary containing metadata about the snippet, or null if the snippet does not exist.</returns>
    IDictionary<string, object>? GetSnippetMetadata(string snippetName, string language);
    
    /// <summary>
    /// Gets the last modified time for a snippet.
    /// </summary>
    /// <param name="snippetName">The name of the snippet.</param>
    /// <param name="language">The programming language of the snippet.</param>
    /// <returns>The last modified time, or null if the snippet does not exist.</returns>
    DateTime? GetLastModifiedTime(string snippetName, string language);
    
    /// <summary>
    /// Adds a base path to search for snippets.
    /// </summary>
    /// <param name="basePath">The base path to add.</param>
    void AddBasePath(string basePath);
    
    /// <summary>
    /// Gets all base paths configured for this repository.
    /// </summary>
    /// <returns>A collection of base paths.</returns>
    IReadOnlyCollection<string> GetBasePaths();
    
    /// <summary>
    /// Imports snippets from a file or directory.
    /// </summary>
    /// <param name="path">The path to the file or directory to import from.</param>
    /// <param name="language">The programming language to assign to the imported snippets, or null to auto-detect.</param>
    /// <returns>The number of snippets imported.</returns>
    int ImportSnippets(string path, string? language = null);
    
    /// <summary>
    /// Exports snippets to a directory.
    /// </summary>
    /// <param name="directoryPath">The directory to export to.</param>
    /// <param name="language">The programming language to filter by, or null for all languages.</param>
    /// <returns>The number of snippets exported.</returns>
    int ExportSnippets(string directoryPath, string? language = null);
}
