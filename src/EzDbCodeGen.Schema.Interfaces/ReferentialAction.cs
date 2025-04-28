namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents referential action behavior for foreign key constraints.
/// </summary>
public enum ReferentialAction
{
    /// <summary>
    /// No action is taken when the referenced entity is deleted or updated.
    /// </summary>
    NoAction,
    
    /// <summary>
    /// Related entities are deleted when the referenced entity is deleted.
    /// </summary>
    Cascade,
    
    /// <summary>
    /// The relationship is enforced for deletes and updates of referenced entities.
    /// </summary>
    Restrict,
    
    /// <summary>
    /// The foreign key values in dependent tables are set to null when the referenced entity is deleted or updated.
    /// </summary>
    SetNull,
    
    /// <summary>
    /// The foreign key values in dependent tables are set to their default values when the referenced entity is deleted or updated.
    /// </summary>
    SetDefault
}
