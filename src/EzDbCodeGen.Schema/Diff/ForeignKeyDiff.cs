using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two foreign keys.
    /// </summary>
    public class ForeignKeyDiff : IForeignKeyDiff
    {
        private readonly List<IColumn> _addedColumns = new();
        private readonly List<IColumn> _removedColumns = new();
        private readonly List<IForeignKeyColumnMapping> _addedMappings = new();
        private readonly List<IForeignKeyColumnMapping> _removedMappings = new();
        private bool? _hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyDiff"/> class.
        /// </summary>
        /// <param name="source">The source foreign key.</param>
        /// <param name="target">The target foreign key.</param>
        public ForeignKeyDiff(IForeignKey? source, IForeignKey? target)
        {
            if (source == null && target == null)
            {
                throw new ArgumentNullException(nameof(source), "Both foreign keys cannot be null.");
            }

            Source = source;
            Target = target;
            Original = source;
            New = target;
            
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public IForeignKey? Source { get; }

        /// <inheritdoc/>
        public IForeignKey? Target { get; }

        /// <inheritdoc/>
        public IForeignKey? Original { get; }

        /// <inheritdoc/>
        public IForeignKey? New { get; }

        /// <inheritdoc/>
        public bool NameChanged { get; private set; }

        /// <inheritdoc/>
        public bool ReferencedTableChanged { get; private set; }

        /// <inheritdoc/>
        public bool DeleteBehaviorChanged { get; private set; }

        /// <inheritdoc/>
        public bool UpdateBehaviorChanged { get; private set; }

        /// <inheritdoc/>
        public bool DeleteRuleChanged { get; private set; }

        /// <inheritdoc/>
        public bool UpdateRuleChanged { get; private set; }

        /// <inheritdoc/>
        public bool ReferencedColumnsChanged { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> AddedColumns => _addedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> RemovedColumns => _removedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IForeignKeyColumnMapping> AddedMappings => _addedMappings.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IForeignKeyColumnMapping> RemovedMappings => _removedMappings.AsReadOnly();

        /// <inheritdoc/>
        public bool HasChanged
        {
            get
            {
                if (!_hasChanged.HasValue)
                {
                    _hasChanged = NameChanged ||
                                 ReferencedTableChanged ||
                                 DeleteBehaviorChanged ||
                                 UpdateBehaviorChanged ||
                                 DeleteRuleChanged ||
                                 UpdateRuleChanged ||
                                 ReferencedColumnsChanged ||
                                 AddedColumns.Count > 0 ||
                                 RemovedColumns.Count > 0;
                }

                return _hasChanged.Value;
            }
        }

        private void CalculateDifferences()
        {
            // If either foreign key is null, the foreign key was added or removed
            if (Source == null || Target == null)
            {
                // Mark everything as changed
                NameChanged = true;
                ReferencedTableChanged = true;
                DeleteBehaviorChanged = true;
                UpdateBehaviorChanged = true;
                DeleteRuleChanged = true;
                UpdateRuleChanged = true;
                ReferencedColumnsChanged = true;

                if (Source != null)
                {
                    // Foreign key was removed - all column mappings are removed
                    foreach (var mapping in Source.ColumnMappings)
                    {
                        _removedMappings.Add(mapping);
                        _removedColumns.Add(mapping.SourceColumn);
                    }
                }

                if (Target != null)
                {
                    // Foreign key was added - all column mappings are added
                    foreach (var mapping in Target.ColumnMappings)
                    {
                        _addedMappings.Add(mapping);
                        _addedColumns.Add(mapping.SourceColumn);
                    }
                }

                return;
            }

            // Check if name changed
            NameChanged = !string.Equals(Source.Name, Target.Name, StringComparison.OrdinalIgnoreCase);

            // Check if referenced table changed
            ReferencedTableChanged = !string.Equals(Source.ReferencedTable?.Name, Target.ReferencedTable?.Name, StringComparison.OrdinalIgnoreCase) ||
                                   !string.Equals(Source.ReferencedTable?.Schema, Target.ReferencedTable?.Schema, StringComparison.OrdinalIgnoreCase);

            // Check if delete behavior changed
            DeleteBehaviorChanged = !string.Equals(Source.DeleteBehavior, Target.DeleteBehavior, StringComparison.OrdinalIgnoreCase);
            DeleteRuleChanged = DeleteBehaviorChanged;

            // Check if update behavior changed
            UpdateBehaviorChanged = !string.Equals(Source.UpdateBehavior, Target.UpdateBehavior, StringComparison.OrdinalIgnoreCase);
            UpdateRuleChanged = UpdateBehaviorChanged;

            // Check for column mapping changes
            CompareColumnMappings();
        }

        private void CompareColumnMappings()
        {
            if (Source == null || Target == null)
            {
                return;
            }
            
            var sourceMappingsBySourceColumn = Source.ColumnMappings.ToDictionary(
                m => m.SourceColumn.Name,
                StringComparer.OrdinalIgnoreCase);
            
            var targetMappingsBySourceColumn = Target.ColumnMappings.ToDictionary(
                m => m.SourceColumn.Name,
                StringComparer.OrdinalIgnoreCase);

            // Find removed column mappings
            foreach (var sourceMapping in Source.ColumnMappings)
            {
                if (!targetMappingsBySourceColumn.ContainsKey(sourceMapping.SourceColumn.Name))
                {
                    _removedMappings.Add(sourceMapping);
                    _removedColumns.Add(sourceMapping.SourceColumn);
                }
                else
                {
                    // Check if the referenced column has changed
                    var targetMapping = targetMappingsBySourceColumn[sourceMapping.SourceColumn.Name];
                    if (!string.Equals(sourceMapping.ReferencedColumn.Name, targetMapping.ReferencedColumn.Name, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _removedMappings.Add(sourceMapping);
                        _addedMappings.Add(targetMapping);
                        ReferencedColumnsChanged = true;
                    }
                }
            }

            // Find added column mappings
            foreach (var targetMapping in Target.ColumnMappings)
            {
                if (!sourceMappingsBySourceColumn.ContainsKey(targetMapping.SourceColumn.Name))
                {
                    _addedMappings.Add(targetMapping);
                    _addedColumns.Add(targetMapping.SourceColumn);
                }
            }
        }
    }
}
