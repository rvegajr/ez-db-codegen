using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.TemplateEngine.Extensions;

/// <summary>
/// Extension methods for IForeignKey.
/// </summary>
public static class ForeignKeyExtensions
{
    /// <summary>
    /// Determines if a foreign key represents a unique relationship.
    /// </summary>
    /// <param name="foreignKey">The foreign key to check.</param>
    /// <returns>True if the foreign key represents a unique relationship; otherwise, false.</returns>
    public static bool IsUnique(this IForeignKey foreignKey)
    {
        if (foreignKey == null)
        {
            return false;
        }
        
        // A foreign key is considered unique if all of its columns are part of a unique constraint or primary key
        return foreignKey.Columns.All(column => column.IsPartOfPrimaryKey || column.IsPartOfUniqueConstraint);
    }
}
