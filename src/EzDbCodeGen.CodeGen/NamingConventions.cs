using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace EzDbCodeGen.CodeGen;

/// <summary>
/// Defines naming conventions for code generation.
/// </summary>
public class NamingConventions
{
    /// <summary>
    /// Gets or sets the naming convention for class names.
    /// </summary>
    public NamingConvention ClassNaming { get; set; } = NamingConvention.PascalCase;
    
    /// <summary>
    /// Gets or sets the naming convention for property names.
    /// </summary>
    public NamingConvention PropertyNaming { get; set; } = NamingConvention.PascalCase;
    
    /// <summary>
    /// Gets or sets the naming convention for method names.
    /// </summary>
    public NamingConvention MethodNaming { get; set; } = NamingConvention.PascalCase;
    
    /// <summary>
    /// Gets or sets the naming convention for parameter names.
    /// </summary>
    public NamingConvention ParameterNaming { get; set; } = NamingConvention.CamelCase;
    
    /// <summary>
    /// Gets or sets the naming convention for variable names.
    /// </summary>
    public NamingConvention VariableNaming { get; set; } = NamingConvention.CamelCase;
    
    /// <summary>
    /// Gets or sets the naming convention for interface names.
    /// </summary>
    public NamingConvention InterfaceNaming { get; set; } = NamingConvention.PascalCase;
    
    /// <summary>
    /// Gets or sets the prefix for interface names.
    /// </summary>
    public string InterfacePrefix { get; set; } = "I";
    
    /// <summary>
    /// Gets or sets the naming convention for enum names.
    /// </summary>
    public NamingConvention EnumNaming { get; set; } = NamingConvention.PascalCase;
    
    /// <summary>
    /// Gets or sets the naming convention for enum values.
    /// </summary>
    public NamingConvention EnumValueNaming { get; set; } = NamingConvention.PascalCase;
    
    /// <summary>
    /// Gets or sets the naming convention for constant names.
    /// </summary>
    public NamingConvention ConstantNaming { get; set; } = NamingConvention.PascalCase;
    
    /// <summary>
    /// Gets or sets the naming convention for file names.
    /// </summary>
    public NamingConvention FileNaming { get; set; } = NamingConvention.PascalCase;
    
    /// <summary>
    /// Gets or sets a value indicating whether to remove underscores from names.
    /// </summary>
    public bool RemoveUnderscores { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to remove spaces from names.
    /// </summary>
    public bool RemoveSpaces { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to preserve casing from original database names.
    /// </summary>
    public bool PreserveOriginalCasing { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to pluralize collection names.
    /// </summary>
    public bool PluralizeCollectionNames { get; set; } = true;
    
    /// <summary>
    /// Creates a new instance of the <see cref="NamingConventions"/> class with C# naming conventions.
    /// </summary>
    /// <returns>A new instance of the <see cref="NamingConventions"/> class with C# naming conventions.</returns>
    public static NamingConventions CSharp()
    {
        return new NamingConventions
        {
            ClassNaming = NamingConvention.PascalCase,
            PropertyNaming = NamingConvention.PascalCase,
            MethodNaming = NamingConvention.PascalCase,
            ParameterNaming = NamingConvention.CamelCase,
            VariableNaming = NamingConvention.CamelCase,
            InterfaceNaming = NamingConvention.PascalCase,
            InterfacePrefix = "I",
            EnumNaming = NamingConvention.PascalCase,
            EnumValueNaming = NamingConvention.PascalCase,
            ConstantNaming = NamingConvention.PascalCase,
            FileNaming = NamingConvention.PascalCase,
            RemoveUnderscores = true,
            RemoveSpaces = true,
            PreserveOriginalCasing = false,
            PluralizeCollectionNames = true
        };
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="NamingConventions"/> class with TypeScript naming conventions.
    /// </summary>
    /// <returns>A new instance of the <see cref="NamingConventions"/> class with TypeScript naming conventions.</returns>
    public static NamingConventions TypeScript()
    {
        return new NamingConventions
        {
            ClassNaming = NamingConvention.PascalCase,
            PropertyNaming = NamingConvention.CamelCase,
            MethodNaming = NamingConvention.CamelCase,
            ParameterNaming = NamingConvention.CamelCase,
            VariableNaming = NamingConvention.CamelCase,
            InterfaceNaming = NamingConvention.PascalCase,
            InterfacePrefix = "I",
            EnumNaming = NamingConvention.PascalCase,
            EnumValueNaming = NamingConvention.PascalCase,
            ConstantNaming = NamingConvention.CamelCase,
            FileNaming = NamingConvention.KebabCase,
            RemoveUnderscores = true,
            RemoveSpaces = true,
            PreserveOriginalCasing = false,
            PluralizeCollectionNames = true
        };
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="NamingConventions"/> class with Java naming conventions.
    /// </summary>
    /// <returns>A new instance of the <see cref="NamingConventions"/> class with Java naming conventions.</returns>
    public static NamingConventions Java()
    {
        return new NamingConventions
        {
            ClassNaming = NamingConvention.PascalCase,
            PropertyNaming = NamingConvention.CamelCase,
            MethodNaming = NamingConvention.CamelCase,
            ParameterNaming = NamingConvention.CamelCase,
            VariableNaming = NamingConvention.CamelCase,
            InterfaceNaming = NamingConvention.PascalCase,
            InterfacePrefix = "",
            EnumNaming = NamingConvention.PascalCase,
            EnumValueNaming = NamingConvention.ScreamingSnakeCase,
            ConstantNaming = NamingConvention.ScreamingSnakeCase,
            FileNaming = NamingConvention.PascalCase,
            RemoveUnderscores = true,
            RemoveSpaces = true,
            PreserveOriginalCasing = false,
            PluralizeCollectionNames = true
        };
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="NamingConventions"/> class with Python naming conventions.
    /// </summary>
    /// <returns>A new instance of the <see cref="NamingConventions"/> class with Python naming conventions.</returns>
    public static NamingConventions Python()
    {
        return new NamingConventions
        {
            ClassNaming = NamingConvention.PascalCase,
            PropertyNaming = NamingConvention.SnakeCase,
            MethodNaming = NamingConvention.SnakeCase,
            ParameterNaming = NamingConvention.SnakeCase,
            VariableNaming = NamingConvention.SnakeCase,
            InterfaceNaming = NamingConvention.PascalCase,
            InterfacePrefix = "",
            EnumNaming = NamingConvention.PascalCase,
            EnumValueNaming = NamingConvention.ScreamingSnakeCase,
            ConstantNaming = NamingConvention.ScreamingSnakeCase,
            FileNaming = NamingConvention.SnakeCase,
            RemoveUnderscores = false,
            RemoveSpaces = true,
            PreserveOriginalCasing = false,
            PluralizeCollectionNames = true
        };
    }
    
    /// <summary>
    /// Applies the naming conventions to a name.
    /// </summary>
    /// <param name="name">The name to format.</param>
    /// <param name="convention">The naming convention to apply.</param>
    /// <returns>The formatted name.</returns>
    public string ApplyNamingConvention(string name, NamingConvention convention)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }
        
        // Remove spaces and underscores if configured
        if (RemoveSpaces)
        {
            name = name.Replace(" ", "");
        }
        
        if (RemoveUnderscores)
        {
            name = name.Replace("_", "");
        }
        
        // Apply the naming convention
        switch (convention)
        {
            case NamingConvention.PascalCase:
                return ToPascalCase(name);
                
            case NamingConvention.CamelCase:
                return ToCamelCase(name);
                
            case NamingConvention.SnakeCase:
                return ToSnakeCase(name);
                
            case NamingConvention.KebabCase:
                return ToKebabCase(name);
                
            case NamingConvention.ScreamingSnakeCase:
                return ToScreamingSnakeCase(name);
                
            default:
                return name;
        }
    }
    
    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The PascalCase string.</returns>
    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        // Handle underscore or space separated inputs
        string[] words = Regex.Split(input, @"[\s_]+")
            .Where(word => !string.IsNullOrEmpty(word))
            .ToArray();
        
        // Capitalize the first letter of each word
        for (int i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
            {
                words[i] = char.ToUpper(words[i][0]) + (words[i].Length > 1 ? words[i].Substring(1) : "");
            }
        }
        
        return string.Join("", words);
    }
    
    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The camelCase string.</returns>
    private string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        string pascalCase = ToPascalCase(input);
        return char.ToLower(pascalCase[0]) + (pascalCase.Length > 1 ? pascalCase.Substring(1) : "");
    }
    
    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The snake_case string.</returns>
    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        // First handle underscore or space separated inputs
        string[] words = Regex.Split(input, @"[\s_]+")
            .Where(word => !string.IsNullOrEmpty(word))
            .ToArray();
        
        // If already in snake case format
        if (words.Length > 1)
        {
            return string.Join("_", words.Select(word => word.ToLower()));
        }
        
        // Handle camelCase and PascalCase
        var snakeCase = Regex.Replace(input, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
        
        return snakeCase;
    }
    
    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The kebab-case string.</returns>
    private string ToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        // First handle underscore or space separated inputs
        string[] words = Regex.Split(input, @"[\s_]+")
            .Where(word => !string.IsNullOrEmpty(word))
            .ToArray();
        
        // If already in kebab case format
        if (words.Length > 1)
        {
            return string.Join("-", words.Select(word => word.ToLower()));
        }
        
        // Handle camelCase and PascalCase
        var kebabCase = Regex.Replace(input, "([a-z0-9])([A-Z])", "$1-$2").ToLower();
        
        return kebabCase;
    }
    
    /// <summary>
    /// Converts a string to SCREAMING_SNAKE_CASE.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The SCREAMING_SNAKE_CASE string.</returns>
    private string ToScreamingSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        return ToSnakeCase(input).ToUpper();
    }
}

/// <summary>
/// Defines the naming convention for code elements.
/// </summary>
public enum NamingConvention
{
    /// <summary>
    /// PascalCase - first letter of each word is uppercase, no separators.
    /// </summary>
    PascalCase,
    
    /// <summary>
    /// camelCase - first letter is lowercase, first letter of each subsequent word is uppercase, no separators.
    /// </summary>
    CamelCase,
    
    /// <summary>
    /// snake_case - all lowercase, words separated by underscores.
    /// </summary>
    SnakeCase,
    
    /// <summary>
    /// kebab-case - all lowercase, words separated by hyphens.
    /// </summary>
    KebabCase,
    
    /// <summary>
    /// SCREAMING_SNAKE_CASE - all uppercase, words separated by underscores.
    /// </summary>
    ScreamingSnakeCase
}
