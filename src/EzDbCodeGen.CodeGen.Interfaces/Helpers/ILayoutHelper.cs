namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines an interface for template layout helpers that support named regions, rendering order control, and region manipulation.
/// </summary>
public interface ILayoutHelper : ITemplateHelper
{
    /// <summary>
    /// Defines a named region in a template.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <param name="content">The content of the region.</param>
    /// <returns>A placeholder for the region.</returns>
    string DefineRegion(string name, string content);
    
    /// <summary>
    /// Renders a named region.
    /// </summary>
    /// <param name="name">The name of the region to render.</param>
    /// <returns>The content of the region.</returns>
    string RenderRegion(string name);
    
    /// <summary>
    /// Appends content to a named region.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <param name="content">The content to append.</param>
    /// <returns>A placeholder for the updated region.</returns>
    string AppendToRegion(string name, string content);
    
    /// <summary>
    /// Prepends content to a named region.
    /// </summary>
    /// <param name="name">The name of the region.</param>
    /// <param name="content">The content to prepend.</param>
    /// <returns>A placeholder for the updated region.</returns>
    string PrependToRegion(string name, string content);
    
    /// <summary>
    /// Sets the rendering order of regions.
    /// </summary>
    /// <param name="regionNames">The names of the regions in the order they should be rendered.</param>
    void SetRenderingOrder(params string[] regionNames);
    
    /// <summary>
    /// Checks if a region exists.
    /// </summary>
    /// <param name="name">The name of the region to check.</param>
    /// <returns>True if the region exists; otherwise, false.</returns>
    bool RegionExists(string name);
    
    /// <summary>
    /// Clears a region's content.
    /// </summary>
    /// <param name="name">The name of the region to clear.</param>
    void ClearRegion(string name);
    
    /// <summary>
    /// Gets all defined region names.
    /// </summary>
    /// <returns>A list of all defined region names.</returns>
    IReadOnlyList<string> GetRegionNames();
    
    /// <summary>
    /// Renders all regions in the specified order.
    /// </summary>
    /// <returns>The combined content of all regions.</returns>
    string RenderAllRegions();
}
