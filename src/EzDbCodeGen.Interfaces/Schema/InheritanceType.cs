namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines the types of inheritance patterns that can be implemented in a database schema.
    /// </summary>
    public enum InheritanceType
    {
        /// <summary>
        /// Table-Per-Hierarchy pattern where a single table contains all entity types with a discriminator column.
        /// </summary>
        TablePerHierarchy,

        /// <summary>
        /// Table-Per-Type pattern where each entity type has its own table with a foreign key to the parent table.
        /// </summary>
        TablePerType
    }
}
