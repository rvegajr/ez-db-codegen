using System;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Extensions;

/// <summary>
/// Extension methods for the <see cref="ITable"/> interface.
/// </summary>
public static class TableExtensions
{
    /// <summary>
    /// Gets the full name of the table, including the schema.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>The full name of the table in the format: [Schema].[TableName]</returns>
    public static string GetFullName(this ITable table)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        return string.IsNullOrEmpty(table.Schema) 
            ? table.Name 
            : $"{table.Schema}.{table.Name}";
    }
    
    /// <summary>
    /// Gets the safe C# class name for the table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>A safe C# class name for the table.</returns>
    public static string GetSafeClassName(this ITable table)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }
        
        // Remove spaces and special characters
        string safeName = table.Name.Replace(" ", "");
        
        // Ensure the first character is uppercase
        if (!string.IsNullOrEmpty(safeName) && char.IsLower(safeName[0]))
        {
            safeName = char.ToUpper(safeName[0]) + safeName.Substring(1);
        }
        
        return safeName;
    }
    
    /// <summary>
    /// Gets a suggested property name for a navigation property referencing this table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="isManyRelationship">Whether the relationship is a collection (many) relationship.</param>
    /// <returns>A suggested property name.</returns>
    public static string GetNavigationPropertyName(this ITable table, bool isManyRelationship)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }
        
        string propertyName = table.GetSafeClassName();
        
        // For many relationships, pluralize the name
        if (isManyRelationship)
        {
            // Simple pluralization - add "s" or "es"
            if (propertyName.EndsWith("s") || propertyName.EndsWith("x") || 
                propertyName.EndsWith("z") || propertyName.EndsWith("ch") || 
                propertyName.EndsWith("sh"))
            {
                propertyName += "es";
            }
            else if (propertyName.EndsWith("y") && !IsVowel(propertyName[propertyName.Length - 2]))
            {
                propertyName = propertyName.Substring(0, propertyName.Length - 1) + "ies";
            }
            else
            {
                propertyName += "s";
            }
        }
        
        return propertyName;
    }
    
    /// <summary>
    /// Checks if a character is a vowel.
    /// </summary>
    /// <param name="c">The character to check.</param>
    /// <returns>True if the character is a vowel; otherwise, false.</returns>
    private static bool IsVowel(char c)
    {
        char lower = char.ToLower(c);
        return lower == 'a' || lower == 'e' || lower == 'i' || lower == 'o' || lower == 'u';
    }
}
