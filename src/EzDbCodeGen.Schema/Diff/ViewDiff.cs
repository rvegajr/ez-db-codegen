using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two database views.
    /// </summary>
    public class ViewDiff : IViewDiff
    {
        private readonly List<IViewColumn> _addedColumns = new();
        private readonly List<IViewColumn> _removedColumns = new();
        private bool? _hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewDiff"/> class.
        /// </summary>
        /// <param name="original">The original view.</param>
        /// <param name="newView">The new view.</param>
        /// <exception cref="ArgumentException">Thrown when the views have different names or schemas.</exception>
        public ViewDiff(IView? original, IView? newView)
        {
            if (original == null && newView == null)
            {
                throw new ArgumentNullException(nameof(original), "Both views cannot be null.");
            }

            // Allow null for one of the views (represents added or removed)
            if (original != null && newView != null)
            {
                if (!string.Equals(original.Name, newView.Name, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(original.Schema, newView.Schema, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Cannot compare views with different names or schemas.");
                }
            }

            Original = original;
            New = newView;
            
            // Calculate differences
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public IView? Original { get; }

        /// <inheritdoc/>
        public IView? New { get; }

        /// <inheritdoc/>
        public bool HasChanged
        {
            get
            {
                if (!_hasChanged.HasValue)
                {
                    _hasChanged = AddedColumns.Count > 0 ||
                                  RemovedColumns.Count > 0 ||
                                  DefinitionChanged ||
                                  IsIndexedChanged;
                }

                return _hasChanged.Value;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IViewColumn> AddedColumns => _addedColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IViewColumn> RemovedColumns => _removedColumns.AsReadOnly();

        /// <inheritdoc/>
        public bool DefinitionChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsIndexedChanged { get; private set; }

        private void CalculateDifferences()
        {
            // If either view is null, the whole view was added or removed
            if (Original == null || New == null)
            {
                DefinitionChanged = true;
                IsIndexedChanged = true;

                if (Original != null)
                {
                    // View was removed - all columns are removed
                    foreach (var column in Original.Columns)
                    {
                        _removedColumns.Add(column);
                    }
                }

                if (New != null)
                {
                    // View was added - all columns are added
                    foreach (var column in New.Columns)
                    {
                        _addedColumns.Add(column);
                    }
                }

                return;
            }

            // Check if the view definition has changed
            DefinitionChanged = !string.Equals(Original.Definition?.Trim(), New.Definition?.Trim(), StringComparison.OrdinalIgnoreCase);

            // Check if the indexed property has changed
            IsIndexedChanged = Original.IsIndexed != New.IsIndexed;

            // Check for added or removed columns
            var originalColumnsByName = Original.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            var newColumnsByName = New.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            // Identify removed columns (in original but not in new)
            foreach (var originalColumn in Original.Columns)
            {
                if (!newColumnsByName.ContainsKey(originalColumn.Name))
                {
                    _removedColumns.Add(originalColumn);
                }
            }

            // Identify added columns (in new but not in original)
            foreach (var newColumn in New.Columns)
            {
                if (!originalColumnsByName.ContainsKey(newColumn.Name))
                {
                    _addedColumns.Add(newColumn);
                }
            }

            // Check for changed columns (not part of the interface, but could be added)
            // For columns with same name, check if data type, nullability, etc. has changed
            foreach (var newColumn in New.Columns)
            {
                if (originalColumnsByName.TryGetValue(newColumn.Name, out var originalColumn))
                {
                    // If any property changed, mark the definition as changed
                    if (originalColumn.DataType != newColumn.DataType ||
                        originalColumn.IsNullable != newColumn.IsNullable ||
                        originalColumn.MaxLength != newColumn.MaxLength ||
                        originalColumn.Precision != newColumn.Precision ||
                        originalColumn.Scale != newColumn.Scale ||
                        originalColumn.OrdinalPosition != newColumn.OrdinalPosition)
                    {
                        DefinitionChanged = true;
                    }
                }
            }
        }
    }
}
