using System;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Represents a navigation property in an entity model
    /// </summary>
    public class NavigationProperty
    {
        /// <summary>
        /// The name of the navigation property
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The name of the source table (the table that contains the navigation property)
        /// </summary>
        public string SourceTableName { get; set; }
        
        /// <summary>
        /// The name of the target table (the table that the navigation property points to)
        /// </summary>
        public string TargetTableName { get; set; }
        
        /// <summary>
        /// Indicates whether this navigation property is a collection
        /// </summary>
        public bool IsCollection { get; set; }
        
        /// <summary>
        /// The semantic quality score of the navigation property name (0-100)
        /// Higher scores indicate more meaningful names
        /// </summary>
        public int NameQualityScore 
        { 
            get
            {
                // Calculate a quality score based on the name
                // Generic names like "RelatedEntity" or "EntityList" get lower scores
                if (string.IsNullOrEmpty(Name))
                    return 0;
                
                // Check for generic names
                if (Name.Contains("Related") || 
                    Name.Contains("Entity") || 
                    Name.EndsWith("List") ||
                    Name.StartsWith("Navigation"))
                    return 50;
                
                // Check if the name contains the target table name (good practice)
                if (Name.Contains(TargetTableName) || 
                    (IsCollection && Name == TargetTableName + "s"))
                    return 90;
                
                // Default score for other names
                return 75;
            }
        }
    }
}
