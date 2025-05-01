using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EzDbCodeGen.Core.CodeGeneration;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine;

namespace EzDbCodeGen.CodeGeneration
{
    /// <summary>
    /// Implements a code generator that generates code from a database schema using templates.
    /// </summary>
    public class CodeGenerator : ICodeGenerator
    {
        private readonly ILogger _logger;
        private readonly ITemplateProcessor _templateProcessor;
        private readonly CodeGenerationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerator"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="templateProcessor">The template processor to use.</param>
        /// <param name="options">The code generation options to use.</param>
        public CodeGenerator(
            ILogger logger,
            ITemplateProcessor templateProcessor,
            CodeGenerationOptions options = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
            _options = options ?? new CodeGenerationOptions();
        }

        /// <inheritdoc/>
        public async Task GenerateAsync(
            IDatabaseSchema schema,
            string templatePath,
            string outputPath,
            CodeGenerationOptions options = null)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (string.IsNullOrWhiteSpace(templatePath))
            {
                throw new ArgumentException("Template path cannot be empty.", nameof(templatePath));
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("Output path cannot be empty.", nameof(outputPath));
            }

            // Use the provided options or fall back to the default options
            var generationOptions = options ?? _options;

            try
            {
                _logger.Info($"Starting code generation from schema with {schema.Tables.Count} tables...");
                _logger.Info($"Template path: {templatePath}");
                _logger.Info($"Output path: {outputPath}");

                // Ensure the output directory exists
                Directory.CreateDirectory(outputPath);

                // Get all template files
                var templateFiles = GetTemplateFiles(templatePath, generationOptions.TemplatePattern);
                _logger.Info($"Found {templateFiles.Count} template files.");

                // Generate code for each table
                foreach (var table in schema.Tables)
                {
                    await GenerateForTableAsync(table, templateFiles, outputPath, generationOptions);
                }

                // Generate additional files as needed (e.g. DbContext, repositories, etc.)
                await GenerateAdditionalFilesAsync(schema, templateFiles, outputPath, generationOptions);

                _logger.Info("Code generation completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating code: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateForTableAsync(
            ITable table,
            IList<string> templateFiles,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug($"Generating code for table: {table.Schema}.{table.Name}");

                // Check if the table should be included
                if (!ShouldIncludeTable(table, options))
                {
                    _logger.Debug($"Skipping excluded table: {table.Schema}.{table.Name}");
                    return;
                }

                // Find entity template
                var entityTemplate = FindTemplate(templateFiles, "entity", options);
                if (entityTemplate == null)
                {
                    _logger.Warning($"No entity template found for table: {table.Schema}.{table.Name}");
                    return;
                }

                // Prepare the model data
                var model = new Dictionary<string, object>
                {
                    ["Table"] = table,
                    ["Options"] = options,
                    ["Namespace"] = options.Namespace,
                    ["UseDataAnnotations"] = options.UseDataAnnotations,
                    ["UseFluentApi"] = options.UseFluentApi,
                    ["GenerateNavigationProperties"] = options.GenerateNavigationProperties
                };

                // Process the template
                var entityCode = _templateProcessor.ProcessTemplateFile(entityTemplate, model);

                // Generate the output file path
                var outputFileName = GetOutputFileName(table, options);
                var outputFilePath = Path.Combine(outputPath, outputFileName);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

                // Write the generated code to the output file
                await File.WriteAllTextAsync(outputFilePath, entityCode);

                _logger.Info($"Generated code for table {table.Schema}.{table.Name} -> {outputFilePath}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating code for table {table.Schema}.{table.Name}: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateAdditionalFilesAsync(
            IDatabaseSchema schema,
            IList<string> templateFiles,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug("Generating additional files...");

                // Find DbContext template
                var dbContextTemplate = FindTemplate(templateFiles, "dbcontext", options);
                if (dbContextTemplate != null)
                {
                    await GenerateDbContextAsync(schema, dbContextTemplate, outputPath, options);
                }

                // Find repository interface template
                var repositoryInterfaceTemplate = FindTemplate(templateFiles, "repository-interface", options);
                if (repositoryInterfaceTemplate != null)
                {
                    await GenerateRepositoryInterfaceAsync(schema, repositoryInterfaceTemplate, outputPath, options);
                }

                // Find repository implementation template
                var repositoryImplTemplate = FindTemplate(templateFiles, "repository-impl", options);
                if (repositoryImplTemplate != null)
                {
                    await GenerateRepositoryImplementationAsync(schema, repositoryImplTemplate, outputPath, options);
                }

                // Find unit of work template
                var unitOfWorkTemplate = FindTemplate(templateFiles, "unit-of-work", options);
                if (unitOfWorkTemplate != null)
                {
                    await GenerateUnitOfWorkAsync(schema, unitOfWorkTemplate, outputPath, options);
                }

                // Find service interface template
                var serviceInterfaceTemplate = FindTemplate(templateFiles, "service-interface", options);
                if (serviceInterfaceTemplate != null)
                {
                    await GenerateServiceInterfaceAsync(schema, serviceInterfaceTemplate, outputPath, options);
                }

                // Find service implementation template
                var serviceImplTemplate = FindTemplate(templateFiles, "service-impl", options);
                if (serviceImplTemplate != null)
                {
                    await GenerateServiceImplementationAsync(schema, serviceImplTemplate, outputPath, options);
                }

                // Find controller template
                var controllerTemplate = FindTemplate(templateFiles, "controller", options);
                if (controllerTemplate != null)
                {
                    await GenerateControllersAsync(schema, controllerTemplate, outputPath, options);
                }

                _logger.Info("Generated additional files successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating additional files: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateDbContextAsync(
            IDatabaseSchema schema,
            string templateFile,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug("Generating DbContext...");

                // Prepare the model data
                var model = new Dictionary<string, object>
                {
                    ["Schema"] = schema,
                    ["Options"] = options,
                    ["Namespace"] = options.Namespace,
                    ["UseDataAnnotations"] = options.UseDataAnnotations,
                    ["UseFluentApi"] = options.UseFluentApi,
                    ["ContextClassName"] = options.ContextClassName ?? $"{options.NamePrefix}DbContext"
                };

                // Process the template
                var dbContextCode = _templateProcessor.ProcessTemplateFile(templateFile, model);

                // Generate the output file path
                var outputFileName = $"{options.ContextClassName ?? $"{options.NamePrefix}DbContext"}.cs";
                var outputFilePath = Path.Combine(outputPath, outputFileName);

                // Write the generated code to the output file
                await File.WriteAllTextAsync(outputFilePath, dbContextCode);

                _logger.Info($"Generated DbContext -> {outputFilePath}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating DbContext: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateRepositoryInterfaceAsync(
            IDatabaseSchema schema,
            string templateFile,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug("Generating repository interfaces...");

                // Create the repository directory
                var repositoryDir = Path.Combine(outputPath, "Repositories");
                Directory.CreateDirectory(repositoryDir);

                // Generate generic repository interface
                var genericModel = new Dictionary<string, object>
                {
                    ["Schema"] = schema,
                    ["Options"] = options,
                    ["Namespace"] = options.Namespace,
                    ["IsGeneric"] = true
                };

                var genericRepoCode = _templateProcessor.ProcessTemplateFile(templateFile, genericModel);
                var genericRepoPath = Path.Combine(repositoryDir, "IRepository.cs");
                await File.WriteAllTextAsync(genericRepoPath, genericRepoCode);

                // Generate specific repository interfaces for each table
                foreach (var table in schema.Tables)
                {
                    if (!ShouldIncludeTable(table, options))
                    {
                        continue;
                    }

                    var modelName = GetEntityName(table, options);
                    var model = new Dictionary<string, object>
                    {
                        ["Schema"] = schema,
                        ["Table"] = table,
                        ["Options"] = options,
                        ["Namespace"] = options.Namespace,
                        ["ModelName"] = modelName,
                        ["IsGeneric"] = false
                    };

                    var repoCode = _templateProcessor.ProcessTemplateFile(templateFile, model);
                    var repoPath = Path.Combine(repositoryDir, $"I{modelName}Repository.cs");
                    await File.WriteAllTextAsync(repoPath, repoCode);
                }

                _logger.Info($"Generated repository interfaces -> {repositoryDir}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating repository interfaces: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateRepositoryImplementationAsync(
            IDatabaseSchema schema,
            string templateFile,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug("Generating repository implementations...");

                // Create the repository directory
                var repositoryDir = Path.Combine(outputPath, "Repositories", "Implementations");
                Directory.CreateDirectory(repositoryDir);

                // Generate generic repository implementation
                var genericModel = new Dictionary<string, object>
                {
                    ["Schema"] = schema,
                    ["Options"] = options,
                    ["Namespace"] = options.Namespace,
                    ["IsGeneric"] = true,
                    ["ContextClassName"] = options.ContextClassName ?? $"{options.NamePrefix}DbContext"
                };

                var genericRepoCode = _templateProcessor.ProcessTemplateFile(templateFile, genericModel);
                var genericRepoPath = Path.Combine(repositoryDir, "Repository.cs");
                await File.WriteAllTextAsync(genericRepoPath, genericRepoCode);

                // Generate specific repository implementations for each table
                foreach (var table in schema.Tables)
                {
                    if (!ShouldIncludeTable(table, options))
                    {
                        continue;
                    }

                    var modelName = GetEntityName(table, options);
                    var model = new Dictionary<string, object>
                    {
                        ["Schema"] = schema,
                        ["Table"] = table,
                        ["Options"] = options,
                        ["Namespace"] = options.Namespace,
                        ["ModelName"] = modelName,
                        ["IsGeneric"] = false,
                        ["ContextClassName"] = options.ContextClassName ?? $"{options.NamePrefix}DbContext"
                    };

                    var repoCode = _templateProcessor.ProcessTemplateFile(templateFile, model);
                    var repoPath = Path.Combine(repositoryDir, $"{modelName}Repository.cs");
                    await File.WriteAllTextAsync(repoPath, repoCode);
                }

                _logger.Info($"Generated repository implementations -> {repositoryDir}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating repository implementations: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateUnitOfWorkAsync(
            IDatabaseSchema schema,
            string templateFile,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug("Generating unit of work...");

                // Create the directory
                var unitOfWorkDir = Path.Combine(outputPath, "UnitOfWork");
                Directory.CreateDirectory(unitOfWorkDir);

                // Generate interface
                var interfaceModel = new Dictionary<string, object>
                {
                    ["Schema"] = schema,
                    ["Options"] = options,
                    ["Namespace"] = options.Namespace,
                    ["IsInterface"] = true,
                    ["Tables"] = GetIncludedTables(schema, options)
                };

                var interfaceCode = _templateProcessor.ProcessTemplateFile(templateFile, interfaceModel);
                var interfacePath = Path.Combine(unitOfWorkDir, "IUnitOfWork.cs");
                await File.WriteAllTextAsync(interfacePath, interfaceCode);

                // Generate implementation
                var implModel = new Dictionary<string, object>
                {
                    ["Schema"] = schema,
                    ["Options"] = options,
                    ["Namespace"] = options.Namespace,
                    ["IsInterface"] = false,
                    ["Tables"] = GetIncludedTables(schema, options),
                    ["ContextClassName"] = options.ContextClassName ?? $"{options.NamePrefix}DbContext"
                };

                var implCode = _templateProcessor.ProcessTemplateFile(templateFile, implModel);
                var implPath = Path.Combine(unitOfWorkDir, "UnitOfWork.cs");
                await File.WriteAllTextAsync(implPath, implCode);

                _logger.Info($"Generated unit of work -> {unitOfWorkDir}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating unit of work: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateServiceInterfaceAsync(
            IDatabaseSchema schema,
            string templateFile,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug("Generating service interfaces...");

                // Create the service directory
                var serviceDir = Path.Combine(outputPath, "Services");
                Directory.CreateDirectory(serviceDir);

                // Generate generic service interface
                var genericModel = new Dictionary<string, object>
                {
                    ["Schema"] = schema,
                    ["Options"] = options,
                    ["Namespace"] = options.Namespace,
                    ["IsGeneric"] = true
                };

                var genericServiceCode = _templateProcessor.ProcessTemplateFile(templateFile, genericModel);
                var genericServicePath = Path.Combine(serviceDir, "IService.cs");
                await File.WriteAllTextAsync(genericServicePath, genericServiceCode);

                // Generate specific service interfaces for each table
                foreach (var table in schema.Tables)
                {
                    if (!ShouldIncludeTable(table, options))
                    {
                        continue;
                    }

                    var modelName = GetEntityName(table, options);
                    var model = new Dictionary<string, object>
                    {
                        ["Schema"] = schema,
                        ["Table"] = table,
                        ["Options"] = options,
                        ["Namespace"] = options.Namespace,
                        ["ModelName"] = modelName,
                        ["IsGeneric"] = false
                    };

                    var serviceCode = _templateProcessor.ProcessTemplateFile(templateFile, model);
                    var servicePath = Path.Combine(serviceDir, $"I{modelName}Service.cs");
                    await File.WriteAllTextAsync(servicePath, serviceCode);
                }

                _logger.Info($"Generated service interfaces -> {serviceDir}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating service interfaces: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateServiceImplementationAsync(
            IDatabaseSchema schema,
            string templateFile,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug("Generating service implementations...");

                // Create the service implementation directory
                var serviceDir = Path.Combine(outputPath, "Services", "Implementations");
                Directory.CreateDirectory(serviceDir);

                // Generate generic service implementation
                var genericModel = new Dictionary<string, object>
                {
                    ["Schema"] = schema,
                    ["Options"] = options,
                    ["Namespace"] = options.Namespace,
                    ["IsGeneric"] = true
                };

                var genericServiceCode = _templateProcessor.ProcessTemplateFile(templateFile, genericModel);
                var genericServicePath = Path.Combine(serviceDir, "Service.cs");
                await File.WriteAllTextAsync(genericServicePath, genericServiceCode);

                // Generate specific service implementations for each table
                foreach (var table in schema.Tables)
                {
                    if (!ShouldIncludeTable(table, options))
                    {
                        continue;
                    }

                    var modelName = GetEntityName(table, options);
                    var model = new Dictionary<string, object>
                    {
                        ["Schema"] = schema,
                        ["Table"] = table,
                        ["Options"] = options,
                        ["Namespace"] = options.Namespace,
                        ["ModelName"] = modelName,
                        ["IsGeneric"] = false
                    };

                    var serviceCode = _templateProcessor.ProcessTemplateFile(templateFile, model);
                    var servicePath = Path.Combine(serviceDir, $"{modelName}Service.cs");
                    await File.WriteAllTextAsync(servicePath, serviceCode);
                }

                _logger.Info($"Generated service implementations -> {serviceDir}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating service implementations: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private async Task GenerateControllersAsync(
            IDatabaseSchema schema,
            string templateFile,
            string outputPath,
            CodeGenerationOptions options)
        {
            try
            {
                _logger.Debug("Generating controllers...");

                // Create the controllers directory
                var controllerDir = Path.Combine(outputPath, "Controllers");
                Directory.CreateDirectory(controllerDir);

                // Generate controllers for each table
                foreach (var table in schema.Tables)
                {
                    if (!ShouldIncludeTable(table, options))
                    {
                        continue;
                    }

                    var modelName = GetEntityName(table, options);
                    var model = new Dictionary<string, object>
                    {
                        ["Schema"] = schema,
                        ["Table"] = table,
                        ["Options"] = options,
                        ["Namespace"] = options.Namespace,
                        ["ModelName"] = modelName,
                        ["ApiVersion"] = options.ApiVersion ?? "1",
                        ["RoutePrefix"] = options.RoutePrefix ?? "api"
                    };

                    var controllerCode = _templateProcessor.ProcessTemplateFile(templateFile, model);
                    var controllerPath = Path.Combine(controllerDir, $"{modelName}Controller.cs");
                    await File.WriteAllTextAsync(controllerPath, controllerCode);
                }

                _logger.Info($"Generated controllers -> {controllerDir}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating controllers: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        private IList<string> GetTemplateFiles(string templatePath, string pattern)
        {
            // Check if the template path is a file or directory
            if (File.Exists(templatePath))
            {
                return new List<string> { templatePath };
            }

            if (!Directory.Exists(templatePath))
            {
                throw new DirectoryNotFoundException($"Template path not found: {templatePath}");
            }

            // Get all template files matching the pattern
            var searchPattern = string.IsNullOrWhiteSpace(pattern) ? "*.hbs" : pattern;
            var templateFiles = Directory.GetFiles(templatePath, searchPattern, SearchOption.AllDirectories);
            
            return new List<string>(templateFiles);
        }

        private string FindTemplate(IList<string> templateFiles, string templateType, CodeGenerationOptions options)
        {
            // Look for a template that matches the template type
            var templateTypeLower = templateType.ToLowerInvariant();
            var templateSuffix = options.TemplateSuffix ?? string.Empty;
            
            foreach (var templateFile in templateFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(templateFile);
                
                if (fileName.Equals(templateTypeLower, StringComparison.OrdinalIgnoreCase) ||
                    fileName.Equals($"{templateTypeLower}{templateSuffix}", StringComparison.OrdinalIgnoreCase))
                {
                    return templateFile;
                }
            }
            
            return null;
        }

        private bool ShouldIncludeTable(ITable table, CodeGenerationOptions options)
        {
            // Check if the table should be excluded based on the configuration
            if (options.ExcludedTables != null && options.ExcludedTables.Contains(table.Name))
            {
                return false;
            }
            
            if (options.ExcludedSchemas != null && options.ExcludedSchemas.Contains(table.Schema))
            {
                return false;
            }
            
            if (options.IncludedTables != null && options.IncludedTables.Count > 0)
            {
                return options.IncludedTables.Contains(table.Name);
            }
            
            if (options.IncludedSchemas != null && options.IncludedSchemas.Count > 0)
            {
                return options.IncludedSchemas.Contains(table.Schema);
            }
            
            return true;
        }

        private IList<ITable> GetIncludedTables(IDatabaseSchema schema, CodeGenerationOptions options)
        {
            var includedTables = new List<ITable>();
            
            foreach (var table in schema.Tables)
            {
                if (ShouldIncludeTable(table, options))
                {
                    includedTables.Add(table);
                }
            }
            
            return includedTables;
        }

        private string GetEntityName(ITable table, CodeGenerationOptions options)
        {
            // Generate the entity name based on configuration
            var name = table.Name;
            
            // Remove prefix if specified
            if (!string.IsNullOrWhiteSpace(options.TablePrefix) && name.StartsWith(options.TablePrefix))
            {
                name = name.Substring(options.TablePrefix.Length);
            }
            
            // Remove suffix if specified
            if (!string.IsNullOrWhiteSpace(options.TableSuffix) && name.EndsWith(options.TableSuffix))
            {
                name = name.Substring(0, name.Length - options.TableSuffix.Length);
            }
            
            // Convert to singular if specified
            if (options.SingularizeEntityNames && name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - 1);
            }
            
            // Add prefix if specified
            if (!string.IsNullOrWhiteSpace(options.EntityPrefix))
            {
                name = options.EntityPrefix + name;
            }
            
            // Add suffix if specified
            if (!string.IsNullOrWhiteSpace(options.EntitySuffix))
            {
                name = name + options.EntitySuffix;
            }
            
            return name;
        }

        private string GetOutputFileName(ITable table, CodeGenerationOptions options)
        {
            // Generate the output file name based on configuration
            var entityName = GetEntityName(table, options);
            var outputFileName = $"{entityName}.cs";
            
            // Add subdirectory if specified
            if (!string.IsNullOrWhiteSpace(options.OutputSubDirectory))
            {
                outputFileName = Path.Combine(options.OutputSubDirectory, outputFileName);
            }
            
            // Add models directory
            outputFileName = Path.Combine("Models", outputFileName);
            
            return outputFileName;
        }
    }
}
