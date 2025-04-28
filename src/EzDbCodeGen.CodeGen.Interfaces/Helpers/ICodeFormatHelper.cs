namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines an interface for code formatting helpers that support multiple programming languages.
/// </summary>
public interface ICodeFormatHelper : ITemplateHelper
{
    /// <summary>
    /// Formats C# code with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="code">The C# code to format.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted C# code.</returns>
    string FormatCSharp(string code, int indentSize = 4);
    
    /// <summary>
    /// Formats SQL code with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="sql">The SQL code to format.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted SQL code.</returns>
    string FormatSql(string sql, int indentSize = 2);
    
    /// <summary>
    /// Formats TypeScript code with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="code">The TypeScript code to format.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted TypeScript code.</returns>
    string FormatTypeScript(string code, int indentSize = 2);
    
    /// <summary>
    /// Formats JavaScript code with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="code">The JavaScript code to format.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted JavaScript code.</returns>
    string FormatJavaScript(string code, int indentSize = 2);
    
    /// <summary>
    /// Formats HTML code with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="html">The HTML code to format.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted HTML code.</returns>
    string FormatHtml(string html, int indentSize = 2);
    
    /// <summary>
    /// Formats CSS code with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="css">The CSS code to format.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted CSS code.</returns>
    string FormatCss(string css, int indentSize = 2);
    
    /// <summary>
    /// Formats Java code with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="code">The Java code to format.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted Java code.</returns>
    string FormatJava(string code, int indentSize = 4);
    
    /// <summary>
    /// Formats Python code with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="code">The Python code to format.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted Python code.</returns>
    string FormatPython(string code, int indentSize = 4);
    
    /// <summary>
    /// Formats code in the specified language with proper indentation and syntax highlighting.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <param name="language">The programming language.</param>
    /// <param name="indentSize">The number of spaces per indentation level.</param>
    /// <returns>The formatted code.</returns>
    string Format(string code, string language, int indentSize = 4);
}
