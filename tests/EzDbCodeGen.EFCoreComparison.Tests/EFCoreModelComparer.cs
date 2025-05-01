using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Compares EF Core models with database schemas
    /// </summary>
    public class EFCoreModelComparer
    {
        private readonly ILogger _logger;

        public EFCoreModelComparer(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Analyzes relationships in an EF Core model
        /// </summary>
        /// <param name="dbContext">The EF Core DbContext</param>
        /// <returns>A list of relationships detected in the EF Core model</returns>
        public List<RelationshipInfo> AnalyzeEFCoreRelationships(DbContext dbContext)
        {
            _logger.LogInformation("Analyzing relationships in EF Core model...");

            var efRelationships = new List<RelationshipInfo>();
            var efModel = dbContext.Model;

            // Get all EF Core relationships
            foreach (var entityType in efModel.GetEntityTypes())
            {
                foreach (var navigation in entityType.GetNavigations())
                {
                    var foreignKey = navigation.ForeignKey;
                    
                    // Skip duplicate entries (each relationship has two navigations)
                    if (efRelationships.Any(r => 
                        r.SourceTable == foreignKey.PrincipalEntityType.GetTableName() && 
                        r.TargetTable == foreignKey.DeclaringEntityType.GetTableName() &&
                        r.IsCollection == !navigation.IsCollection))
                    {
                        continue;
                    }

                    efRelationships.Add(new RelationshipInfo
                    {
                        SourceTable = foreignKey.DeclaringEntityType.GetTableName(),
                        TargetTable = foreignKey.PrincipalEntityType.GetTableName(),
                        SourceProperty = navigation.Name,
                        IsCollection = navigation.IsCollection,
                        RelationshipType = navigation.IsCollection ? 
                            "OneToMany" : 
                            (foreignKey.PrincipalToDependent?.IsCollection == true ? "ManyToOne" : "OneToOne")
                    });
                }
            }

            return efRelationships;
        }

        /// <summary>
        /// Analyzes column types in an EF Core model
        /// </summary>
        /// <param name="dbContext">The EF Core DbContext</param>
        /// <returns>A dictionary mapping table.column to its type</returns>
        public Dictionary<string, string> AnalyzeEFCoreColumnTypes(DbContext dbContext)
        {
            _logger.LogInformation("Analyzing column types in EF Core model...");

            var columnTypes = new Dictionary<string, string>();
            var efModel = dbContext.Model;

            foreach (var entityType in efModel.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                
                foreach (var property in entityType.GetProperties())
                {
                    var columnName = property.GetColumnName();
                    var columnType = property.ClrType.Name;
                    
                    var key = $"{tableName}.{columnName}";
                    columnTypes[key] = columnType;
                }
            }

            return columnTypes;
        }
    }

    /// <summary>
    /// Represents information about a relationship
    /// </summary>
    public class RelationshipInfo
    {
        public string SourceTable { get; set; }
        public string TargetTable { get; set; }
        public string SourceProperty { get; set; }
        public bool IsCollection { get; set; }
        public string RelationshipType { get; set; }

        public override string ToString()
        {
            return $"{SourceTable} -> {TargetTable} via {SourceProperty} ({RelationshipType})";
        }
    }
}
