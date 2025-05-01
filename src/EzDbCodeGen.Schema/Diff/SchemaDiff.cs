using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two database schemas.
    /// </summary>
    public class SchemaDiff : ISchemaDiff
    {
        private readonly List<ITableDiff> _modifiedTables = new();
        private readonly List<IRelationshipDiff> _relationshipDiffs = new();
        private readonly List<IViewDiff> _viewDiffs = new();
        private readonly List<IStoredProcedureDiff> _storedProcedureDiffs = new();
        private readonly List<IFunctionDiff> _functionDiffs = new();
        private bool? _hasDifferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaDiff"/> class.
        /// </summary>
        /// <param name="source">The source schema.</param>
        /// <param name="target">The target schema.</param>
        public SchemaDiff(IDatabaseSchema source, IDatabaseSchema target)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            
            // Initialize non-nullable properties with empty collections
            AddedTables = new List<ITable>().AsReadOnly();
            RemovedTables = new List<ITable>().AsReadOnly();
            
            // Calculate differences
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public IDatabaseSchema Source { get; }

        /// <inheritdoc/>
        public IDatabaseSchema Target { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ITable> AddedTables { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<ITable> RemovedTables { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<ITableDiff> ModifiedTables => _modifiedTables.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IRelationshipDiff> RelationshipDiffs => _relationshipDiffs.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IViewDiff> ViewDiffs => _viewDiffs.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IStoredProcedureDiff> StoredProcedureDiffs => _storedProcedureDiffs.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IFunctionDiff> FunctionDiffs => _functionDiffs.AsReadOnly();

        /// <inheritdoc/>
        public bool HasDifferences
        {
            get
            {
                if (!_hasDifferences.HasValue)
                {
                    _hasDifferences = AddedTables.Count > 0 ||
                                     RemovedTables.Count > 0 ||
                                     ModifiedTables.Count > 0 ||
                                     RelationshipDiffs.Count > 0 ||
                                     ViewDiffs.Count > 0 ||
                                     StoredProcedureDiffs.Count > 0 ||
                                     FunctionDiffs.Count > 0;
                }

                return _hasDifferences.Value;
            }
        }

        /// <inheritdoc/>
        public string GetSummary()
        {
            if (!HasDifferences)
            {
                return "No differences found between the schemas.";
            }

            var builder = new StringBuilder();
            builder.AppendLine("Schema Differences Summary:");
            builder.AppendLine($"- Added Tables: {AddedTables.Count}");
            builder.AppendLine($"- Removed Tables: {RemovedTables.Count}");
            builder.AppendLine($"- Modified Tables: {ModifiedTables.Count}");
            builder.AppendLine($"- Relationship Changes: {RelationshipDiffs.Count}");
            builder.AppendLine($"- View Changes: {ViewDiffs.Count}");
            builder.AppendLine($"- Stored Procedure Changes: {StoredProcedureDiffs.Count}");
            builder.AppendLine($"- Function Changes: {FunctionDiffs.Count}");
            
            return builder.ToString();
        }

        /// <inheritdoc/>
        public string GetDetailedReport()
        {
            if (!HasDifferences)
            {
                return "No differences found between the schemas.";
            }

            var builder = new StringBuilder();
            builder.AppendLine("Detailed Schema Differences Report:");
            
            // Report added tables
            if (AddedTables.Count > 0)
            {
                builder.AppendLine("\nAdded Tables:");
                foreach (var table in AddedTables)
                {
                    builder.AppendLine($"  - {table.Schema}.{table.Name} ({table.Columns.Count} columns)");
                }
            }
            
            // Report removed tables
            if (RemovedTables.Count > 0)
            {
                builder.AppendLine("\nRemoved Tables:");
                foreach (var table in RemovedTables)
                {
                    builder.AppendLine($"  - {table.Schema}.{table.Name} ({table.Columns.Count} columns)");
                }
            }
            
            // Report modified tables
            if (ModifiedTables.Count > 0)
            {
                builder.AppendLine("\nModified Tables:");
                foreach (var tableDiff in ModifiedTables)
                {
                    builder.AppendLine($"  - {tableDiff.Source.Schema}.{tableDiff.Source.Name}:");
                    
                    if (tableDiff.AddedColumns.Count > 0)
                    {
                        builder.AppendLine("    Added Columns:");
                        foreach (var column in tableDiff.AddedColumns)
                        {
                            builder.AppendLine($"      - {column.Name} ({column.DataType})");
                        }
                    }
                    
                    if (tableDiff.RemovedColumns.Count > 0)
                    {
                        builder.AppendLine("    Removed Columns:");
                        foreach (var column in tableDiff.RemovedColumns)
                        {
                            builder.AppendLine($"      - {column.Name} ({column.DataType})");
                        }
                    }
                    
                    if (tableDiff.ModifiedColumns.Count > 0)
                    {
                        builder.AppendLine("    Changed Columns:");
                        foreach (var column in tableDiff.ModifiedColumns)
                        {
                            builder.AppendLine($"      - {column.Source.Name}");
                        }
                    }
                }
            }
            
            // Report relationship changes
            if (RelationshipDiffs.Count > 0)
            {
                builder.AppendLine("\nRelationship Changes:");
                foreach (var relationshipDiff in RelationshipDiffs)
                {
                    if (relationshipDiff.Source == null && relationshipDiff.Target != null && relationshipDiff.Target.SourceTable != null && relationshipDiff.Target.TargetTable != null)
                    {
                        builder.AppendLine($"  - Added: {relationshipDiff.Target.SourceTable.Name} -> {relationshipDiff.Target.TargetTable.Name} ({relationshipDiff.Target.Type})");
                    }
                    else if (relationshipDiff.Target == null && relationshipDiff.Source != null && relationshipDiff.Source.SourceTable != null && relationshipDiff.Source.TargetTable != null)
                    {
                        builder.AppendLine($"  - Removed: {relationshipDiff.Source.SourceTable.Name} -> {relationshipDiff.Source.TargetTable.Name} ({relationshipDiff.Source.Type})");
                    }
                    else
                    {
                        builder.AppendLine($"  - Modified: {relationshipDiff.Source?.SourceTable?.Name ?? "Unknown"} -> {relationshipDiff.Source?.TargetTable?.Name ?? "Unknown"}");
                        
                        if (relationshipDiff.TypeChanged)
                        {
                            builder.AppendLine($"    - Type changed: {relationshipDiff.Source?.Type} -> {relationshipDiff.Target?.Type}");
                        }
                        
                        if (relationshipDiff.SourceNavigationPropertyNameChanged)
                        {
                            builder.AppendLine($"    - Source navigation property: {relationshipDiff.Source?.SourceNavigationPropertyName} -> {relationshipDiff.Target?.SourceNavigationPropertyName}");
                        }
                        
                        if (relationshipDiff.TargetNavigationPropertyNameChanged)
                        {
                            builder.AppendLine($"    - Target navigation property: {relationshipDiff.Source?.TargetNavigationPropertyName} -> {relationshipDiff.Target?.TargetNavigationPropertyName}");
                        }
                        
                        if (relationshipDiff.DeleteBehaviorChanged)
                        {
                            builder.AppendLine($"    - Delete behavior: {relationshipDiff.Source?.DeleteBehavior} -> {relationshipDiff.Target?.DeleteBehavior}");
                        }
                    }
                }
            }
            
            // Report view changes
            if (ViewDiffs.Count > 0)
            {
                builder.AppendLine("\nView Changes:");
                foreach (var viewDiff in ViewDiffs)
                {
                    if (viewDiff.Original == null && viewDiff.New != null)
                    {
                        builder.AppendLine($"  - Added: {viewDiff.New.Schema}.{viewDiff.New.Name} ({viewDiff.New.Columns?.Count ?? 0} columns)");
                    }
                    else if (viewDiff.New == null && viewDiff.Original != null)
                    {
                        builder.AppendLine($"  - Removed: {viewDiff.Original.Schema}.{viewDiff.Original.Name} ({viewDiff.Original.Columns?.Count ?? 0} columns)");
                    }
                    else
                    {
                        builder.AppendLine($"  - Modified: {viewDiff.Original?.Schema ?? "Unknown"}.{viewDiff.Original?.Name ?? "Unknown"}:");
                        
                        if (viewDiff.AddedColumns.Count > 0)
                        {
                            builder.AppendLine("    Added Columns:");
                            foreach (var column in viewDiff.AddedColumns)
                            {
                                builder.AppendLine($"      - {column.Name} ({column.DataType})");
                            }
                        }
                        
                        if (viewDiff.RemovedColumns.Count > 0)
                        {
                            builder.AppendLine("    Removed Columns:");
                            foreach (var column in viewDiff.RemovedColumns)
                            {
                                builder.AppendLine($"      - {column.Name} ({column.DataType})");
                            }
                        }
                        
                        if (viewDiff.DefinitionChanged)
                        {
                            builder.AppendLine("    Definition changed");
                        }
                        
                        if (viewDiff.IsIndexedChanged)
                        {
                            builder.AppendLine("    Indexing changed");
                        }
                    }
                }
            }
            
            // Report stored procedure changes
            if (StoredProcedureDiffs.Count > 0)
            {
                builder.AppendLine("\nStored Procedure Changes:");
                foreach (var procDiff in StoredProcedureDiffs)
                {
                    if (procDiff.Original == null && procDiff.New != null)
                    {
                        builder.AppendLine($"  - Added: {procDiff.New.Schema}.{procDiff.New.Name}");
                    }
                    else if (procDiff.New == null && procDiff.Original != null)
                    {
                        builder.AppendLine($"  - Removed: {procDiff.Original.Schema}.{procDiff.Original.Name}");
                    }
                    else if (procDiff.New != null && procDiff.Original != null)
                    {
                        builder.AppendLine($"  - Modified: {procDiff.Original.Schema}.{procDiff.Original.Name}:");
                        
                        if (procDiff.AddedParameters.Count > 0)
                        {
                            builder.AppendLine("    Added Parameters:");
                            foreach (var param in procDiff.AddedParameters)
                            {
                                builder.AppendLine($"      - {param.Name} ({param.DataType})");
                            }
                        }
                        
                        if (procDiff.RemovedParameters.Count > 0)
                        {
                            builder.AppendLine("    Removed Parameters:");
                            foreach (var param in procDiff.RemovedParameters)
                            {
                                builder.AppendLine($"      - {param.Name} ({param.DataType})");
                            }
                        }
                        
                        if (procDiff.ChangedParameters.Count > 0)
                        {
                            builder.AppendLine("    Changed Parameters:");
                            foreach (var param in procDiff.ChangedParameters)
                            {
                                builder.AppendLine($"      - {param.Name}");
                            }
                        }
                        
                        if (procDiff.DefinitionChanged)
                        {
                            builder.AppendLine("    Definition changed");
                        }
                    }
                }
            }
            
            // Report function changes
            if (FunctionDiffs.Count > 0)
            {
                builder.AppendLine("\nFunction Changes:");
                foreach (var funcDiff in FunctionDiffs)
                {
                    if (funcDiff.Original == null && funcDiff.New != null)
                    {
                        builder.AppendLine($"  - Added: {funcDiff.New.Schema}.{funcDiff.New.Name}");
                    }
                    else if (funcDiff.New == null && funcDiff.Original != null)
                    {
                        builder.AppendLine($"  - Removed: {funcDiff.Original.Schema}.{funcDiff.Original.Name}");
                    }
                    else if (funcDiff.New != null && funcDiff.Original != null)
                    {
                        builder.AppendLine($"  - Modified: {funcDiff.Original.Schema}.{funcDiff.Original.Name}:");
                        
                        if (funcDiff.ReturnTypeChanged)
                        {
                            builder.AppendLine($"    Return type changed: {funcDiff.Original.ReturnType} -> {funcDiff.New.ReturnType}");
                        }
                        
                        if (funcDiff.IsTableValuedChanged)
                        {
                            builder.AppendLine($"    Function type changed: {(funcDiff.Original.IsTableValued ? "Table-valued" : "Scalar")} -> {(funcDiff.New.IsTableValued ? "Table-valued" : "Scalar")}");
                        }
                        
                        if (funcDiff.AddedParameters.Count > 0)
                        {
                            builder.AppendLine("    Added Parameters:");
                            foreach (var param in funcDiff.AddedParameters)
                            {
                                builder.AppendLine($"      - {param.Name} ({param.DataType})");
                            }
                        }
                        
                        if (funcDiff.RemovedParameters.Count > 0)
                        {
                            builder.AppendLine("    Removed Parameters:");
                            foreach (var param in funcDiff.RemovedParameters)
                            {
                                builder.AppendLine($"      - {param.Name} ({param.DataType})");
                            }
                        }
                        
                        if (funcDiff.DefinitionChanged)
                        {
                            builder.AppendLine("    Definition changed");
                        }
                    }
                }
            }
            
            return builder.ToString();
        }

        private void CalculateDifferences()
        {
            // Compare tables
            CompareTablesAndViews();
            
            // Compare stored procedures
            CompareStoredProcedures();
            
            // Compare functions
            CompareFunctions();
            
            // Compare relationships
            CompareRelationships();
        }

        private void CompareTablesAndViews()
        {
            // Compare tables
            var sourceTablesByName = Source.Tables.ToDictionary(
                t => $"{t.Schema}.{t.Name}",
                StringComparer.OrdinalIgnoreCase);
            
            var targetTablesByName = Target.Tables.ToDictionary(
                t => $"{t.Schema}.{t.Name}",
                StringComparer.OrdinalIgnoreCase);
            
            // Find added and removed tables
            var addedTables = new List<ITable>();
            var removedTables = new List<ITable>();
            
            foreach (var table in Target.Tables)
            {
                var key = $"{table.Schema}.{table.Name}";
                if (!sourceTablesByName.ContainsKey(key))
                {
                    addedTables.Add(table);
                }
            }
            
            foreach (var table in Source.Tables)
            {
                var key = $"{table.Schema}.{table.Name}";
                if (!targetTablesByName.ContainsKey(key))
                {
                    removedTables.Add(table);
                }
            }
            
            AddedTables = addedTables.AsReadOnly();
            RemovedTables = removedTables.AsReadOnly();
            
            // Find modified tables
            foreach (var sourceTable in Source.Tables)
            {
                var key = $"{sourceTable.Schema}.{sourceTable.Name}";
                if (targetTablesByName.TryGetValue(key, out var targetTable))
                {
                    var tableDiff = new TableDiff(sourceTable, targetTable);
                    if (tableDiff.HasDifferences)
                    {
                        _modifiedTables.Add(tableDiff);
                    }
                }
            }
            
            // Compare views
            if (Source.Views != null && Target.Views != null)
            {
                var sourceViewsByName = Source.Views.ToDictionary(
                    v => $"{v.Schema}.{v.Name}",
                    StringComparer.OrdinalIgnoreCase);
                
                var targetViewsByName = Target.Views.ToDictionary(
                    v => $"{v.Schema}.{v.Name}",
                    StringComparer.OrdinalIgnoreCase);
                
                // Find added views
                foreach (var view in Target.Views)
                {
                    var key = $"{view.Schema}.{view.Name}";
                    if (!sourceViewsByName.ContainsKey(key))
                    {
                        _viewDiffs.Add(new ViewDiff(null, view));
                    }
                }
                
                // Find removed views
                foreach (var view in Source.Views)
                {
                    var key = $"{view.Schema}.{view.Name}";
                    if (!targetViewsByName.ContainsKey(key))
                    {
                        _viewDiffs.Add(new ViewDiff(view, null));
                    }
                }
                
                // Find modified views
                foreach (var sourceView in Source.Views)
                {
                    var key = $"{sourceView.Schema}.{sourceView.Name}";
                    if (targetViewsByName.TryGetValue(key, out var targetView))
                    {
                        var viewDiff = new ViewDiff(sourceView, targetView);
                        if (viewDiff.HasChanged)
                        {
                            _viewDiffs.Add(viewDiff);
                        }
                    }
                }
            }
            else if (Source.Views != null)
            {
                // All views were removed
                foreach (var view in Source.Views)
                {
                    _viewDiffs.Add(new ViewDiff(view, null));
                }
            }
            else if (Target.Views != null)
            {
                // All views were added
                foreach (var view in Target.Views)
                {
                    _viewDiffs.Add(new ViewDiff(null, view));
                }
            }
        }

        private void CompareStoredProcedures()
        {
            if (Source.StoredProcedures != null && Target.StoredProcedures != null)
            {
                var sourceProcsByName = Source.StoredProcedures.ToDictionary(
                    p => $"{p.Schema}.{p.Name}",
                    StringComparer.OrdinalIgnoreCase);
                
                var targetProcsByName = Target.StoredProcedures.ToDictionary(
                    p => $"{p.Schema}.{p.Name}",
                    StringComparer.OrdinalIgnoreCase);
                
                // Find added procs
                foreach (var proc in Target.StoredProcedures)
                {
                    var key = $"{proc.Schema}.{proc.Name}";
                    if (!sourceProcsByName.ContainsKey(key))
                    {
                        _storedProcedureDiffs.Add(new StoredProcedureDiff(null!, proc));
                    }
                }
                
                // Find removed procs
                foreach (var proc in Source.StoredProcedures)
                {
                    var key = $"{proc.Schema}.{proc.Name}";
                    if (!targetProcsByName.ContainsKey(key))
                    {
                        _storedProcedureDiffs.Add(new StoredProcedureDiff(proc, null!));
                    }
                }
                
                // Find modified procs
                foreach (var sourceProc in Source.StoredProcedures)
                {
                    var key = $"{sourceProc.Schema}.{sourceProc.Name}";
                    if (targetProcsByName.TryGetValue(key, out var targetProc))
                    {
                        var procDiff = new StoredProcedureDiff(sourceProc, targetProc);
                        if (procDiff.HasChanged)
                        {
                            _storedProcedureDiffs.Add(procDiff);
                        }
                    }
                }
            }
            else if (Source.StoredProcedures != null)
            {
                // All procs were removed
                foreach (var proc in Source.StoredProcedures)
                {
                    _storedProcedureDiffs.Add(new StoredProcedureDiff(proc, null!));
                }
            }
            else if (Target.StoredProcedures != null)
            {
                // All procs were added
                foreach (var proc in Target.StoredProcedures)
                {
                    _storedProcedureDiffs.Add(new StoredProcedureDiff(null!, proc));
                }
            }
        }

        private void CompareFunctions()
        {
            if (Source.Functions != null && Target.Functions != null)
            {
                var sourceFuncsByName = Source.Functions.ToDictionary(
                    f => $"{f.Schema}.{f.Name}",
                    StringComparer.OrdinalIgnoreCase);
                
                var targetFuncsByName = Target.Functions.ToDictionary(
                    f => $"{f.Schema}.{f.Name}",
                    StringComparer.OrdinalIgnoreCase);
                
                // Find added functions
                foreach (var func in Target.Functions)
                {
                    var key = $"{func.Schema}.{func.Name}";
                    if (!sourceFuncsByName.ContainsKey(key))
                    {
                        _functionDiffs.Add(new FunctionDiff(null!, func));
                    }
                }
                
                // Find removed functions
                foreach (var func in Source.Functions)
                {
                    var key = $"{func.Schema}.{func.Name}";
                    if (!targetFuncsByName.ContainsKey(key))
                    {
                        _functionDiffs.Add(new FunctionDiff(func, null!));
                    }
                }
                
                // Find modified functions
                foreach (var sourceFunc in Source.Functions)
                {
                    var key = $"{sourceFunc.Schema}.{sourceFunc.Name}";
                    if (targetFuncsByName.TryGetValue(key, out var targetFunc))
                    {
                        var funcDiff = new FunctionDiff(sourceFunc, targetFunc);
                        if (funcDiff.HasChanged)
                        {
                            _functionDiffs.Add(funcDiff);
                        }
                    }
                }
            }
            else if (Source.Functions != null)
            {
                // All functions were removed
                foreach (var func in Source.Functions)
                {
                    _functionDiffs.Add(new FunctionDiff(func, null!));
                }
            }
            else if (Target.Functions != null)
            {
                // All functions were added
                foreach (var func in Target.Functions)
                {
                    _functionDiffs.Add(new FunctionDiff(null!, func));
                }
            }
        }

        private void CompareRelationships()
        {
            // Compare relationships
            if (Source.Relationships != null && Target.Relationships != null)
            {
                // Create a composite key for relationships: SourceTable.Name+TargetTable.Name+Type
                var sourceRelationshipsByKey = Source.Relationships.ToDictionary(
                    r => $"{r.SourceTable.Schema}.{r.SourceTable.Name}_{r.TargetTable.Schema}.{r.TargetTable.Name}_{r.Type}",
                    StringComparer.OrdinalIgnoreCase);
                
                var targetRelationshipsByKey = Target.Relationships.ToDictionary(
                    r => $"{r.SourceTable.Schema}.{r.SourceTable.Name}_{r.TargetTable.Schema}.{r.TargetTable.Name}_{r.Type}",
                    StringComparer.OrdinalIgnoreCase);
                
                // Find added relationships
                foreach (var rel in Target.Relationships)
                {
                    var key = $"{rel.SourceTable.Schema}.{rel.SourceTable.Name}_{rel.TargetTable.Schema}.{rel.TargetTable.Name}_{rel.Type}";
                    if (!sourceRelationshipsByKey.ContainsKey(key))
                    {
                        _relationshipDiffs.Add(new RelationshipDiff(null, rel));
                    }
                }
                
                // Find removed relationships
                foreach (var rel in Source.Relationships)
                {
                    var key = $"{rel.SourceTable.Schema}.{rel.SourceTable.Name}_{rel.TargetTable.Schema}.{rel.TargetTable.Name}_{rel.Type}";
                    if (!targetRelationshipsByKey.ContainsKey(key))
                    {
                        _relationshipDiffs.Add(new RelationshipDiff(rel, null));
                    }
                }
                
                // Find modified relationships
                foreach (var sourceRel in Source.Relationships)
                {
                    var key = $"{sourceRel.SourceTable.Schema}.{sourceRel.SourceTable.Name}_{sourceRel.TargetTable.Schema}.{sourceRel.TargetTable.Name}_{sourceRel.Type}";
                    if (targetRelationshipsByKey.TryGetValue(key, out var targetRel))
                    {
                        var relDiff = new RelationshipDiff(sourceRel, targetRel);
                        if (relDiff.HasChanged)
                        {
                            _relationshipDiffs.Add(relDiff);
                        }
                    }
                }
            }
            else if (Source.Relationships != null)
            {
                // All relationships were removed
                foreach (var rel in Source.Relationships)
                {
                    _relationshipDiffs.Add(new RelationshipDiff(rel, null));
                }
            }
            else if (Target.Relationships != null)
            {
                // All relationships were added
                foreach (var rel in Target.Relationships)
                {
                    _relationshipDiffs.Add(new RelationshipDiff(null, rel));
                }
            }
        }
    }
}
