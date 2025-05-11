using System;
using System.Collections.Generic;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Represents a database relationship between two tables
    /// </summary>
    public class Relationship
    {
        /// <summary>
        /// The name of the relationship (typically the constraint name)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The schema of the primary key table
        /// </summary>
        public string PrimaryKeyTableSchema { get; set; }
        
        /// <summary>
        /// The name of the primary key table
        /// </summary>
        public string PrimaryKeyTableName { get; set; }
        
        /// <summary>
        /// The column names that make up the primary key
        /// </summary>
        public List<string> PrimaryKeyColumnNames { get; set; } = new List<string>();
        
        /// <summary>
        /// The schema of the foreign key table
        /// </summary>
        public string ForeignKeyTableSchema { get; set; }
        
        /// <summary>
        /// The name of the foreign key table
        /// </summary>
        public string ForeignKeyTableName { get; set; }
        
        /// <summary>
        /// The column names that make up the foreign key
        /// </summary>
        public List<string> ForeignKeyColumnNames { get; set; } = new List<string>();
        
        /// <summary>
        /// Indicates whether this is a self-referencing relationship
        /// </summary>
        public bool IsSelfReferencing { get; set; }
        
        /// <summary>
        /// Gets the full name of the primary key table including schema
        /// </summary>
        public string FullPrimaryKeyTableName => $"{PrimaryKeyTableSchema}.{PrimaryKeyTableName}";
        
        /// <summary>
        /// Gets the full name of the foreign key table including schema
        /// </summary>
        public string FullForeignKeyTableName => $"{ForeignKeyTableSchema}.{ForeignKeyTableName}";
    }
}
