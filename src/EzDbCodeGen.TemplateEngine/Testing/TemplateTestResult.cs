using System;
using System.Collections.Generic;

namespace EzDbCodeGen.TemplateEngine.Testing;

/// <summary>
/// Represents the result of a template test.
/// </summary>
public class TemplateTestResult
{
    /// <summary>
    /// Gets or sets the path to the template that was tested.
    /// </summary>
    public string TemplatePath { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the name of the database that was used for testing.
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a value indicating whether the test was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the time when the test was run.
    /// </summary>
    public DateTime TestTime { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the output of the template.
    /// </summary>
    public string Output { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the errors that occurred during the test.
    /// </summary>
    public List<string> Errors { get; } = new List<string>();
    
    /// <summary>
    /// Gets the warnings that occurred during the test.
    /// </summary>
    public List<string> Warnings { get; } = new List<string>();
    
    /// <summary>
    /// Gets or sets the table that was used for testing, if any.
    /// </summary>
    public string? TableTested { get; set; }
    
    /// <summary>
    /// Gets or sets the view that was used for testing, if any.
    /// </summary>
    public string? ViewTested { get; set; }
    
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"Test {(Success ? "PASSED" : "FAILED")} - Template: {TemplatePath}, DB: {DatabaseName}, " +
               $"Errors: {Errors.Count}, Warnings: {Warnings.Count}";
    }
    
    /// <summary>
    /// Gets a detailed report of the test result.
    /// </summary>
    /// <returns>A detailed report string.</returns>
    public string GetDetailedReport()
    {
        var report = new System.Text.StringBuilder();
        
        report.AppendLine($"Template Test Result: {(Success ? "PASSED" : "FAILED")}");
        report.AppendLine($"Template: {TemplatePath}");
        report.AppendLine($"Database: {DatabaseName}");
        
        if (!string.IsNullOrEmpty(TableTested))
        {
            report.AppendLine($"Table Tested: {TableTested}");
        }
        
        if (!string.IsNullOrEmpty(ViewTested))
        {
            report.AppendLine($"View Tested: {ViewTested}");
        }
        
        report.AppendLine($"Test Time: {TestTime:yyyy-MM-dd HH:mm:ss}");
        
        if (Errors.Count > 0)
        {
            report.AppendLine("\nErrors:");
            foreach (var error in Errors)
            {
                report.AppendLine($"- {error}");
            }
        }
        
        if (Warnings.Count > 0)
        {
            report.AppendLine("\nWarnings:");
            foreach (var warning in Warnings)
            {
                report.AppendLine($"- {warning}");
            }
        }
        
        if (!string.IsNullOrEmpty(Output))
        {
            report.AppendLine("\nOutput Preview (first 500 chars):");
            report.AppendLine(Output.Length > 500 ? Output.Substring(0, 500) + "..." : Output);
        }
        
        return report.ToString();
    }
}
