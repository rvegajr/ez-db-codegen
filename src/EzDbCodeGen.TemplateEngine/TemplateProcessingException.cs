using System;

namespace EzDbCodeGen.TemplateEngine;

/// <summary>
/// Exception thrown when an error occurs during template processing.
/// </summary>
public class TemplateProcessingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateProcessingException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public TemplateProcessingException(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateProcessingException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TemplateProcessingException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
