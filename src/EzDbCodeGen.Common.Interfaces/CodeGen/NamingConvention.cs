namespace EzDbCodeGen.Common.Interfaces.CodeGen;

/// <summary>
/// Represents naming conventions for code generation.
/// </summary>
public enum NamingConvention
{
    /// <summary>
    /// Keep the original naming as-is.
    /// </summary>
    KeepOriginal,
    
    /// <summary>
    /// Use PascalCase (e.g., CustomerName).
    /// </summary>
    PascalCase,
    
    /// <summary>
    /// Use camelCase (e.g., customerName).
    /// </summary>
    CamelCase,
    
    /// <summary>
    /// Use snake_case (e.g., customer_name).
    /// </summary>
    SnakeCase,
    
    /// <summary>
    /// Use kebab-case (e.g., customer-name).
    /// </summary>
    KebabCase,
    
    /// <summary>
    /// Use UPPER_SNAKE_CASE (e.g., CUSTOMER_NAME).
    /// </summary>
    UpperSnakeCase
}
