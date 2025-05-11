using System;
using System.Collections.Generic;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Represents a many-to-many relationship between two tables through a join table
    /// </summary>
    public class ManyToManyRelationship
    {
        /// <summary>
        /// The name of the relationship
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The schema of the left table
        /// </summary>
        public string LeftTableSchema { get; set; }
        
        /// <summary>
        /// The name of the left table
        /// </summary>
        public string LeftTableName { get; set; }
        
        /// <summary>
        /// The column names in the left table that participate in the relationship
        /// </summary>
        public List<string> LeftColumnNames { get; set; } = new List<string>();
        
        /// <summary>
        /// The schema of the right table
        /// </summary>
        public string RightTableSchema { get; set; }
        
        /// <summary>
        /// The name of the right table
        /// </summary>
        public string RightTableName { get; set; }
        
        /// <summary>
        /// The column names in the right table that participate in the relationship
        /// </summary>
        public List<string> RightColumnNames { get; set; } = new List<string>();
        
        /// <summary>
        /// The schema of the join table
        /// </summary>
        public string JoinTableSchema { get; set; }
        
        /// <summary>
        /// The name of the join table
        /// </summary>
        public string JoinTableName { get; set; }
        
        /// <summary>
        /// The column names in the join table that reference the left table
        /// </summary>
        public List<string> LeftJoinColumnNames { get; set; } = new List<string>();
        
        /// <summary>
        /// The column names in the join table that reference the right table
        /// </summary>
        public List<string> RightJoinColumnNames { get; set; } = new List<string>();
        
        /// <summary>
        /// Indicates whether the join table has payload columns (additional data beyond the foreign keys)
        /// </summary>
        public bool HasPayloadColumns { get; set; }
        
        /// <summary>
        /// The names of payload columns in the join table
        /// </summary>
        public List<string> PayloadColumnNames { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets the full name of the left table including schema
        /// </summary>
        public string FullLeftTableName => $"{LeftTableSchema}.{LeftTableName}";
        
        /// <summary>
        /// Gets the full name of the right table including schema
        /// </summary>
        public string FullRightTableName => $"{RightTableSchema}.{RightTableName}";
        
        /// <summary>
        /// Gets the full name of the join table including schema
        /// </summary>
        public string FullJoinTableName => $"{JoinTableSchema}.{JoinTableName}";
    }
}
