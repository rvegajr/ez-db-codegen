using System.Collections.Generic;
using System.Threading.Tasks;

namespace EzDbCodeGen.Interfaces.Utilities
{
    /// <summary>
    /// Defines utilities for file operations.
    /// </summary>
    public interface IFileUtility
    {
        /// <summary>
        /// Reads text from a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The contents of the file.</returns>
        string ReadAllText(string path);

        /// <summary>
        /// Asynchronously reads text from a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the contents of the file.</returns>
        Task<string> ReadAllTextAsync(string path);

        /// <summary>
        /// Writes text to a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="contents">The contents to write to the file.</param>
        void WriteAllText(string path, string contents);

        /// <summary>
        /// Asynchronously writes text to a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="contents">The contents to write to the file.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task WriteAllTextAsync(string path, string contents);

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Checks if a directory exists.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>True if the directory exists, false otherwise.</returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        void CreateDirectory(string path);

        /// <summary>
        /// Gets all files in a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <param name="searchPattern">The search pattern to match against file names.</param>
        /// <param name="recursive">Whether to include files in subdirectories.</param>
        /// <returns>A collection of file paths.</returns>
        IEnumerable<string> GetFiles(string path, string searchPattern = "*", bool recursive = false);

        /// <summary>
        /// Gets all directories in a directory.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <param name="searchPattern">The search pattern to match against directory names.</param>
        /// <param name="recursive">Whether to include directories in subdirectories.</param>
        /// <returns>A collection of directory paths.</returns>
        IEnumerable<string> GetDirectories(string path, string searchPattern = "*", bool recursive = false);

        /// <summary>
        /// Gets the file name from a path.
        /// </summary>
        /// <param name="path">The path to get the file name from.</param>
        /// <returns>The file name.</returns>
        string GetFileName(string path);

        /// <summary>
        /// Gets the directory name from a path.
        /// </summary>
        /// <param name="path">The path to get the directory name from.</param>
        /// <returns>The directory name.</returns>
        string GetDirectoryName(string path);

        /// <summary>
        /// Combines multiple path components into a single path.
        /// </summary>
        /// <param name="paths">The path components to combine.</param>
        /// <returns>The combined path.</returns>
        string CombinePath(params string[] paths);

        /// <summary>
        /// Gets the absolute path from a path that might be relative.
        /// </summary>
        /// <param name="path">The path to get the absolute path for.</param>
        /// <returns>The absolute path.</returns>
        string GetAbsolutePath(string path);

        /// <summary>
        /// Gets the relative path from one path to another.
        /// </summary>
        /// <param name="fromPath">The path to get the relative path from.</param>
        /// <param name="toPath">The path to get the relative path to.</param>
        /// <returns>The relative path.</returns>
        string GetRelativePath(string fromPath, string toPath);
    }
}
