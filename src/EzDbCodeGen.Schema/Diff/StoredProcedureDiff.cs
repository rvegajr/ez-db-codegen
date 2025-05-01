using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two stored procedures.
    /// </summary>
    public class StoredProcedureDiff : IStoredProcedureDiff
    {
        private readonly List<IParameter> _addedParameters = new();
        private readonly List<IParameter> _removedParameters = new();
        private readonly List<IParameter> _changedParameters = new();
        private readonly List<IResultColumn> _addedResultColumns = new();
        private readonly List<IResultColumn> _removedResultColumns = new();
        private readonly List<IResultColumn> _changedResultColumns = new();
        private bool? _hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedureDiff"/> class.
        /// </summary>
        /// <param name="original">The original stored procedure.</param>
        /// <param name="newProcedure">The new stored procedure.</param>
        /// <exception cref="ArgumentException">Thrown when the stored procedures have different names or schemas.</exception>
        public StoredProcedureDiff(IStoredProcedure? original, IStoredProcedure? newProcedure)
        {
            if (original == null && newProcedure == null)
            {
                throw new ArgumentNullException(nameof(original), "Both stored procedures cannot be null.");
            }

            // Allow null for one of the stored procedures (represents added or removed)
            if (original != null && newProcedure != null)
            {
                if (!string.Equals(original.Name, newProcedure.Name, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(original.Schema, newProcedure.Schema, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Cannot compare stored procedures with different names or schemas.");
                }
            }

            // Create empty stored procedure for null values
            var emptyProc = new StoredProcedure("Empty", "dbo");
            Original = original ?? emptyProc;
            New = newProcedure ?? emptyProc;
            
            // Calculate differences
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public IStoredProcedure Original { get; }

        /// <inheritdoc/>
        public IStoredProcedure New { get; }

        /// <inheritdoc/>
        public bool HasChanged
        {
            get
            {
                if (!_hasChanged.HasValue)
                {
                    _hasChanged = AddedParameters.Count > 0 ||
                                  RemovedParameters.Count > 0 ||
                                  ChangedParameters.Count > 0 ||
                                  AddedResultColumns.Count > 0 ||
                                  RemovedResultColumns.Count > 0 ||
                                  ChangedResultColumns.Count > 0 ||
                                  DefinitionChanged;
                }

                return _hasChanged.Value;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IParameter> AddedParameters => _addedParameters.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IParameter> RemovedParameters => _removedParameters.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IParameter> ChangedParameters => _changedParameters.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IResultColumn> AddedResultColumns => _addedResultColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IResultColumn> RemovedResultColumns => _removedResultColumns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IResultColumn> ChangedResultColumns => _changedResultColumns.AsReadOnly();

        /// <inheritdoc/>
        public bool DefinitionChanged { get; private set; }

        private void CalculateDifferences()
        {
            // If either stored procedure is null, the whole stored procedure was added or removed
            if (Original == null || New == null)
            {
                DefinitionChanged = true;

                if (Original != null)
                {
                    // Stored procedure was removed
                    foreach (var parameter in Original.Parameters)
                    {
                        _removedParameters.Add(parameter);
                    }

                    if (Original.ResultColumns != null)
                    {
                        foreach (var resultColumn in Original.ResultColumns)
                        {
                            _removedResultColumns.Add(resultColumn);
                        }
                    }
                }

                if (New != null)
                {
                    // Stored procedure was added
                    foreach (var parameter in New.Parameters)
                    {
                        _addedParameters.Add(parameter);
                    }

                    if (New.ResultColumns != null)
                    {
                        foreach (var resultColumn in New.ResultColumns)
                        {
                            _addedResultColumns.Add(resultColumn);
                        }
                    }
                }

                return;
            }

            // Check if the definition has changed
            DefinitionChanged = !string.Equals(Original.Definition?.Trim(), New.Definition?.Trim(), StringComparison.OrdinalIgnoreCase);

            // Check for added, removed, or changed parameters
            var originalParametersByName = Original.Parameters.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            var newParametersByName = New.Parameters.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            // Identify removed parameters
            foreach (var originalParameter in Original.Parameters)
            {
                if (!newParametersByName.ContainsKey(originalParameter.Name))
                {
                    _removedParameters.Add(originalParameter);
                }
            }

            // Identify added parameters
            foreach (var newParameter in New.Parameters)
            {
                if (!originalParametersByName.ContainsKey(newParameter.Name))
                {
                    _addedParameters.Add(newParameter);
                }
            }

            // Identify changed parameters
            foreach (var newParameter in New.Parameters)
            {
                if (originalParametersByName.TryGetValue(newParameter.Name, out var originalParameter))
                {
                    if (originalParameter.DataType != newParameter.DataType ||
                        originalParameter.IsNullable != newParameter.IsNullable ||
                        originalParameter.MaxLength != newParameter.MaxLength ||
                        originalParameter.Precision != newParameter.Precision ||
                        originalParameter.Scale != newParameter.Scale ||
                        originalParameter.Direction != newParameter.Direction ||
                        originalParameter.OrdinalPosition != newParameter.OrdinalPosition)
                    {
                        _changedParameters.Add(newParameter);
                    }
                }
            }

            // Check for result columns (if either has none, treat as a change)
            if ((Original.ResultColumns == null && New.ResultColumns != null) ||
                (Original.ResultColumns != null && New.ResultColumns == null))
            {
                DefinitionChanged = true;

                if (Original.ResultColumns != null)
                {
                    foreach (var resultColumn in Original.ResultColumns)
                    {
                        _removedResultColumns.Add(resultColumn);
                    }
                }

                if (New.ResultColumns != null)
                {
                    foreach (var resultColumn in New.ResultColumns)
                    {
                        _addedResultColumns.Add(resultColumn);
                    }
                }
            }
            else if (Original.ResultColumns != null && New.ResultColumns != null)
            {
                // Check for added, removed, or changed result columns
                var originalResultColumnsByName = Original.ResultColumns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
                var newResultColumnsByName = New.ResultColumns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

                // Identify removed result columns
                foreach (var originalResultColumn in Original.ResultColumns)
                {
                    if (!newResultColumnsByName.ContainsKey(originalResultColumn.Name))
                    {
                        _removedResultColumns.Add(originalResultColumn);
                    }
                }

                // Identify added result columns
                foreach (var newResultColumn in New.ResultColumns)
                {
                    if (!originalResultColumnsByName.ContainsKey(newResultColumn.Name))
                    {
                        _addedResultColumns.Add(newResultColumn);
                    }
                }

                // Identify changed result columns
                foreach (var newResultColumn in New.ResultColumns)
                {
                    if (originalResultColumnsByName.TryGetValue(newResultColumn.Name, out var originalResultColumn))
                    {
                        if (originalResultColumn.DataType != newResultColumn.DataType ||
                            originalResultColumn.IsNullable != newResultColumn.IsNullable ||
                            originalResultColumn.MaxLength != newResultColumn.MaxLength ||
                            originalResultColumn.Precision != newResultColumn.Precision ||
                            originalResultColumn.Scale != newResultColumn.Scale ||
                            originalResultColumn.OrdinalPosition != newResultColumn.OrdinalPosition)
                        {
                            _changedResultColumns.Add(newResultColumn);
                        }
                    }
                }
            }
        }
    }
}
