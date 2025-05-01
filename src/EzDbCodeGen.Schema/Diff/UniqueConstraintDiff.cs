using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two unique constraints.
    /// </summary>
    public class UniqueConstraintDiff : IUniqueConstraintDiff
    {
        private readonly List<IColumn> _addedColumns = new();
        private readonly List<IColumn> _removedColumns = new();
        private bool? _hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintDiff"/> class.
        /// </summary>
        /// <param name="source">The source unique constraint.</param>
        /// <param name="target">The target unique constraint.</param>
        public UniqueConstraintDiff(IUniqueConstraint? source, IUniqueConstraint? target)
        {
            if (source == null && target == null)
            {
                throw new ArgumentNullException(nameof(source), "Both unique constraints cannot be null.");
            }

            Source = source;
            Target = target;
            
            // Implement interface properties
            Original = source;
            New = target;
            
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public IUniqueConstraint? Source { get; }

        /// <inheritdoc/>
        public IUniqueConstraint? Target { get; }

        /// <inheritdoc/>
        public IUniqueConstraint? Original { get; }

        /// <inheritdoc/>
        public IUniqueConstraint? New { get; }

        /// <inheritdoc/>
        public bool NameChanged { get; private set; }

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
                                 IsClusteredChanged ||
                                 AddedColumns.Count > 0 ||
                                 RemovedColumns.Count > 0;
                }

                return _hasChanged.Value;
            }
        }

        private void CalculateDifferences()
        {
            // If either unique constraint is null, the constraint was added or removed
            if (Source == null || Target == null)
            {
                // Mark everything as changed
                NameChanged = true;
                IsClusteredChanged = true;

                if (Source != null)
                {
                    // Unique constraint was removed - all columns are removed
                    foreach (var column in Source.Columns)
                    {
                        _removedColumns.Add(column);
                    }
                }

                if (Target != null)
                {
                    // Unique constraint was added - all columns are added
                    foreach (var column in Target.Columns)
                    {
                        _addedColumns.Add(column);
                    }
                }

                return;
            }

            // Check if name changed
            NameChanged = !string.Equals(Source.Name, Target.Name, StringComparison.OrdinalIgnoreCase);

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
            
            var sourceColumnsByName = Source.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            var targetColumnsByName = Target.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            // Find removed columns
            foreach (var sourceColumn in Source.Columns)
            {
                if (!targetColumnsByName.ContainsKey(sourceColumn.Name))
                {
                    _removedColumns.Add(sourceColumn);
                }
            }

            // Find added columns
            foreach (var targetColumn in Target.Columns)
            {
                if (!sourceColumnsByName.ContainsKey(targetColumn.Name))
                {
                    _addedColumns.Add(targetColumn);
                }
            }
        }
    }
}
