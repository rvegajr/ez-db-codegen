namespace EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;

/// <summary>
/// Represents a collection of code formatting helpers for templates.
/// Implementation of the CodeFormatEz helper mentioned in the architectural specifications.
/// </summary>
public interface ICodeFormatHelpers
{
    /// <summary>
    /// Formats C# code.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <returns>The formatted code.</returns>
    string FormatCSharp(string code);
    
    /// <summary>
    /// Formats SQL code.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <returns>The formatted code.</returns>
    string FormatSql(string code);
    
    /// <summary>
    /// Formats TypeScript code.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <returns>The formatted code.</returns>
    string FormatTypeScript(string code);
    
    /// <summary>
    /// Formats JavaScript code.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <returns>The formatted code.</returns>
    string FormatJavaScript(string code);
    
    /// <summary>
    /// Formats HTML code.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <returns>The formatted code.</returns>
    string FormatHtml(string code);
    
    /// <summary>
    /// Formats CSS code.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <returns>The formatted code.</returns>
    string FormatCss(string code);
    
    /// <summary>
    /// Formats Java code.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <returns>The formatted code.</returns>
    string FormatJava(string code);
    
    /// <summary>
    /// Formats Python code.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <returns>The formatted code.</returns>
    string FormatPython(string code);
    
    /// <summary>
    /// Escapes a string for use in C#.
    /// </summary>
    /// <param name="value">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    string EscapeCSharpString(string value);
    
    /// <summary>
    /// Escapes a string for use in SQL.
    /// </summary>
    /// <param name="value">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    string EscapeSqlString(string value);
    
    /// <summary>
    /// Escapes a string for use in JavaScript/TypeScript.
    /// </summary>
    /// <param name="value">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    string EscapeJsString(string value);
    
    /// <summary>
    /// Escapes a string for use in HTML.
    /// </summary>
    /// <param name="value">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    string EscapeHtmlString(string value);
    
    /// <summary>
    /// Formats code based on the specified language.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <param name="language">The language of the code.</param>
    /// <returns>The formatted code.</returns>
    string FormatCodeByLanguage(string code, string language);
}
