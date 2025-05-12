using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EzDbCodeGen.Interfaces.Utilities;

namespace EzDbCodeGen.Core.Utilities
{
    /// <summary>
    /// Implementation of the IFileUtility interface providing file system operations.
    /// </summary>
    public class FileUtility : IFileUtility
    {
        /// <inheritdoc/>
        public string ReadAllText(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            return File.ReadAllText(path);
        }

        /// <inheritdoc/>
        public async Task<string> ReadAllTextAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            using var reader = File.OpenText(path);
            return await reader.ReadToEndAsync();
        }

        /// <inheritdoc/>
        public void WriteAllText(string path, string contents)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            // Ensure the directory exists
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, contents ?? string.Empty);
        }

        /// <inheritdoc/>
        public async Task WriteAllTextAsync(string path, string contents)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            // Ensure the directory exists
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var writer = File.CreateText(path);
            await writer.WriteAsync(contents ?? string.Empty);
        }

        /// <inheritdoc/>
        public bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            return File.Exists(path);
        }

        /// <inheritdoc/>
        public bool DirectoryExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            return Directory.Exists(path);
        }

        /// <inheritdoc/>
        public void CreateDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            Directory.CreateDirectory(path);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetFiles(string path, string searchPattern = "*", bool recursive = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            searchPattern ??= "*";
            
            return Directory.GetFiles(
                path, 
                searchPattern, 
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetDirectories(string path, string searchPattern = "*", bool recursive = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            searchPattern ??= "*";
            
            return Directory.GetDirectories(
                path, 
                searchPattern, 
                recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        /// <inheritdoc/>
        public string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            return Path.GetFileName(path);
        }

        /// <inheritdoc/>
        public string GetDirectoryName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            return Path.GetDirectoryName(path) ?? string.Empty;
        }

        /// <inheritdoc/>
        public string CombinePath(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
            {
                throw new ArgumentException("At least one path component is required.", nameof(paths));
            }

            return Path.Combine(paths);
        }

        /// <inheritdoc/>
        public string GetAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            return Path.GetFullPath(path);
        }

        /// <inheritdoc/>
        public string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentException("From path cannot be null or empty.", nameof(fromPath));
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentException("To path cannot be null or empty.", nameof(toPath));
            }

            return Path.GetRelativePath(fromPath, toPath);
        }
    }
}
