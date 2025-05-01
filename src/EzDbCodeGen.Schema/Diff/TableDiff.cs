using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two database tables.
    /// </summary>
    public class TableDiff : ITableDiff
    {
        private readonly List<IColumn> _addedColumns = new();
        private readonly List<IColumn> _removedColumns = new();
        private readonly List<IColumnDiff> _modifiedColumns = new();
        private readonly List<IForeignKeyDiff> _foreignKeyDiffs = new();
        private readonly List<IIndexDiff> _indexDiffs = new();
        private readonly List<IUniqueConstraintDiff> _uniqueConstraintDiffs = new();
        private bool? _hasDifferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDiff"/> class.
        /// </summary>
        /// <param name="source">The source table.</param>
        /// <param name="target">The target table.</param>
        public TableDiff(ITable source, ITable target)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            
            // Initialize non-nullable property with a default value
            PrimaryKeyDiff = new KeyDiff(null, null);
            
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public ITable Source { get; }

        /// <inheritdoc/>
        public ITable Target { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IColumn> AddedColumns => _addedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyList<IColumn> RemovedColumns => _removedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyList<IColumnDiff> ModifiedColumns => _modifiedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IKeyDiff PrimaryKeyDiff { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IForeignKeyDiff> ForeignKeyDiffs => _foreignKeyDiffs.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IIndexDiff> IndexDiffs => _indexDiffs.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IUniqueConstraintDiff> UniqueConstraintDiffs => _uniqueConstraintDiffs.AsReadOnly();

        /// <inheritdoc/>
        public bool NameChanged { get; private set; }

        /// <inheritdoc/>
        public bool SchemaChanged { get; private set; }

        /// <inheritdoc/>
        public bool HasDifferences
        {
            get
            {
                if (!_hasDifferences.HasValue)
                {
                    _hasDifferences = NameChanged || 
                                     SchemaChanged || 
                                     AddedColumns.Count > 0 || 
                                     RemovedColumns.Count > 0 || 
                                     ModifiedColumns.Count > 0 || 
                                     PrimaryKeyDiff != null || 
                                     ForeignKeyDiffs.Count > 0 || 
                                     IndexDiffs.Count > 0 || 
                                     UniqueConstraintDiffs.Count > 0;
                }

                return _hasDifferences.Value;
            }
        }

        /// <inheritdoc/>
        public string GetSummary()
        {
            if (!HasDifferences)
            {
                return $"No differences found for table {Source.Schema}.{Source.Name}.";
            }

            var summary = new StringBuilder();
            summary.AppendLine($"Table: {Source.Schema}.{Source.Name}");
            
            if (NameChanged)
            {
                summary.AppendLine($"- Name changed: {Source.Name} -> {Target.Name}");
            }
            
            if (SchemaChanged)
            {
                summary.AppendLine($"- Schema changed: {Source.Schema} -> {Target.Schema}");
            }
            
            if (AddedColumns.Count > 0)
            {
                summary.AppendLine($"- Added columns: {AddedColumns.Count}");
            }
            
            if (RemovedColumns.Count > 0)
            {
                summary.AppendLine($"- Removed columns: {RemovedColumns.Count}");
            }
            
            if (ModifiedColumns.Count > 0)
            {
                summary.AppendLine($"- Modified columns: {ModifiedColumns.Count}");
            }
            
            if (PrimaryKeyDiff != null)
            {
                summary.AppendLine("- Primary key changed");
            }
            
            if (ForeignKeyDiffs.Count > 0)
            {
                summary.AppendLine($"- Foreign key changes: {ForeignKeyDiffs.Count}");
            }
            
            if (IndexDiffs.Count > 0)
            {
                summary.AppendLine($"- Index changes: {IndexDiffs.Count}");
            }
            
            if (UniqueConstraintDiffs.Count > 0)
            {
                summary.AppendLine($"- Unique constraint changes: {UniqueConstraintDiffs.Count}");
            }
            
            return summary.ToString();
        }

        /// <inheritdoc/>
        public string GetDetailedReport()
        {
            if (!HasDifferences)
            {
                return $"No differences found for table {Source.Schema}.{Source.Name}.";
            }

            var report = new StringBuilder();
            report.AppendLine($"Table: {Source.Schema}.{Source.Name}");
            
            if (NameChanged)
            {
                report.AppendLine($"- Name changed: {Source.Name} -> {Target.Name}");
            }
            
            if (SchemaChanged)
            {
                report.AppendLine($"- Schema changed: {Source.Schema} -> {Target.Schema}");
            }
            
            // Detail added columns
            if (AddedColumns.Count > 0)
            {
                report.AppendLine("- Added columns:");
                foreach (var column in AddedColumns)
                {
                    report.AppendLine($"  * {column.Name} ({column.DataType})");
                    report.AppendLine($"    - Nullable: {column.IsNullable}");
                    if (column.DefaultValue != null)
                    {
                        report.AppendLine($"    - Default: {column.DefaultValue}");
                    }
                }
            }
            
            // Detail removed columns
            if (RemovedColumns.Count > 0)
            {
                report.AppendLine("- Removed columns:");
                foreach (var column in RemovedColumns)
                {
                    report.AppendLine($"  * {column.Name} ({column.DataType})");
                }
            }
            
            // Detail modified columns
            if (ModifiedColumns.Count > 0)
            {
                report.AppendLine("- Modified columns:");
                foreach (var columnDiff in ModifiedColumns)
                {
                    report.AppendLine($"  * {columnDiff.Source.Name}:");
                    
                    if (columnDiff.DataTypeChanged)
                    {
                        report.AppendLine($"    - Data type: {columnDiff.Source.DataType} -> {columnDiff.Target.DataType}");
                    }
                    
                    if (columnDiff.NullabilityChanged)
                    {
                        report.AppendLine($"    - Nullable: {columnDiff.Source.IsNullable} -> {columnDiff.Target.IsNullable}");
                    }
                    
                    if (columnDiff.DefaultValueChanged)
                    {
                        report.AppendLine($"    - Default: {columnDiff.Source.DefaultValue} -> {columnDiff.Target.DefaultValue}");
                    }
                    
                    if (columnDiff.ComputedExpressionChanged)
                    {
                        report.AppendLine($"    - Computed expression changed");
                    }
                    
                    if (columnDiff.MaxLengthChanged)
                    {
                        report.AppendLine($"    - Max length: {columnDiff.Source.MaxLength} -> {columnDiff.Target.MaxLength}");
                    }
                    
                    if (columnDiff.PrecisionChanged)
                    {
                        report.AppendLine($"    - Precision: {columnDiff.Source.Precision} -> {columnDiff.Target.Precision}");
                    }
                    
                    if (columnDiff.ScaleChanged)
                    {
                        report.AppendLine($"    - Scale: {columnDiff.Source.Scale} -> {columnDiff.Target.Scale}");
                    }
                }
            }
            
            // Detail primary key changes
            if (PrimaryKeyDiff != null)
            {
                report.AppendLine("- Primary key changes:");
                
                if (PrimaryKeyDiff.Original == null)
                {
                    report.AppendLine("  * Added primary key");
                }
                else if (PrimaryKeyDiff.New == null)
                {
                    report.AppendLine("  * Removed primary key");
                }
                else
                {
                    report.AppendLine("  * Modified primary key");
                }
                
                if (PrimaryKeyDiff.AddedColumns.Count > 0)
                {
                    report.AppendLine("  * Added columns:");
                    foreach (var column in PrimaryKeyDiff.AddedColumns)
                    {
                        report.AppendLine($"    - {column.Name}");
                    }
                }
                
                if (PrimaryKeyDiff.RemovedColumns.Count > 0)
                {
                    report.AppendLine("  * Removed columns:");
                    foreach (var column in PrimaryKeyDiff.RemovedColumns)
                    {
                        report.AppendLine($"    - {column.Name}");
                    }
                }
            }
            
            // Detail foreign key changes
            if (ForeignKeyDiffs.Count > 0)
            {
                report.AppendLine("- Foreign key changes:");
                foreach (var fkDiff in ForeignKeyDiffs)
                {
                    if (fkDiff.Source == null)
                    {
                        report.AppendLine($"  * Added: {fkDiff.Target.Name}");
                    }
                    else if (fkDiff.Target == null)
                    {
                        report.AppendLine($"  * Removed: {fkDiff.Source.Name}");
                    }
                    else
                    {
                        report.AppendLine($"  * Modified: {fkDiff.Source.Name}");
                        
                        if (fkDiff.NameChanged)
                        {
                            report.AppendLine($"    - Name: {fkDiff.Source.Name} -> {fkDiff.Target.Name}");
                        }
                        
                        if (fkDiff.ReferencedTableChanged)
                        {
                            report.AppendLine($"    - Referenced table: {fkDiff.Source.ReferencedTable.Name} -> {fkDiff.Target.ReferencedTable.Name}");
                        }
                        
                        if (fkDiff.DeleteBehaviorChanged)
                        {
                            report.AppendLine($"    - Delete behavior: {fkDiff.Source.DeleteBehavior} -> {fkDiff.Target.DeleteBehavior}");
                        }
                        
                        if (fkDiff.UpdateBehaviorChanged)
                        {
                            report.AppendLine($"    - Update behavior: {fkDiff.Source.UpdateBehavior} -> {fkDiff.Target.UpdateBehavior}");
                        }
                        
                        if (fkDiff.AddedColumns.Count > 0)
                        {
                            report.AppendLine("    - Added columns:");
                            foreach (var column in fkDiff.AddedColumns)
                            {
                                report.AppendLine($"      * {column.Name}");
                            }
                        }
                        
                        if (fkDiff.RemovedColumns.Count > 0)
                        {
                            report.AppendLine("    - Removed columns:");
                            foreach (var column in fkDiff.RemovedColumns)
                            {
                                report.AppendLine($"      * {column.Name}");
                            }
                        }
                    }
                }
            }
            
            // Detail index changes
            if (IndexDiffs.Count > 0)
            {
                report.AppendLine("- Index changes:");
                foreach (var indexDiff in IndexDiffs)
                {
                    if (indexDiff.Source == null)
                    {
                        report.AppendLine($"  * Added: {indexDiff.Target.Name}");
                    }
                    else if (indexDiff.Target == null)
                    {
                        report.AppendLine($"  * Removed: {indexDiff.Source.Name}");
                    }
                    else
                    {
                        report.AppendLine($"  * Modified: {indexDiff.Source.Name}");
                        
                        if (indexDiff.NameChanged)
                        {
                            report.AppendLine($"    - Name: {indexDiff.Source.Name} -> {indexDiff.Target.Name}");
                        }
                        
                        if (indexDiff.IsUniqueChanged)
                        {
                            report.AppendLine($"    - Unique: {indexDiff.Source.IsUnique} -> {indexDiff.Target.IsUnique}");
                        }
                        
                        if (indexDiff.AddedColumns.Count > 0)
                        {
                            report.AppendLine("    - Added columns:");
                            foreach (var column in indexDiff.AddedColumns)
                            {
                                report.AppendLine($"      * {column.Name}");
                            }
                        }
                        
                        if (indexDiff.RemovedColumns.Count > 0)
                        {
                            report.AppendLine("    - Removed columns:");
                            foreach (var column in indexDiff.RemovedColumns)
                            {
                                report.AppendLine($"      * {column.Name}");
                            }
                        }
                    }
                }
            }
            
            // Detail unique constraint changes
            if (UniqueConstraintDiffs.Count > 0)
            {
                report.AppendLine("- Unique constraint changes:");
                foreach (var ucDiff in UniqueConstraintDiffs)
                {
                    if (ucDiff.Original == null)
                    {
                        report.AppendLine($"  * Added: {ucDiff.New?.Name}");
                    }
                    else if (ucDiff.New == null)
                    {
                        report.AppendLine($"  * Removed: {ucDiff.Original.Name}");
                    }
                    else
                    {
                        report.AppendLine($"  * Modified: {ucDiff.Original.Name}");
                        
                        if (ucDiff.NameChanged)
                        {
                            report.AppendLine($"    - Name: {ucDiff.Original.Name} -> {ucDiff.New.Name}");
                        }
                        
                        if (ucDiff.AddedColumns.Count > 0)
                        {
                            report.AppendLine("    - Added columns:");
                            foreach (var column in ucDiff.AddedColumns)
                            {
                                report.AppendLine($"      * {column.Name}");
                            }
                        }
                        
                        if (ucDiff.RemovedColumns.Count > 0)
                        {
                            report.AppendLine("    - Removed columns:");
                            foreach (var column in ucDiff.RemovedColumns)
                            {
                                report.AppendLine($"      * {column.Name}");
                            }
                        }
                    }
                }
            }
            
            return report.ToString();
        }

        /// <summary>
        /// Calculates all differences between the source and target tables.
        /// </summary>
        private void CalculateDifferences()
        {
            // Check if name or schema changed
            NameChanged = !string.Equals(Source.Name, Target.Name, StringComparison.OrdinalIgnoreCase);
            SchemaChanged = !string.Equals(Source.Schema, Target.Schema, StringComparison.OrdinalIgnoreCase);
            
            // Compare columns
            CompareColumns();
            
            // Compare primary keys
            ComparePrimaryKeys();
            
            // Compare foreign keys
            CompareForeignKeys();
            
            // Compare indexes
            CompareIndexes();
            
            // Compare unique constraints
            CompareUniqueConstraints();
        }

        /// <summary>
        /// Compares columns between source and target tables.
        /// </summary>
        private void CompareColumns()
        {
            var sourceColumnsByName = Source.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            var targetColumnsByName = Target.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            
            // Identify removed columns
            foreach (var sourceColumn in Source.Columns)
            {
                if (!targetColumnsByName.ContainsKey(sourceColumn.Name))
                {
                    _removedColumns.Add(sourceColumn);
                }
            }
            
            // Identify added columns
            foreach (var targetColumn in Target.Columns)
            {
                if (!sourceColumnsByName.ContainsKey(targetColumn.Name))
                {
                    _addedColumns.Add(targetColumn);
                }
            }
            
            // Identify modified columns
            foreach (var targetColumn in Target.Columns)
            {
                if (sourceColumnsByName.TryGetValue(targetColumn.Name, out var sourceColumn))
                {
                    // Create a column diff object
                    var columnDiff = new ColumnDiff(sourceColumn, targetColumn);
                    
                    if (columnDiff.HasDifferences)
                    {
                        _modifiedColumns.Add(columnDiff);
                    }
                }
            }
        }

        /// <summary>
        /// Compares primary keys between source and target tables.
        /// </summary>
        private void ComparePrimaryKeys()
        {
            // Check if either table has no primary key
            if (Source.PrimaryKey == null && Target.PrimaryKey == null)
            {
                return; // No primary keys to compare
            }
            
            if (Source.PrimaryKey == null || Target.PrimaryKey == null)
            {
                // One has a primary key and the other doesn't
                PrimaryKeyDiff = new KeyDiff(Source.PrimaryKey, Target.PrimaryKey);
                return;
            }
            
            // Both have primary keys - compare them
            var pkDiff = new KeyDiff(Source.PrimaryKey, Target.PrimaryKey);
            if (pkDiff.HasChanged)
            {
                PrimaryKeyDiff = pkDiff;
            }
        }

        /// <summary>
        /// Compares foreign keys between source and target tables.
        /// </summary>
        private void CompareForeignKeys()
        {
            // Check if either table has no foreign keys
            if ((Source.ForeignKeys == null || Source.ForeignKeys.Count == 0) && 
                (Target.ForeignKeys == null || Target.ForeignKeys.Count == 0))
            {
                return; // No foreign keys to compare
            }
            
            if (Source.ForeignKeys == null || Source.ForeignKeys.Count == 0)
            {
                // Source has no foreign keys but target does - all target foreign keys are added
                if (Target.ForeignKeys != null)
                {
                    foreach (var targetFk in Target.ForeignKeys)
                    {
                        _foreignKeyDiffs.Add(new ForeignKeyDiff(null, targetFk));
                    }
                }
                return;
            }
            
            if (Target.ForeignKeys == null || Target.ForeignKeys.Count == 0)
            {
                // Target has no foreign keys but source does - all source foreign keys are removed
                foreach (var sourceFk in Source.ForeignKeys)
                {
                    _foreignKeyDiffs.Add(new ForeignKeyDiff(sourceFk, null));
                }
                return;
            }
            
            // Both have foreign keys - compare them
            var sourceFkByName = Source.ForeignKeys.ToDictionary(fk => fk.Name, StringComparer.OrdinalIgnoreCase);
            var targetFkByName = Target.ForeignKeys.ToDictionary(fk => fk.Name, StringComparer.OrdinalIgnoreCase);
            
            // Find removed foreign keys
            foreach (var sourceFk in Source.ForeignKeys)
            {
                if (!targetFkByName.ContainsKey(sourceFk.Name))
                {
                    _foreignKeyDiffs.Add(new ForeignKeyDiff(sourceFk, null));
                }
            }
            
            // Find added foreign keys
            foreach (var targetFk in Target.ForeignKeys)
            {
                if (!sourceFkByName.ContainsKey(targetFk.Name))
                {
                    _foreignKeyDiffs.Add(new ForeignKeyDiff(null, targetFk));
                }
            }
            
            // Find modified foreign keys
            foreach (var targetFk in Target.ForeignKeys)
            {
                if (sourceFkByName.TryGetValue(targetFk.Name, out var sourceFk))
                {
                    var fkDiff = new ForeignKeyDiff(sourceFk, targetFk);
                    if (fkDiff.HasChanged)
                    {
                        _foreignKeyDiffs.Add(fkDiff);
                    }
                }
            }
        }

        /// <summary>
        /// Compares indexes between source and target tables.
        /// </summary>
        private void CompareIndexes()
        {
            // Check if either table has no indexes
            if ((Source.Indexes == null || Source.Indexes.Count == 0) && 
                (Target.Indexes == null || Target.Indexes.Count == 0))
            {
                return; // No indexes to compare
            }
            
            if (Source.Indexes == null || Source.Indexes.Count == 0)
            {
                // Source has no indexes but target does - all target indexes are added
                if (Target.Indexes != null)
                {
                    foreach (var targetIndex in Target.Indexes)
                    {
                        _indexDiffs.Add(new IndexDiff(null, targetIndex));
                    }
                }
                return;
            }
            
            if (Target.Indexes == null || Target.Indexes.Count == 0)
            {
                // Target has no indexes but source does - all source indexes are removed
                foreach (var sourceIndex in Source.Indexes)
                {
                    _indexDiffs.Add(new IndexDiff(sourceIndex, null));
                }
                return;
            }
            
            // Both have indexes - compare them
            var sourceIndexByName = Source.Indexes.ToDictionary(idx => idx.Name, StringComparer.OrdinalIgnoreCase);
            var targetIndexByName = Target.Indexes.ToDictionary(idx => idx.Name, StringComparer.OrdinalIgnoreCase);
            
            // Find removed indexes
            foreach (var sourceIndex in Source.Indexes)
            {
                if (!targetIndexByName.ContainsKey(sourceIndex.Name))
                {
                    _indexDiffs.Add(new IndexDiff(sourceIndex, null));
                }
            }
            
            // Find added indexes
            foreach (var targetIndex in Target.Indexes)
            {
                if (!sourceIndexByName.ContainsKey(targetIndex.Name))
                {
                    _indexDiffs.Add(new IndexDiff(null, targetIndex));
                }
            }
            
            // Find modified indexes
            foreach (var targetIndex in Target.Indexes)
            {
                if (sourceIndexByName.TryGetValue(targetIndex.Name, out var sourceIndex))
                {
                    var indexDiff = new IndexDiff(sourceIndex, targetIndex);
                    if (indexDiff.HasChanged)
                    {
                        _indexDiffs.Add(indexDiff);
                    }
                }
            }
        }

        /// <summary>
        /// Compares unique constraints between source and target tables.
        /// </summary>
        private void CompareUniqueConstraints()
        {
            // Check if either table has no unique constraints
            if ((Source.UniqueConstraints == null || Source.UniqueConstraints.Count == 0) && 
                (Target.UniqueConstraints == null || Target.UniqueConstraints.Count == 0))
            {
                return; // No unique constraints to compare
            }
            
            if (Source.UniqueConstraints == null || Source.UniqueConstraints.Count == 0)
            {
                // Source has no unique constraints but target does - all target unique constraints are added
                if (Target.UniqueConstraints != null)
                {
                    foreach (var targetUc in Target.UniqueConstraints)
                    {
                        _uniqueConstraintDiffs.Add(new UniqueConstraintDiff(null, targetUc));
                    }
                }
                return;
            }
            
            if (Target.UniqueConstraints == null || Target.UniqueConstraints.Count == 0)
            {
                // Target has no unique constraints but source does - all source unique constraints are removed
                foreach (var sourceUc in Source.UniqueConstraints)
                {
                    _uniqueConstraintDiffs.Add(new UniqueConstraintDiff(sourceUc, null));
                }
                return;
            }
            
            // Both have unique constraints - compare them
            var sourceUcByName = Source.UniqueConstraints.ToDictionary(uc => uc.Name, StringComparer.OrdinalIgnoreCase);
            var targetUcByName = Target.UniqueConstraints.ToDictionary(uc => uc.Name, StringComparer.OrdinalIgnoreCase);
            
            // Find removed unique constraints
            foreach (var sourceUc in Source.UniqueConstraints)
            {
                if (!targetUcByName.ContainsKey(sourceUc.Name))
                {
                    _uniqueConstraintDiffs.Add(new UniqueConstraintDiff(sourceUc, null));
                }
            }
            
            // Find added unique constraints
            foreach (var targetUc in Target.UniqueConstraints)
            {
                if (!sourceUcByName.ContainsKey(targetUc.Name))
                {
                    _uniqueConstraintDiffs.Add(new UniqueConstraintDiff(null, targetUc));
                }
            }
            
            // Find modified unique constraints
            foreach (var targetUc in Target.UniqueConstraints)
            {
                if (sourceUcByName.TryGetValue(targetUc.Name, out var sourceUc))
                {
                    var ucDiff = new UniqueConstraintDiff(sourceUc, targetUc);
                    if (ucDiff.HasChanged)
                    {
                        _uniqueConstraintDiffs.Add(ucDiff);
                    }
                }
            }
        }
    }
}
