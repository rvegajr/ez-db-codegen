namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents the action taken by a foreign key constraint when the referenced row is deleted or updated.
/// </summary>
public enum ReferentialAction
{
    /// <summary>
    /// No action is taken.
    /// </summary>
    NoAction,
    
    /// <summary>
    /// The operation is restricted.
    /// </summary>
    Restrict,
    
    /// <summary>
    /// The affected rows are cascaded.
    /// </summary>
    Cascade,
    
    /// <summary>
    /// The affected columns are set to null.
    /// </summary>
    SetNull,
    
    /// <summary>
    /// The affected columns are set to their default values.
    /// </summary>
    SetDefault
}
