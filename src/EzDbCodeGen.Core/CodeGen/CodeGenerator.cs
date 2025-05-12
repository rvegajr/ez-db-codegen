using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EzDbCodeGen.Interfaces.CodeGen;
using EzDbCodeGen.Interfaces.Schema;
using EzDbCodeGen.Interfaces.TemplateEngine;
using EzDbCodeGen.Interfaces.Utilities;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Core.CodeGen
{
    /// <summary>
    /// Implementation of the ICodeGenerator interface that processes templates against database schemas.
    /// This implementation offers superior code generation capabilities compared to EF Core Power Tools.
    /// </summary>
    public class CodeGenerator : ICodeGenerator
    {
        private readonly ILogger<CodeGenerator> _logger;
        private readonly IFileUtility _fileUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerator"/> class.
        /// </summary>
        /// <param name="templateProcessor">The template processor to use.</param>
        /// <param name="fileUtility">The file utility to use.</param>
        /// <param name="logger">The logger to use.</param>
        public CodeGenerator(
            ITemplateProcessor templateProcessor,
            IFileUtility fileUtility,
            ILogger<CodeGenerator> logger)
        {
            TemplateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
            _fileUtility = fileUtility ?? throw new ArgumentNullException(nameof(fileUtility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            BasePath = AppDomain.CurrentDomain.BaseDirectory;
            OutputPath = Path.Combine(BasePath, "Output");
        }

        /// <inheritdoc/>
        public ITemplateProcessor TemplateProcessor { get; }

        /// <inheritdoc/>
        public string BasePath { get; set; }

        /// <inheritdoc/>
        public string OutputPath { get; set; }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<string>> GenerateCodeAsync(string templatePath, IDatabaseSchema schema, string outputPath, CodeGenerationOptions options)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentException("Template path cannot be null or empty.", nameof(templatePath));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            options ??= CodeGenerationOptions.Default();
            outputPath ??= OutputPath;

            if (!options.DryRun)
            {
                _fileUtility.CreateDirectory(outputPath);
            }

            // Read the template content
            var templateContent = _fileUtility.ReadAllText(templatePath);

            // Generate the code
            return await GenerateCodeFromContentAsync(templateContent, schema, outputPath, options);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<string>> GenerateCodeFromContentAsync(string templateContent, IDatabaseSchema schema, string outputPath, CodeGenerationOptions options)
        {
            if (string.IsNullOrEmpty(templateContent))
            {
                throw new ArgumentException("Template content cannot be null or empty.", nameof(templateContent));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            options ??= CodeGenerationOptions.Default();
            outputPath ??= OutputPath;

            if (!options.DryRun)
            {
                _fileUtility.CreateDirectory(outputPath);
            }

            // Preview the code first (gets a dictionary of file paths and their content)
            var previewResult = await PreviewCodeFromContentAsync(templateContent, schema, options);

            // Write the files
            var generatedFiles = new List<string>();

            // Create a model with the schema and additional options
            foreach (var (filePath, content) in previewResult)
            {
                // Determine the output file path
                string outputFilePath = Path.Combine(outputPath, filePath);

                if (!options.DryRun)
                {
                    // Ensure directory exists
                    string? directory = Path.GetDirectoryName(outputFilePath);
                    if (!string.IsNullOrEmpty(directory) && !_fileUtility.DirectoryExists(directory))
                    {
                        _fileUtility.CreateDirectory(directory);
                    }

                    // Check if we should overwrite existing files
                    bool shouldWrite = true;
                    if (_fileUtility.FileExists(outputFilePath) && !options.OverwriteExistingFiles)
                    {
                        _logger.LogInformation($"Skipping file {outputFilePath} (exists and overwrite is disabled)");
                        shouldWrite = false;
                    }

                    if (shouldWrite)
                    {
                        await _fileUtility.WriteAllTextAsync(outputFilePath, content);
                        _logger.LogInformation($"Generated file: {outputFilePath}");
                    }

                    generatedFiles.Add(outputFilePath);
                }
                else
                {
                    // In dry run mode, still add the files to the result but don't write them
                    _logger.LogInformation($"Would generate file: {outputFilePath}");
                    generatedFiles.Add(outputFilePath);
                }
            }

            return generatedFiles;
        }

        /// <inheritdoc/>
        public async Task<IDictionary<string, string>> PreviewCodeAsync(string templatePath, IDatabaseSchema schema, CodeGenerationOptions options)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentException("Template path cannot be null or empty.", nameof(templatePath));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            // Read the template content
            var templateContent = _fileUtility.ReadAllText(templatePath);

            // Preview the code
            return await PreviewCodeFromContentAsync(templateContent, schema, options);
        }

        /// <inheritdoc/>
        public Task<IDictionary<string, string>> PreviewCodeFromContentAsync(string templateContent, IDatabaseSchema schema, CodeGenerationOptions options)
        {
            if (string.IsNullOrEmpty(templateContent))
            {
                throw new ArgumentException("Template content cannot be null or empty.", nameof(templateContent));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            options ??= CodeGenerationOptions.Default();

            // Create a model with the schema and additional options
            var model = new Dictionary<string, object>
            {
                ["Schema"] = schema,
                ["Options"] = options,
                ["CustomProperties"] = options.CustomProperties
            };

            // Add custom properties directly to the model for easier access in templates
            foreach (var (key, value) in options.CustomProperties)
            {
                model[key] = value;
            }

            // Process the template
            string result = TemplateProcessor.ProcessTemplate(templateContent, model);

            // Parse the result to extract file content (looking for FILE_START and FILE_END markers)
            var files = new Dictionary<string, string>();

            // Pattern to match: {{FILE_START:<filename>}} ... {{FILE_END}}
            var filePattern = new Regex(@"\{\{FILE_START:(.*?)\}\}(.*?)\{\{FILE_END\}\}", RegexOptions.Singleline);
            var matches = filePattern.Matches(result);

            if (matches.Count > 0)
            {
                // Multi-file output (using FILE_START/FILE_END markers)
                foreach (Match match in matches)
                {
                    string fileName = match.Groups[1].Value.Trim();
                    string content = match.Groups[2].Value;
                    
                    // Apply file name transformations based on options
                    fileName = ApplyFileNameOptions(fileName, options);
                    
                    files[fileName] = content;
                }
            }
            else if (options.GeneratePerTable || options.GeneratePerView || 
                     options.GeneratePerStoredProcedure || options.GeneratePerFunction)
            {
                // No file markers found, but options indicate we should generate per-object files
                // This requires custom template processing for each object type
                
                if (options.GeneratePerTable)
                {
                    foreach (var table in schema.Tables.Where(t => !t.IsView))
                    {
                        if (ShouldIncludeTable(table, options))
                        {
                            string fileName = $"{options.FileNamePrefix}{table.Name}{options.FileNameSuffix}{options.FileExtension}";
                            var tableModel = new Dictionary<string, object>(model)
                            {
                                ["Table"] = table,
                                ["CurrentTable"] = table
                            };
                            
                            string content = TemplateProcessor.ProcessTemplate(templateContent, tableModel);
                            files[fileName] = content;
                        }
                    }
                }
                
                if (options.GeneratePerView)
                {
                    foreach (var view in schema.Tables.Where(t => t.IsView))
                    {
                        if (ShouldIncludeTable(view, options))
                        {
                            string fileName = $"{options.FileNamePrefix}{view.Name}{options.FileNameSuffix}{options.FileExtension}";
                            var viewModel = new Dictionary<string, object>(model)
                            {
                                ["View"] = view,
                                ["CurrentView"] = view
                            };
                            
                            string content = TemplateProcessor.ProcessTemplate(templateContent, viewModel);
                            files[fileName] = content;
                        }
                    }
                }
                
                if (options.GeneratePerStoredProcedure)
                {
                    foreach (var procedure in schema.StoredProcedures)
                    {
                        string fileName = $"{options.FileNamePrefix}{procedure.Name}{options.FileNameSuffix}{options.FileExtension}";
                        var procedureModel = new Dictionary<string, object>(model)
                        {
                            ["StoredProcedure"] = procedure,
                            ["CurrentStoredProcedure"] = procedure
                        };
                        
                        string content = TemplateProcessor.ProcessTemplate(templateContent, procedureModel);
                        files[fileName] = content;
                    }
                }
                
                if (options.GeneratePerFunction)
                {
                    foreach (var function in schema.Functions)
                    {
                        string fileName = $"{options.FileNamePrefix}{function.Name}{options.FileNameSuffix}{options.FileExtension}";
                        var functionModel = new Dictionary<string, object>(model)
                        {
                            ["Function"] = function,
                            ["CurrentFunction"] = function
                        };
                        
                        string content = TemplateProcessor.ProcessTemplate(templateContent, functionModel);
                        files[fileName] = content;
                    }
                }
            }
            else
            {
                // Single file output (no markers and no per-object options)
                string fileName = $"{options.FileNamePrefix}{options.SingleFileName}{options.FileNameSuffix}{options.FileExtension}";
                files[fileName] = result;
            }

            return Task.FromResult<IDictionary<string, string>>(files);
        }

        /// <inheritdoc/>
        public void RegisterHelper(string name, Delegate helper)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Helper name cannot be null or empty.", nameof(name));
            }

            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            TemplateProcessor.RegisterHelper(name, helper);
        }

        /// <inheritdoc/>
        public void RegisterBlockHelper(string name, Delegate helper)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Helper name cannot be null or empty.", nameof(name));
            }

            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            TemplateProcessor.RegisterBlockHelper(name, helper);
        }

        private string ApplyFileNameOptions(string fileName, CodeGenerationOptions options)
        {
            // Add prefix if specified
            if (!string.IsNullOrEmpty(options.FileNamePrefix) && !fileName.StartsWith(options.FileNamePrefix))
            {
                fileName = options.FileNamePrefix + fileName;
            }
            
            // Add suffix (before extension) if specified
            if (!string.IsNullOrEmpty(options.FileNameSuffix))
            {
                string extension = Path.GetExtension(fileName);
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                
                if (!nameWithoutExtension.EndsWith(options.FileNameSuffix))
                {
                    fileName = nameWithoutExtension + options.FileNameSuffix + extension;
                }
            }
            
            // Ensure it has the right extension if specified
            if (!string.IsNullOrEmpty(options.FileExtension) && 
                !fileName.EndsWith(options.FileExtension, StringComparison.OrdinalIgnoreCase))
            {
                fileName = Path.ChangeExtension(fileName, options.FileExtension.TrimStart('.'));
            }
            
            return fileName;
        }

        private bool ShouldIncludeTable(ITable table, CodeGenerationOptions options)
        {
            // Check if the schema should be included
            if (options.IncludeSchemas.Any() && !options.IncludeSchemas.Contains(table.Schema, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            // Check if the schema should be excluded
            if (options.ExcludeSchemas.Contains(table.Schema, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            // Check if the table should be included
            if (options.IncludeTables.Any())
            {
                // Check for specific table names or patterns
                bool shouldInclude = false;
                foreach (var pattern in options.IncludeTables)
                {
                    if (MatchesWildcard(table.Name, pattern))
                    {
                        shouldInclude = true;
                        break;
                    }
                }
                
                if (!shouldInclude)
                {
                    return false;
                }
            }

            // Check if the table should be excluded
            foreach (var pattern in options.ExcludeTables)
            {
                if (MatchesWildcard(table.Name, pattern))
                {
                    return false;
                }
            }

            return true;
        }

        private bool MatchesWildcard(string input, string pattern)
        {
            // Convert wildcard pattern to regex
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";
            
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
