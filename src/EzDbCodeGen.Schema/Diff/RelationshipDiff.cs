using System;
using System.Text;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two table relationships.
    /// </summary>
    public class RelationshipDiff : IRelationshipDiff
    {
        private bool? _hasChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipDiff"/> class.
        /// </summary>
        /// <param name="source">The source relationship.</param>
        /// <param name="target">The target relationship.</param>
        public RelationshipDiff(IRelationship? source, IRelationship? target)
        {
            if (source == null && target == null)
            {
                throw new ArgumentNullException(nameof(source), "Both relationships cannot be null.");
            }

            Source = source;
            Target = target;
            
            // Implement interface properties
            Original = source;
            New = target;
            
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public IRelationship? Source { get; }

        /// <inheritdoc/>
        public IRelationship? Target { get; }

        /// <inheritdoc/>
        public IRelationship? Original { get; }

        /// <inheritdoc/>
        public IRelationship? New { get; }

        /// <inheritdoc/>
        public bool TypeChanged { get; private set; }

        /// <inheritdoc/>
        public bool SourceTableChanged { get; private set; }

        /// <inheritdoc/>
        public bool TargetTableChanged { get; private set; }

        /// <inheritdoc/>
        public bool SourcePropertyChanged { get; private set; }

        /// <inheritdoc/>
        public bool TargetPropertyChanged { get; private set; }

        /// <inheritdoc/>
        public bool JunctionTableChanged { get; private set; }

        /// <inheritdoc/>
        public bool SourceNavigationPropertyNameChanged { get; private set; }

        /// <inheritdoc/>
        public bool TargetNavigationPropertyNameChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsSourceCollectionChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsTargetCollectionChanged { get; private set; }

        /// <inheritdoc/>
        public bool DeleteBehaviorChanged { get; private set; }

        /// <inheritdoc/>
        public bool InheritanceStatusChanged { get; private set; }

        /// <inheritdoc/>
        public bool InheritanceTypeChanged { get; private set; }

        /// <inheritdoc/>
        public bool DiscriminatorColumnChanged { get; private set; }

        /// <inheritdoc/>
        public bool DiscriminatorValueChanged { get; private set; }

        /// <inheritdoc/>
        public bool HasChanged
        {
            get
            {
                if (!_hasChanged.HasValue)
                {
                    _hasChanged = TypeChanged ||
                                 SourceTableChanged ||
                                 TargetTableChanged ||
                                 SourcePropertyChanged ||
                                 TargetPropertyChanged ||
                                 JunctionTableChanged ||
                                 SourceNavigationPropertyNameChanged ||
                                 TargetNavigationPropertyNameChanged ||
                                 IsSourceCollectionChanged ||
                                 IsTargetCollectionChanged ||
                                 DeleteBehaviorChanged ||
                                 InheritanceStatusChanged ||
                                 InheritanceTypeChanged ||
                                 DiscriminatorColumnChanged ||
                                 DiscriminatorValueChanged;
                }

                return _hasChanged.Value;
            }
        }

        /// <inheritdoc/>
        public string GetSummary()
        {
            if (!HasChanged)
            {
                return $"No differences found for relationship between {Source?.SourceTable?.Name ?? "unknown"} and {Source?.TargetTable?.Name ?? "unknown"}";
            }

            var summary = new StringBuilder();
            
            if (Source != null && Target != null)
            {
                summary.AppendLine($"Relationship: {Source.SourceTable?.Name ?? "unknown"} -> {Source.TargetTable?.Name ?? "unknown"}");
                
                if (TypeChanged)
                {
                    summary.AppendLine($"- Type changed: {Source.Type} -> {Target.Type}");
                }
                
                if (SourceTableChanged)
                {
                    summary.AppendLine($"- Source table changed: {Source.SourceTable?.Name ?? "unknown"} -> {Target.SourceTable?.Name ?? "unknown"}");
                }
                
                if (TargetTableChanged)
                {
                    summary.AppendLine($"- Target table changed: {Source.TargetTable?.Name ?? "unknown"} -> {Target.TargetTable?.Name ?? "unknown"}");
                }
                
                if (SourcePropertyChanged || SourceNavigationPropertyNameChanged)
                {
                    summary.AppendLine($"- Source navigation property name changed: {Source.SourceNavigationPropertyName} -> {Target.SourceNavigationPropertyName}");
                }
                
                if (TargetPropertyChanged || TargetNavigationPropertyNameChanged)
                {
                    summary.AppendLine($"- Target navigation property name changed: {Source.TargetNavigationPropertyName} -> {Target.TargetNavigationPropertyName}");
                }
                
                if (JunctionTableChanged)
                {
                    summary.AppendLine($"- Junction table changed: {Source.JoinTable?.Name ?? "none"} -> {Target.JoinTable?.Name ?? "none"}");
                }
                
                if (IsSourceCollectionChanged || IsTargetCollectionChanged)
                {
                    summary.AppendLine("- Collection status changed");
                }
                
                if (DeleteBehaviorChanged)
                {
                    summary.AppendLine($"- Delete behavior changed: {Source.DeleteBehavior} -> {Target.DeleteBehavior}");
                }
                
                if (InheritanceStatusChanged)
                {
                    summary.AppendLine($"- Inheritance status changed: {Source.IsInheritance} -> {Target.IsInheritance}");
                }
                
                if (InheritanceTypeChanged)
                {
                    summary.AppendLine($"- Inheritance type changed: {Source.InheritanceType} -> {Target.InheritanceType}");
                }
            }
            else
            {
                // One of the relationships is null, so it was added or removed
                summary.AppendLine(Source == null 
                    ? $"Relationship added: {Target?.SourceTable?.Name ?? "unknown"} -> {Target?.TargetTable?.Name ?? "unknown"}"
                    : $"Relationship removed: {Source.SourceTable?.Name ?? "unknown"} -> {Source.TargetTable?.Name ?? "unknown"}");
            }
            
            return summary.ToString();
        }

        private void CalculateDifferences()
        {
            // If either relationship is null, the relationship was added or removed
            if (Source == null || Target == null)
            {
                // Mark everything as changed
                TypeChanged = true;
                SourceTableChanged = true;
                TargetTableChanged = true;
                SourcePropertyChanged = true;
                TargetPropertyChanged = true;
                JunctionTableChanged = true;
                SourceNavigationPropertyNameChanged = true;
                TargetNavigationPropertyNameChanged = true;
                IsSourceCollectionChanged = true;
                IsTargetCollectionChanged = true;
                DeleteBehaviorChanged = true;
                InheritanceStatusChanged = true;
                InheritanceTypeChanged = true;
                DiscriminatorColumnChanged = true;
                DiscriminatorValueChanged = true;
                return;
            }

            // Check if relationship type changed
            TypeChanged = Source.Type != Target.Type;

            // Check if source or target tables changed
            SourceTableChanged = Source.SourceTable?.Name != Target.SourceTable?.Name;
            TargetTableChanged = Source.TargetTable?.Name != Target.TargetTable?.Name;

            // Check if navigation property names changed
            SourceNavigationPropertyNameChanged = !string.Equals(Source.SourceNavigationPropertyName, Target.SourceNavigationPropertyName, StringComparison.OrdinalIgnoreCase);
            TargetNavigationPropertyNameChanged = !string.Equals(Source.TargetNavigationPropertyName, Target.TargetNavigationPropertyName, StringComparison.OrdinalIgnoreCase);

            // Map to interface properties
            SourcePropertyChanged = SourceNavigationPropertyNameChanged;
            TargetPropertyChanged = TargetNavigationPropertyNameChanged;

            // Check if junction table changed
            JunctionTableChanged = Source.JoinTable?.Name != Target.JoinTable?.Name;

            // Check if collection status changed
            IsSourceCollectionChanged = Source.IsSourceCollection != Target.IsSourceCollection;
            IsTargetCollectionChanged = Source.IsTargetCollection != Target.IsTargetCollection;

            // Check if delete behavior changed
            DeleteBehaviorChanged = Source.DeleteBehavior != Target.DeleteBehavior;

            // Check if inheritance status changed
            InheritanceStatusChanged = Source.IsInheritance != Target.IsInheritance;

            // Check if inheritance type changed
            InheritanceTypeChanged = Source.InheritanceType != Target.InheritanceType;

            // Check if discriminator column changed
            DiscriminatorColumnChanged = Source.DiscriminatorColumn?.Name != Target.DiscriminatorColumn?.Name;

            // Check if discriminator value changed
            DiscriminatorValueChanged = !string.Equals(Source.DiscriminatorValue, Target.DiscriminatorValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
