using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Core.Schema.Models;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.RelationshipDetection
{
    /// <summary>
    /// A strategy for detecting many-to-many relationships through junction tables.
    /// This strategy is significantly more advanced than EF Core's default many-to-many detection.
    /// </summary>
    public class ManyToManyRelationshipStrategy : IRelationshipDetectionStrategy
    {
        /// <inheritdoc/>
        public string Name => "Many-to-Many Strategy";

        /// <inheritdoc/>
        public string Description => "Detects many-to-many relationships through junction tables with advanced heuristics.";

        /// <inheritdoc/>
        public int Priority => 20; // Run after foreign key strategy

        /// <inheritdoc/>
        public IEnumerable<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!options.DetectManyToManyRelationships)
            {
                yield break;
            }

            // Find potential junction tables
            var junctionTables = FindPotentialJunctionTables(schema, options);

            foreach (var junctionTable in junctionTables)
            {
                // Get the foreign keys from the junction table
                var foreignKeys = junctionTable.ForeignKeys.ToList();
                
                // A junction table should have at least 2 foreign keys
                if (foreignKeys.Count < 2)
                {
                    continue;
                }

                // Try to find pairs of foreign keys that form a many-to-many relationship
                for (int i = 0; i < foreignKeys.Count - 1; i++)
                {
                    for (int j = i + 1; j < foreignKeys.Count; j++)
                    {
                        var fk1 = foreignKeys[i];
                        var fk2 = foreignKeys[j];

                        // Create a many-to-many relationship
                        var relationship = CreateManyToManyRelationship(junctionTable, fk1, fk2, options);
                        
                        if (relationship != null)
                        {
                            yield return relationship;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds potential junction tables in the database schema.
        /// We use advanced heuristics to identify tables that might serve as junction tables.
        /// </summary>
        /// <param name="schema">The database schema.</param>
        /// <param name="options">The relationship detection options.</param>
        /// <returns>A collection of potential junction tables.</returns>
        private IEnumerable<ITable> FindPotentialJunctionTables(IDatabaseSchema schema, RelationshipDetectionOptions options)
        {
            foreach (var table in schema.Tables)
            {
                // Skip tables that don't have a primary key
                if (!table.HasPrimaryKey)
                {
                    continue;
                }

                // Skip tables explicitly excluded from being junction tables
                if (options.ExcludeJunctionTables.Contains(table.FullName))
                {
                    continue;
                }

                // A table with more than options.MaxColumnsInJunctionTable columns is probably not a junction table
                // unless the additional columns are considered "payload" columns
                if (table.Columns.Count > options.MaxColumnsInJunctionTable && !options.DetectPayloadColumns)
                {
                    continue;
                }

                // Skip tables with only one foreign key
                if (table.ForeignKeys.Count < 2)
                {
                    continue;
                }

                // Potential junction table if it has at least 2 foreign keys
                // and the primary key consists of the foreign key columns
                var foreignKeyColumns = table.ForeignKeys
                    .SelectMany(fk => fk.SourceColumns)
                    .Distinct()
                    .ToList();

                // Primary key columns should be a subset of the foreign key columns for a typical junction table
                bool isPrimaryKeyFromForeignKeys = table.PrimaryKeyColumns
                    .All(pk => foreignKeyColumns.Contains(pk));

                if (isPrimaryKeyFromForeignKeys)
                {
                    yield return table;
                    continue;
                }

                // Even if the primary key isn't made up of foreign keys, it might still be a junction table
                // if it meets certain naming patterns or structural criteria
                if (options.UseAdvancedJunctionTableHeuristics)
                {
                    // Check for naming patterns typical of junction tables
                    if (IsJunctionTableByNamingPattern(table))
                    {
                        yield return table;
                        continue;
                    }
                    
                    // Check for structural patterns typical of junction tables
                    if (IsJunctionTableByStructure(table, foreignKeyColumns))
                    {
                        yield return table;
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a table is likely a junction table based on its name.
        /// </summary>
        /// <param name="table">The table to check.</param>
        /// <returns>True if the table name matches common junction table patterns; otherwise, false.</returns>
        private bool IsJunctionTableByNamingPattern(ITable table)
        {
            // Common naming patterns for junction tables
            string tableName = table.Name.ToLowerInvariant();
            
            // Check for "has" pattern (e.g., "StudentHasCourse")
            if (tableName.Contains("has") || tableName.Contains("_has_"))
            {
                return true;
            }
            
            // Check for "to" pattern (e.g., "StudentToCourse" or "Student_To_Course")
            if (tableName.Contains("to") || tableName.Contains("_to_"))
            {
                return true;
            }
            
            // Check for concatenated table names pattern (e.g., "StudentCourse" or "Student_Course")
            // This is more complex and would require checking if the table name is a combination
            // of other table names in the schema
            
            // Check for common junction table prefixes or suffixes
            string[] junctionPrefixes = { "jnc_", "junction_", "j_", "jt_", "link_", "map_", "mapping_" };
            foreach (var prefix in junctionPrefixes)
            {
                if (tableName.StartsWith(prefix))
                {
                    return true;
                }
            }
            
            string[] junctionSuffixes = { "_junction", "_link", "_map", "_mapping", "_jnc" };
            foreach (var suffix in junctionSuffixes)
            {
                if (tableName.EndsWith(suffix))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Determines if a table is likely a junction table based on its structure.
        /// </summary>
        /// <param name="table">The table to check.</param>
        /// <param name="foreignKeyColumns">The foreign key columns in the table.</param>
        /// <returns>True if the table structure matches common junction table patterns; otherwise, false.</returns>
        private bool IsJunctionTableByStructure(ITable table, List<IColumn> foreignKeyColumns)
        {
            // A typical junction table has few columns beyond the foreign keys
            int nonForeignKeyColumns = table.Columns.Count - foreignKeyColumns.Count;
            
            // If most columns are foreign keys, it's likely a junction table
            if (foreignKeyColumns.Count >= table.Columns.Count * 0.7)
            {
                return true;
            }
            
            // Check for common payload columns that don't disqualify a table from being a junction
            var nonFkColumnNames = table.Columns
                .Where(c => !foreignKeyColumns.Contains(c))
                .Select(c => c.Name.ToLowerInvariant())
                .ToList();
            
            string[] commonPayloadNames = {
                "id", "created", "createdby", "createdon", "modified", "modifiedby", "modifiedon",
                "timestamp", "rowversion", "version", "sequence", "order", "priority", "active",
                "isactive", "enabled", "isenabled", "deleted", "isdeleted", "status", "notes"
            };
            
            // Count how many non-FK columns are common payload columns
            int payloadColumns = nonFkColumnNames.Count(col => commonPayloadNames.Any(p => col.Contains(p)));
            
            // If most non-FK columns are common payload columns, it's likely a junction table
            if (payloadColumns >= nonForeignKeyColumns * 0.7)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Creates a many-to-many relationship from a junction table and two foreign keys.
        /// </summary>
        /// <param name="junctionTable">The junction table.</param>
        /// <param name="foreignKey1">The first foreign key.</param>
        /// <param name="foreignKey2">The second foreign key.</param>
        /// <param name="options">The relationship detection options.</param>
        /// <returns>A many-to-many relationship if one can be created; otherwise, null.</returns>
        private IRelationship CreateManyToManyRelationship(
            ITable junctionTable,
            IForeignKey foreignKey1,
            IForeignKey foreignKey2,
            RelationshipDetectionOptions options)
        {
            // Ensure the foreign keys reference different tables
            // Unless we're allowing self-referencing many-to-many relationships
            if (foreignKey1.ReferencedTable == foreignKey2.ReferencedTable && 
                !options.DetectSelfReferencingRelationships)
            {
                return null;
            }

            // Create the relationship name
            string relationshipName = $"ManyToMany_{foreignKey1.ReferencedTable.Name}_{foreignKey2.ReferencedTable.Name}";
            
            // Handle self-referencing many-to-many
            if (foreignKey1.ReferencedTable == foreignKey2.ReferencedTable)
            {
                relationshipName = $"ManyToMany_Self_{foreignKey1.ReferencedTable.Name}";
            }

            // Create the relationship
            var relationship = new Relationship(
                relationshipName,
                RelationshipType.ManyToMany,
                foreignKey1.ReferencedTable,
                foreignKey2.ReferencedTable
            );

            // Set the junction table
            relationship.JunctionTable = junctionTable;

            // Add the foreign keys to the relationship
            relationship.AddForeignKey(foreignKey1);
            relationship.AddForeignKey(foreignKey2);

            // Add GetPayloadColumns function
            relationship.GetPayloadColumns = () => GetPayloadColumns(junctionTable, foreignKey1, foreignKey2);

            return relationship;
        }

        /// <summary>
        /// Gets the payload columns from a junction table (columns that are not part of any foreign key).
        /// </summary>
        /// <param name="junctionTable">The junction table.</param>
        /// <param name="foreignKey1">The first foreign key.</param>
        /// <param name="foreignKey2">The second foreign key.</param>
        /// <returns>A collection of payload columns.</returns>
        private IEnumerable<IColumn> GetPayloadColumns(
            ITable junctionTable, 
            IForeignKey foreignKey1, 
            IForeignKey foreignKey2)
        {
            // Get all columns that are part of the foreign keys
            var fkColumns = new HashSet<IColumn>(
                foreignKey1.SourceColumns.Concat(foreignKey2.SourceColumns)
            );

            // Return all columns that are not part of the foreign keys
            return junctionTable.Columns.Where(c => !fkColumns.Contains(c));
        }
    }
}
