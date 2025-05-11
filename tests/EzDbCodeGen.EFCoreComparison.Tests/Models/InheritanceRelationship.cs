using System;
using System.Collections.Generic;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Represents an inheritance relationship between two tables
    /// </summary>
    public class InheritanceRelationship
    {
        /// <summary>
        /// The schema of the base table
        /// </summary>
        public string BaseTableSchema { get; set; }
        
        /// <summary>
        /// The name of the base table
        /// </summary>
        public string BaseTableName { get; set; }
        
        /// <summary>
        /// The schema of the derived table
        /// </summary>
        public string DerivedTableSchema { get; set; }
        
        /// <summary>
        /// The name of the derived table
        /// </summary>
        public string DerivedTableName { get; set; }
        
        /// <summary>
        /// The type of inheritance (TPH, TPT)
        /// TPH = Table Per Hierarchy
        /// TPT = Table Per Type
        /// </summary>
        public string InheritanceType { get; set; }
        
        /// <summary>
        /// The name of the discriminator column (for TPH)
        /// </summary>
        public string DiscriminatorColumn { get; set; }
        
        /// <summary>
        /// Gets the full name of the base table including schema
        /// </summary>
        public string FullBaseTableName => $"{BaseTableSchema}.{BaseTableName}";
        
        /// <summary>
        /// Gets the full name of the derived table including schema
        /// </summary>
        public string FullDerivedTableName => $"{DerivedTableSchema}.{DerivedTableName}";
    }
}
