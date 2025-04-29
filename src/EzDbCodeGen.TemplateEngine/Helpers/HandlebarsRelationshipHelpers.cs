using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine.Helpers;
using HandlebarsDotNet;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Provides relationship helpers for Handlebars templates.
    /// </summary>
    public class HandlebarsRelationshipHelpers : IRelationshipHelpers, IHelperRegistration
    {
        /// <inheritdoc/>
        public void RegisterHelpers(ITemplateEngine templateEngine)
        {
            // Register helpers for one-to-many relationships
            RegisterOneToManyHelpers(templateEngine);
            
            // Register helpers for one-to-one relationships
            RegisterOneToOneHelpers(templateEngine);
            
            // Register helpers for many-to-many relationships
            RegisterManyToManyHelpers(templateEngine);
            
            // Register helpers for inheritance relationships
            RegisterInheritanceHelpers(templateEngine);
            
            // Register navigation property helpers
            RegisterNavigationPropertyHelpers(templateEngine);
            
            // Register relationship naming helpers
            RegisterRelationshipNamingHelpers(templateEngine);
        }

        private void RegisterOneToManyHelpers(ITemplateEngine templateEngine)
        {
            // Register helper to get all one-to-many relationships
            templateEngine.RegisterHelper("oneToManyRelationships", (context, options, arguments, blockParams) => {
                if (context is ITable table && options != null)
                {
                    var relationships = GetOneToManyRelationships(table);
                    if (relationships.Count == 0)
                    {
                        // If there are no one-to-many relationships, execute the else block if provided
                        return options.Inverse(context);
                    }
                    
                    // Execute the block for each one-to-many relationship
                    var result = new StringBuilder();
                    foreach (var relationship in relationships)
                    {
                        result.Append(options.Fn(relationship));
                    }
                    
                    return result.ToString();
                }
                
                // If the context is not a table, execute the else block if provided
                return options?.Inverse(context) ?? string.Empty;
            });
            
            // Register helper to check if a table has any one-to-many relationships
            templateEngine.RegisterHelper("hasOneToManyRelationships", (context) => {
                if (context is ITable table)
                {
                    return GetOneToManyRelationships(table).Count > 0;
                }
                
                return false;
            });
            
            // Register helper to get the "many" side of a relationship
            templateEngine.RegisterHelper("getManyTable", (context) => {
                if (context is IRelationship relationship && relationship.RelationshipType == RelationshipType.OneToMany)
                {
                    return relationship.TargetTable;
                }
                
                return null;
            });
            
            // Register helper to get the "one" side of a relationship
            templateEngine.RegisterHelper("getOneTable", (context) => {
                if (context is IRelationship relationship && relationship.RelationshipType == RelationshipType.OneToMany)
                {
                    return relationship.SourceTable;
                }
                
                return null;
            });
        }

        private void RegisterOneToOneHelpers(ITemplateEngine templateEngine)
        {
            // Register helper to get all one-to-one relationships
            templateEngine.RegisterHelper("oneToOneRelationships", (context, options, arguments, blockParams) => {
                if (context is ITable table && options != null)
                {
                    var relationships = GetOneToOneRelationships(table);
                    if (relationships.Count == 0)
                    {
                        // If there are no one-to-one relationships, execute the else block if provided
                        return options.Inverse(context);
                    }
                    
                    // Execute the block for each one-to-one relationship
                    var result = new StringBuilder();
                    foreach (var relationship in relationships)
                    {
                        result.Append(options.Fn(relationship));
                    }
                    
                    return result.ToString();
                }
                
                // If the context is not a table, execute the else block if provided
                return options?.Inverse(context) ?? string.Empty;
            });
            
            // Register helper to check if a table has any one-to-one relationships
            templateEngine.RegisterHelper("hasOneToOneRelationships", (context) => {
                if (context is ITable table)
                {
                    return GetOneToOneRelationships(table).Count > 0;
                }
                
                return false;
            });
            
            // Register helper to get the principal side of a one-to-one relationship
            templateEngine.RegisterHelper("getPrincipalTable", (context) => {
                if (context is IRelationship relationship && relationship.RelationshipType == RelationshipType.OneToOne)
                {
                    return relationship.IsSelfReferencing ? relationship.SourceTable : relationship.TargetTable;
                }
                
                return null;
            });
            
            // Register helper to get the dependent side of a one-to-one relationship
            templateEngine.RegisterHelper("getDependentTable", (context) => {
                if (context is IRelationship relationship && relationship.RelationshipType == RelationshipType.OneToOne)
                {
                    return relationship.IsSelfReferencing ? relationship.TargetTable : relationship.SourceTable;
                }
                
                return null;
            });
        }

        private void RegisterManyToManyHelpers(ITemplateEngine templateEngine)
        {
            // Register helper to get all many-to-many relationships
            templateEngine.RegisterHelper("manyToManyRelationships", (context, options, arguments, blockParams) => {
                if (context is ITable table && options != null)
                {
                    var relationships = GetManyToManyRelationships(table);
                    if (relationships.Count == 0)
                    {
                        // If there are no many-to-many relationships, execute the else block if provided
                        return options.Inverse(context);
                    }
                    
                    // Execute the block for each many-to-many relationship
                    var result = new StringBuilder();
                    foreach (var relationship in relationships)
                    {
                        result.Append(options.Fn(relationship));
                    }
                    
                    return result.ToString();
                }
                
                // If the context is not a table, execute the else block if provided
                return options?.Inverse(context) ?? string.Empty;
            });
            
            // Register helper to check if a table has any many-to-many relationships
            templateEngine.RegisterHelper("hasManyToManyRelationships", (context) => {
                if (context is ITable table)
                {
                    return GetManyToManyRelationships(table).Count > 0;
                }
                
                return false;
            });
            
            // Register helper to get the junction table of a many-to-many relationship
            templateEngine.RegisterHelper("getJunctionTable", (context) => {
                if (context is IRelationship relationship && relationship.RelationshipType == RelationshipType.ManyToMany)
                {
                    return relationship.JunctionTable;
                }
                
                return null;
            });
            
            // Register helper to get the left side table of a many-to-many relationship
            templateEngine.RegisterHelper("getLeftTable", (context) => {
                if (context is IRelationship relationship && relationship.RelationshipType == RelationshipType.ManyToMany)
                {
                    return relationship.SourceTable;
                }
                
                return null;
            });
            
            // Register helper to get the right side table of a many-to-many relationship
            templateEngine.RegisterHelper("getRightTable", (context) => {
                if (context is IRelationship relationship && relationship.RelationshipType == RelationshipType.ManyToMany)
                {
                    return relationship.TargetTable;
                }
                
                return null;
            });
        }

        private void RegisterInheritanceHelpers(ITemplateEngine templateEngine)
        {
            // Register helper to get the base table of a table
            templateEngine.RegisterHelper("baseTable", (context) => {
                if (context is ITable table)
                {
                    var baseTableRelationship = GetInheritanceRelationship(table);
                    if (baseTableRelationship != null && baseTableRelationship.RelationshipType == RelationshipType.Inheritance)
                    {
                        return baseTableRelationship.TargetTable;
                    }
                }
                
                return null;
            });
            
            // Register helper to check if a table has a base table (is derived)
            templateEngine.RegisterHelper("hasBaseTable", (context) => {
                if (context is ITable table)
                {
                    var baseTableRelationship = GetInheritanceRelationship(table);
                    return baseTableRelationship != null && baseTableRelationship.RelationshipType == RelationshipType.Inheritance;
                }
                
                return false;
            });
            
            // Register helper to get all derived tables of a table
            templateEngine.RegisterHelper("derivedTables", (context, options, arguments, blockParams) => {
                if (context is ITable table && options != null)
                {
                    var derivedTables = GetDerivedTables(table);
                    if (derivedTables.Count == 0)
                    {
                        // If there are no derived tables, execute the else block if provided
                        return options.Inverse(context);
                    }
                    
                    // Execute the block for each derived table
                    var result = new StringBuilder();
                    foreach (var derivedTable in derivedTables)
                    {
                        result.Append(options.Fn(derivedTable));
                    }
                    
                    return result.ToString();
                }
                
                // If the context is not a table, execute the else block if provided
                return options?.Inverse(context) ?? string.Empty;
            });
            
            // Register helper to check if a table has any derived tables
            templateEngine.RegisterHelper("hasDerivedTables", (context) => {
                if (context is ITable table)
                {
                    return GetDerivedTables(table).Count > 0;
                }
                
                return false;
            });
            
            // Register helper to check if an inheritance relationship is TPH (Table Per Hierarchy)
            templateEngine.RegisterHelper("isTPH", (context) => {
                if (context is IRelationship relationship)
                {
                    return relationship.InheritanceType == InheritanceType.TablePerHierarchy;
                }
                
                return false;
            });
            
            // Register helper to check if an inheritance relationship is TPT (Table Per Type)
            templateEngine.RegisterHelper("isTPT", (context) => {
                if (context is IRelationship relationship)
                {
                    return relationship.InheritanceType == InheritanceType.TablePerType;
                }
                
                return false;
            });
            
            // Register helper to get the discriminator column for TPH
            templateEngine.RegisterHelper("getDiscriminator", (context) => {
                if (context is IRelationship relationship && 
                    relationship.RelationshipType == RelationshipType.Inheritance && 
                    relationship.InheritanceType == InheritanceType.TablePerHierarchy)
                {
                    return relationship.DiscriminatorColumn;
                }
                
                return null;
            });
        }

        private void RegisterNavigationPropertyHelpers(ITemplateEngine templateEngine)
        {
            // Register helper to generate navigation property name
            templateEngine.RegisterHelper("navigationPropertyName", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var foreignKeyName = arguments[0]?.ToString() ?? string.Empty;
                var isCollection = arguments.Length >= 2 && Convert.ToBoolean(arguments[1] ?? false);
                
                return GenerateNavigationPropertyName(foreignKeyName, isCollection);
            });
            
            // Register helper to generate collection navigation property
            templateEngine.RegisterHelper("collectionProperty", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var name = arguments[1]?.ToString() ?? string.Empty;
                
                var accessModifier = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? "public") : "public";
                var virtualModifier = arguments.Length < 4 || Convert.ToBoolean(arguments[3] ?? true);
                
                return GenerateCollectionProperty(type, name, accessModifier, virtualModifier);
            });
            
            // Register helper to generate reference navigation property
            templateEngine.RegisterHelper("referenceProperty", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var name = arguments[1]?.ToString() ?? string.Empty;
                
                var accessModifier = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? "public") : "public";
                var virtualModifier = arguments.Length < 4 || Convert.ToBoolean(arguments[3] ?? true);
                
                return GenerateReferenceProperty(type, name, accessModifier, virtualModifier);
            });
        }

        private void RegisterRelationshipNamingHelpers(ITemplateEngine templateEngine)
        {
            // Register helper to pluralize a name
            templateEngine.RegisterHelper("pluralize", (context) => {
                if (context == null) return string.Empty;
                return Pluralize(context.ToString());
            });
            
            // Register helper to singularize a name
            templateEngine.RegisterHelper("singularize", (context) => {
                if (context == null) return string.Empty;
                return Singularize(context.ToString());
            });
            
            // Register helper to remove prefix from a name
            templateEngine.RegisterHelper("removePrefix", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return context?.ToString() ?? string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var prefix = arguments[1]?.ToString() ?? string.Empty;
                
                return RemovePrefix(name, prefix);
            });
            
            // Register helper to remove suffix from a name
            templateEngine.RegisterHelper("removeSuffix", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return context?.ToString() ?? string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var suffix = arguments[1]?.ToString() ?? string.Empty;
                
                return RemoveSuffix(name, suffix);
            });
            
            // Register helper to sanitize a name for use as an identifier
            templateEngine.RegisterHelper("sanitizeName", (context) => {
                if (context == null) return string.Empty;
                return SanitizeName(context.ToString());
            });
        }

        #region Relationship Helpers

        private List<IRelationship> GetOneToManyRelationships(ITable table)
        {
            // This is a mock implementation - in a real system, you'd get the actual relationships from the schema
            return new List<IRelationship>();
        }

        private List<IRelationship> GetOneToOneRelationships(ITable table)
        {
            // This is a mock implementation - in a real system, you'd get the actual relationships from the schema
            return new List<IRelationship>();
        }

        private List<IRelationship> GetManyToManyRelationships(ITable table)
        {
            // This is a mock implementation - in a real system, you'd get the actual relationships from the schema
            return new List<IRelationship>();
        }

        private IRelationship GetInheritanceRelationship(ITable table)
        {
            // This is a mock implementation - in a real system, you'd get the actual relationship from the schema
            return null;
        }

        private List<ITable> GetDerivedTables(ITable table)
        {
            // This is a mock implementation - in a real system, you'd get the actual derived tables from the schema
            return new List<ITable>();
        }

        #endregion

        #region Naming Helpers

        private string GenerateNavigationPropertyName(string foreignKeyName, bool isCollection)
        {
            // Extract a suitable name from the foreign key name
            // Typical patterns are:
            // - CustomerId -> Customer
            // - CustomerAddressId -> CustomerAddress
            
            var name = foreignKeyName;
            
            // Remove common suffixes
            name = RemoveSuffix(name, "Id");
            name = RemoveSuffix(name, "ID");
            name = RemoveSuffix(name, "_id");
            name = RemoveSuffix(name, "Key");
            name = RemoveSuffix(name, "FK");
            name = RemoveSuffix(name, "ForeignKey");
            
            if (isCollection)
            {
                // Pluralize for collections
                name = Pluralize(name);
            }
            
            return name;
        }

        private string GenerateCollectionProperty(string type, string name, string accessModifier, bool virtualModifier)
        {
            var sb = new StringBuilder();
            var virtualKeyword = virtualModifier ? "virtual " : "";
            
            // Format the property as an ICollection<T>
            sb.AppendLine($"{accessModifier} {virtualKeyword}ICollection<{type}> {name} {{ get; set; }} = new List<{type}>();");
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateReferenceProperty(string type, string name, string accessModifier, bool virtualModifier)
        {
            var sb = new StringBuilder();
            var virtualKeyword = virtualModifier ? "virtual " : "";
            
            // Format the property as a reference navigation property
            sb.AppendLine($"{accessModifier} {virtualKeyword}{type} {name} {{ get; set; }}");
            
            return sb.ToString().TrimEnd();
        }

        private string Pluralize(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            // This is a simplified implementation - in a real system, you'd use a proper pluralization library
            
            // Handle some common irregular plurals
            switch (name.ToLowerInvariant())
            {
                case "person": return "People";
                case "child": return "Children";
                case "goose": return "Geese";
                case "man": return "Men";
                case "woman": return "Women";
                case "tooth": return "Teeth";
                case "foot": return "Feet";
                case "mouse": return "Mice";
                case "die": return "Dice";
            }
            
            // Apply common suffix rules
            if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase) && 
                name.Length > 1 && 
                !IsVowel(name[name.Length - 2]))
            {
                return name.Substring(0, name.Length - 1) + "ies";
            }
            
            if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase) || 
                name.EndsWith("x", StringComparison.OrdinalIgnoreCase) || 
                name.EndsWith("z", StringComparison.OrdinalIgnoreCase) || 
                name.EndsWith("ch", StringComparison.OrdinalIgnoreCase) || 
                name.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            {
                return name + "es";
            }
            
            // Default: just add 's'
            return name + "s";
        }

        private string Singularize(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            // This is a simplified implementation - in a real system, you'd use a proper singularization library
            
            // Handle some common irregular plurals
            switch (name.ToLowerInvariant())
            {
                case "people": return "Person";
                case "children": return "Child";
                case "geese": return "Goose";
                case "men": return "Man";
                case "women": return "Woman";
                case "teeth": return "Tooth";
                case "feet": return "Foot";
                case "mice": return "Mouse";
                case "dice": return "Die";
            }
            
            // Apply common suffix rules
            if (name.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
            {
                return name.Substring(0, name.Length - 3) + "y";
            }
            
            if (name.EndsWith("es", StringComparison.OrdinalIgnoreCase) && name.Length > 2)
            {
                var root = name.Substring(0, name.Length - 2);
                if (root.EndsWith("s", StringComparison.OrdinalIgnoreCase) || 
                    root.EndsWith("x", StringComparison.OrdinalIgnoreCase) || 
                    root.EndsWith("z", StringComparison.OrdinalIgnoreCase) || 
                    root.EndsWith("ch", StringComparison.OrdinalIgnoreCase) || 
                    root.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
                {
                    return root;
                }
            }
            
            // Default: just remove 's' if it exists
            if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase) && name.Length > 1)
            {
                return name.Substring(0, name.Length - 1);
            }
            
            return name;
        }

        private string RemovePrefix(string name, string prefix)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(prefix)) return name;
            
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return name.Substring(prefix.Length);
            }
            
            return name;
        }

        private string RemoveSuffix(string name, string suffix)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(suffix)) return name;
            
            if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return name.Substring(0, name.Length - suffix.Length);
            }
            
            return name;
        }

        private string SanitizeName(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            // Remove invalid characters
            var sanitized = Regex.Replace(name, @"[^\w]", "_");
            
            // Ensure the name starts with a letter or underscore
            if (!char.IsLetter(sanitized[0]) && sanitized[0] != '_')
            {
                sanitized = "_" + sanitized;
            }
            
            return sanitized;
        }

        private bool IsVowel(char c)
        {
            char lc = char.ToLowerInvariant(c);
            return lc == 'a' || lc == 'e' || lc == 'i' || lc == 'o' || lc == 'u';
        }

        #endregion
    }
}
