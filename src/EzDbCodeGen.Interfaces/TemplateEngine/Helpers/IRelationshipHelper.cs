using System.Collections.Generic;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Interfaces.TemplateEngine.Helpers
{
    /// <summary>
    /// Defines helpers for working with relationships in templates.
    /// </summary>
    public interface IRelationshipHelper
    {
        /// <summary>
        /// Gets all relationships for a table.
        /// </summary>
        /// <param name="table">The table to get relationships for.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>A collection of relationships for the table.</returns>
        IEnumerable<IRelationship> GetRelationshipsForTable(ITable table, IDatabaseSchema schema);

        /// <summary>
        /// Gets all one-to-many relationships for a table.
        /// </summary>
        /// <param name="table">The table to get relationships for.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>A collection of one-to-many relationships for the table.</returns>
        IEnumerable<IRelationship> GetOneToManyRelationships(ITable table, IDatabaseSchema schema);

        /// <summary>
        /// Gets all one-to-one relationships for a table.
        /// </summary>
        /// <param name="table">The table to get relationships for.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>A collection of one-to-one relationships for the table.</returns>
        IEnumerable<IRelationship> GetOneToOneRelationships(ITable table, IDatabaseSchema schema);

        /// <summary>
        /// Gets all many-to-many relationships for a table.
        /// </summary>
        /// <param name="table">The table to get relationships for.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>A collection of many-to-many relationships for the table.</returns>
        IEnumerable<IRelationship> GetManyToManyRelationships(ITable table, IDatabaseSchema schema);

        /// <summary>
        /// Gets all inheritance relationships for a table.
        /// </summary>
        /// <param name="table">The table to get relationships for.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>A collection of inheritance relationships for the table.</returns>
        IEnumerable<IRelationship> GetInheritanceRelationships(ITable table, IDatabaseSchema schema);

        /// <summary>
        /// Gets the navigation property name for a relationship.
        /// </summary>
        /// <param name="relationship">The relationship to get the navigation property name for.</param>
        /// <param name="table">The table perspective to get the navigation property name from.</param>
        /// <returns>The navigation property name for the relationship.</returns>
        string GetNavigationPropertyName(IRelationship relationship, ITable table);

        /// <summary>
        /// Gets the opposite table in a relationship.
        /// </summary>
        /// <param name="relationship">The relationship to get the opposite table for.</param>
        /// <param name="table">The table to get the opposite of.</param>
        /// <returns>The opposite table in the relationship.</returns>
        ITable GetOppositeTable(IRelationship relationship, ITable table);

        /// <summary>
        /// Gets the C# Entity Framework Core relationship configuration.
        /// </summary>
        /// <param name="relationship">The relationship to get the configuration for.</param>
        /// <param name="table">The table perspective to get the configuration from.</param>
        /// <returns>The C# Entity Framework Core relationship configuration.</returns>
        string GetEfCoreConfiguration(IRelationship relationship, ITable table);

        /// <summary>
        /// Gets a value indicating whether a table is a child in an inheritance relationship.
        /// </summary>
        /// <param name="table">The table to check.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>True if the table is a child in an inheritance relationship, false otherwise.</returns>
        bool IsChildTable(ITable table, IDatabaseSchema schema);

        /// <summary>
        /// Gets a value indicating whether a table is a parent in an inheritance relationship.
        /// </summary>
        /// <param name="table">The table to check.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>True if the table is a parent in an inheritance relationship, false otherwise.</returns>
        bool IsParentTable(ITable table, IDatabaseSchema schema);

        /// <summary>
        /// Gets the parent table in an inheritance relationship.
        /// </summary>
        /// <param name="table">The child table to get the parent for.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>The parent table in the inheritance relationship, or null if the table is not a child.</returns>
        ITable GetParentTable(ITable table, IDatabaseSchema schema);

        /// <summary>
        /// Gets the child tables in an inheritance relationship.
        /// </summary>
        /// <param name="table">The parent table to get the children for.</param>
        /// <param name="schema">The database schema containing the relationships.</param>
        /// <returns>A collection of child tables in the inheritance relationship.</returns>
        IEnumerable<ITable> GetChildTables(ITable table, IDatabaseSchema schema);
    }
}
