namespace EzDbCodeGen.Schema.Interfaces
{
    /// <summary>
    /// Represents a mapping between a column in a table and a referenced column in another table.
    /// </summary>
    public interface IForeignKeyColumnMapping
    {
        /// <summary>
        /// Gets the source column.
        /// </summary>
        IColumn SourceColumn { get; }

        /// <summary>
        /// Gets the referenced column.
        /// </summary>
        IColumn ReferencedColumn { get; }
    }
}
