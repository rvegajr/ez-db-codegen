using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.CodeGen.Interfaces;

namespace EzDbCodeGen.TemplateEngine.Filters;

/// <summary>
/// A composite filter that combines multiple template filters.
/// </summary>
public class CompositeTemplateFilter : ITemplateFilter
{
    private readonly List<OutputTemplateFilter> _outputFilters = new();
    private readonly List<ITemplateFilter> _templateFilters = new();
    
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    public string Name => "CompositeFilter";
    
    /// <summary>
    /// Gets a description of the filter.
    /// </summary>
    public string Description => "Combines multiple filters";
    
    /// <summary>
    /// Gets the order in which this filter should be applied relative to other filters.
    /// </summary>
    public int Order => 0;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeTemplateFilter"/> class.
    /// </summary>
    public CompositeTemplateFilter()
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeTemplateFilter"/> class with an initial set of filters.
    /// </summary>
    /// <param name="filters">The filters to include.</param>
    public CompositeTemplateFilter(IEnumerable<ITemplateFilter> filters)
    {
        if (filters == null)
        {
            throw new ArgumentNullException(nameof(filters));
        }
        
        foreach (var filter in filters)
        {
            AddFilter(filter);
        }
    }
    
    /// <summary>
    /// Adds a filter to the composite.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    public void AddFilter(ITemplateFilter filter)
    {
        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter));
        }
        
        if (filter is OutputTemplateFilter outputFilter)
        {
            _outputFilters.Add(outputFilter);
            // Sort by order to ensure filters are applied in the correct order
            _outputFilters.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
        else
        {
            _templateFilters.Add(filter);
            // Sort by order to ensure filters are applied in the correct order
            _templateFilters.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
    }
    
    /// <summary>
    /// Adds an output filter to the composite.
    /// </summary>
    /// <param name="filter">The output filter to add.</param>
    public void AddOutputFilter(OutputTemplateFilter filter)
    {
        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter));
        }
        
        _outputFilters.Add(filter);
        // Sort by order to ensure filters are applied in the correct order
        _outputFilters.Sort((a, b) => a.Order.CompareTo(b.Order));
    }
    
    /// <summary>
    /// Applies all output filters to the generated content.
    /// </summary>
    /// <param name="content">The content to filter.</param>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>The filtered content.</returns>
    public string ApplyOutputFilters(string content, string templateName)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }
        
        // Apply each filter in order if it should be applied to this template
        foreach (var filter in _outputFilters)
        {
            if (filter.ShouldApplyTo(templateName))
            {
                content = filter.ApplyTransform(content);
            }
        }
        
        return content;
    }

    /// <inheritdoc/>
    public bool ShouldProcessTemplate(string templateName, object? model)
    {
        // If any filter says no, then don't process
        foreach (var filter in _templateFilters)
        {
            if (!filter.ShouldProcessTemplate(templateName, model))
            {
                return false;
            }
        }
        
        return true;
    }

    /// <inheritdoc/>
    public bool ShouldProcessModel(object? model, string templateName)
    {
        // If any filter says no, then don't process
        foreach (var filter in _templateFilters)
        {
            if (!filter.ShouldProcessModel(model, templateName))
            {
                return false;
            }
        }
        
        return true;
    }

    /// <inheritdoc/>
    public bool ShouldIncludeProperty(string propertyName, object? model, string templateName)
    {
        // If any filter says no, then don't include
        foreach (var filter in _templateFilters)
        {
            if (!filter.ShouldIncludeProperty(propertyName, model, templateName))
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Gets all the template filters in this composite.
    /// </summary>
    /// <returns>The template filters.</returns>
    public IReadOnlyList<ITemplateFilter> GetTemplateFilters() => _templateFilters.AsReadOnly();
    
    /// <summary>
    /// Gets all the output filters in this composite.
    /// </summary>
    /// <returns>The output filters.</returns>
    public IReadOnlyList<OutputTemplateFilter> GetOutputFilters() => _outputFilters.AsReadOnly();
    
    /// <summary>
    /// Gets a value indicating whether this composite has any filters.
    /// </summary>
    public bool HasFilters => _templateFilters.Count > 0 || _outputFilters.Count > 0;
    
    /// <summary>
    /// Clears all filters from this composite.
    /// </summary>
    public void ClearFilters()
    {
        _templateFilters.Clear();
        _outputFilters.Clear();
    }
}
