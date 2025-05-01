using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two database indexes.
    /// </summary>
    public class IndexDiff : IIndexDiff
    {
        private readonly List<IColumn> _addedColumns = new();
        private readonly List<IColumn> _removedColumns = new();
        private bool? _hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexDiff"/> class.
        /// </summary>
        /// <param name="source">The source index.</param>
        /// <param name="target">The target index.</param>
        public IndexDiff(IIndex? source, IIndex? target)
        {
            if (source == null && target == null)
            {
                throw new ArgumentNullException(nameof(source), "Both indexes cannot be null.");
            }

            Source = source;
            Target = target;
            
            // Implement interface properties
            Original = source;
            New = target;
            
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public IIndex? Source { get; }

        /// <inheritdoc/>
        public IIndex? Target { get; }

        /// <inheritdoc/>
        public IIndex? Original { get; }

        /// <inheritdoc/>
        public IIndex? New { get; }

        /// <inheritdoc/>
        public bool NameChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsUniqueChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsPrimaryKeyChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsClusteredChanged { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> AddedColumns => _addedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> RemovedColumns => _removedColumns.AsReadOnly();

        /// <inheritdoc/>
        public bool HasChanged 
        {
            get
            {
                if (!_hasChanged.HasValue)
                {
                    _hasChanged = NameChanged ||
                                 IsUniqueChanged ||
                                 IsPrimaryKeyChanged ||
                                 IsClusteredChanged ||
                                 AddedColumns.Count > 0 ||
                                 RemovedColumns.Count > 0;
                }

                return _hasChanged.Value;
            }
        }

        private void CalculateDifferences()
        {
            // If either index is null, the index was added or removed
            if (Source == null || Target == null)
            {
                // Mark everything as changed
                NameChanged = true;
                IsUniqueChanged = true;
                IsPrimaryKeyChanged = true;
                IsClusteredChanged = true;

                if (Source != null)
                {
                    // Index was removed - all columns are removed
                    foreach (var indexColumn in Source.Columns)
                    {
                        _removedColumns.Add(indexColumn.Column);
                    }
                }

                if (Target != null)
                {
                    // Index was added - all columns are added
                    foreach (var indexColumn in Target.Columns)
                    {
                        _addedColumns.Add(indexColumn.Column);
                    }
                }

                return;
            }

            // Check if name changed
            NameChanged = !string.Equals(Source.Name, Target.Name, StringComparison.OrdinalIgnoreCase);

            // Check if uniqueness changed
            IsUniqueChanged = Source.IsUnique != Target.IsUnique;

            // Check if primary key status changed - this property is not in IIndex, so we'll check if it exists via reflection
            IsPrimaryKeyChanged = false;
            try
            {
                var sourceProp = Source.GetType().GetProperty("IsPrimaryKey");
                var targetProp = Target.GetType().GetProperty("IsPrimaryKey");
                if (sourceProp != null && targetProp != null)
                {
                    var sourceValue = sourceProp.GetValue(Source);
                    var targetValue = targetProp.GetValue(Target);
                    
                    if (sourceValue is bool sourceIsPrimaryKey && targetValue is bool targetIsPrimaryKey)
                    {
                        IsPrimaryKeyChanged = sourceIsPrimaryKey != targetIsPrimaryKey;
                    }
                }
            }
            catch
            {
                // If we can't get the property, assume it hasn't changed
            }

            // Check if clustered status changed
            IsClusteredChanged = Source.IsClustered != Target.IsClustered;

            // Check for column changes
            CompareColumns();
        }

        private void CompareColumns()
        {
            if (Source == null || Target == null)
            {
                return;
            }
            
            var sourceColumnsByName = Source.Columns.ToDictionary(c => c.Column.Name, StringComparer.OrdinalIgnoreCase);
            var targetColumnsByName = Target.Columns.ToDictionary(c => c.Column.Name, StringComparer.OrdinalIgnoreCase);

            // Find removed columns
            foreach (var sourceIndexColumn in Source.Columns)
            {
                if (!targetColumnsByName.ContainsKey(sourceIndexColumn.Column.Name))
                {
                    _removedColumns.Add(sourceIndexColumn.Column);
                }
            }

            // Find added columns
            foreach (var targetIndexColumn in Target.Columns)
            {
                if (!sourceColumnsByName.ContainsKey(targetIndexColumn.Column.Name))
                {
                    _addedColumns.Add(targetIndexColumn.Column);
                }
            }
        }
    }
}
