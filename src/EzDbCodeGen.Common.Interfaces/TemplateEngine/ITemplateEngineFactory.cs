using System;
using System.Collections.Generic;

namespace EzDbCodeGen.Common.Interfaces.TemplateEngine;

/// <summary>
/// Interface for a factory that creates template engines.
/// </summary>
public interface ITemplateEngineFactory
{
    /// <summary>
    /// Creates a template engine of the specified type with default options.
    /// </summary>
    /// <param name="engineType">The type of template engine to create.</param>
    /// <returns>The created template engine.</returns>
    ITemplateEngine CreateEngine(TemplateEngineType engineType);
    
    /// <summary>
    /// Creates a template engine of the specified type with the specified options.
    /// </summary>
    /// <param name="engineType">The type of template engine to create.</param>
    /// <param name="options">The options to use when creating the template engine.</param>
    /// <returns>The created template engine.</returns>
    ITemplateEngine CreateEngine(TemplateEngineType engineType, TemplateEngineOptions options);
    
    /// <summary>
    /// Registers a factory method for creating template engines of the specified type.
    /// </summary>
    /// <param name="engineType">The type of template engine to register.</param>
    /// <param name="factoryMethod">The factory method to use when creating template engines of the specified type.</param>
    void RegisterEngineFactory(TemplateEngineType engineType, Func<TemplateEngineOptions, ITemplateEngine> factoryMethod);
    
    /// <summary>
    /// Determines whether a factory method is registered for the specified template engine type.
    /// </summary>
    /// <param name="engineType">The type of template engine to check.</param>
    /// <returns>True if a factory method is registered for the specified type; otherwise, false.</returns>
    bool IsEngineRegistered(TemplateEngineType engineType);
    
    /// <summary>
    /// Gets a list of the available template engine types.
    /// </summary>
    /// <returns>A list of the available template engine types.</returns>
    IReadOnlyList<TemplateEngineType> GetAvailableEngines();
}
