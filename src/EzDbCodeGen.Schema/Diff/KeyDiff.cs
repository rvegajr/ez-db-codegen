using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two keys (primary key or unique constraint).
    /// </summary>
    public class KeyDiff : IKeyDiff
    {
        private readonly List<IColumn> _addedColumns = new();
        private readonly List<IColumn> _removedColumns = new();
        private readonly List<IColumn> _reorderedColumns = new();
        private bool? _hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyDiff"/> class.
        /// </summary>
        /// <param name="source">The source key.</param>
        /// <param name="target">The target key.</param>
        /// <exception cref="ArgumentNullException">Thrown when both source and target are null.</exception>
        public KeyDiff(IKey? source, IKey? target)
        {
            if (source == null && target == null)
            {
                throw new ArgumentNullException(nameof(source), "Both keys cannot be null.");
            }

            var emptyKey = new PrimaryKeyModel { Name = "Empty" };
            Source = source ?? emptyKey;
            Target = target ?? emptyKey;
            
            // Implement interface properties
            Original = source ?? emptyKey;
            New = target ?? emptyKey;
            
            CalculateDifferences();
        }

        /// <summary>
        /// Default constructor for creating an empty KeyDiff
        /// </summary>
        public KeyDiff()
        {
            Source = new PrimaryKeyModel { Name = "Empty" };
            Target = new PrimaryKeyModel { Name = "Empty" };
            Original = new PrimaryKeyModel { Name = "Empty" };
            New = new PrimaryKeyModel { Name = "Empty" };
        }

        /// <inheritdoc/>
        public IKey Source { get; }

        /// <inheritdoc/>
        public IKey Target { get; }

        /// <inheritdoc/>
        public IKey Original { get; }

        /// <inheritdoc/>
        public IKey New { get; }

        /// <inheritdoc/>
        public bool NameChanged { get; private set; }

        /// <inheritdoc/>
        public bool TypeChanged { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> AddedColumns => _addedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> RemovedColumns => _removedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> ReorderedColumns => _reorderedColumns.AsReadOnly();

        /// <inheritdoc/>
        public bool HasChanged
        {
            get
            {
                if (!_hasChanged.HasValue)
                {
                    _hasChanged = NameChanged || 
                                 TypeChanged || 
                                 AddedColumns.Count > 0 || 
                                 RemovedColumns.Count > 0 || 
                                 ReorderedColumns.Count > 0;
                }

                return _hasChanged.Value;
            }
        }

        private void CalculateDifferences()
        {
            // If either key is null, the key was added or removed
            if (Source == null || Target == null)
            {
                // Mark everything as changed
                NameChanged = true;
                TypeChanged = true;

                if (Source != null)
                {
                    // Key was removed - all columns are removed
                    foreach (var column in Source.Columns)
                    {
                        _removedColumns.Add(column);
                    }
                }

                if (Target != null)
                {
                    // Key was added - all columns are added
                    foreach (var column in Target.Columns)
                    {
                        _addedColumns.Add(column);
                    }
                }

                return;
            }

            // Check if name changed
            NameChanged = !string.Equals(Source.Name, Target.Name, StringComparison.OrdinalIgnoreCase);

            // Check if the type changed (e.g., from PK to unique constraint)
            TypeChanged = Source.GetType() != Target.GetType();

            // Check for column changes
            CompareColumns();
        }

        private void CompareColumns()
        {
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

            // Check for column order changes
            if (_addedColumns.Count == 0 && _removedColumns.Count == 0)
            {
                // Only check order if columns are the same
                var sourceColumnList = Source.Columns.ToList();
                var targetColumnList = Target.Columns.ToList();

                if (sourceColumnList.Count == targetColumnList.Count)
                {
                    for (int i = 0; i < sourceColumnList.Count; i++)
                    {
                        if (!string.Equals(sourceColumnList[i].Name, targetColumnList[i].Name, StringComparison.OrdinalIgnoreCase))
                        {
                            // Order changed
                            foreach (var column in sourceColumnList)
                            {
                                _reorderedColumns.Add(column);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
