namespace EzDbCodeGen.Core.TemplateEngine.Helpers;

/// <summary>
/// Represents a collection of layout helpers for templates.
/// Implementation of the LayoutEz helper mentioned in the architectural specifications.
/// </summary>
public interface ILayoutHelpers
{
    /// <summary>
    /// Defines a named region in a template.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <param name="content">The content of the region.</param>
    /// <returns>The region placeholder.</returns>
    string DefineRegion(string name, string content);
    
    /// <summary>
    /// References a named region in a template.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <returns>The referenced region content.</returns>
    string ReferenceRegion(string name);
    
    /// <summary>
    /// Renders a named region in a template.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <returns>The rendered region content.</returns>
    string RenderRegion(string name);
    
    /// <summary>
    /// Appends content to a named region.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <param name="content">The content to append.</param>
    /// <returns>The updated region placeholder.</returns>
    string AppendToRegion(string name, string content);
    
    /// <summary>
    /// Prepends content to a named region.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <param name="content">The content to prepend.</param>
    /// <returns>The updated region placeholder.</returns>
    string PrependToRegion(string name, string content);
    
    /// <summary>
    /// Clears the content of a named region.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <returns>The updated region placeholder.</returns>
    string ClearRegion(string name);
    
    /// <summary>
    /// Checks if a named region exists.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <returns>True if the region exists, false otherwise.</returns>
    bool RegionExists(string name);
    
    /// <summary>
    /// Sets the rendering order of regions.
    /// </summary>
    /// <param name="regionNames">The names of the regions in the order they should be rendered.</param>
    void SetRegionRenderOrder(params string[] regionNames);
    
    /// <summary>
    /// Gets the rendering order of regions.
    /// </summary>
    /// <returns>The names of the regions in the order they should be rendered.</returns>
    IReadOnlyList<string> GetRegionRenderOrder();
    
    /// <summary>
    /// Gets all defined regions.
    /// </summary>
    /// <returns>A dictionary of region names and content.</returns>
    IReadOnlyDictionary<string, string> GetAllRegions();
    
    /// <summary>
    /// Renders all regions in the defined order.
    /// </summary>
    /// <returns>The rendered content.</returns>
    string RenderAllRegions();
}
