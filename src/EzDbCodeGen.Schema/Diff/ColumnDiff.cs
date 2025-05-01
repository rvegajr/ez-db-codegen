using System;
using System.Text;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;

namespace EzDbCodeGen.Schema.Diff
{
    /// <summary>
    /// Implements a comparison between two database columns.
    /// </summary>
    public class ColumnDiff : IColumnDiff
    {
        private bool? _hasDifferences;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDiff"/> class.
        /// </summary>
        /// <param name="source">The source column.</param>
        /// <param name="target">The target column.</param>
        /// <exception cref="ArgumentNullException">Thrown when either source or target is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the columns have different names.</exception>
        public ColumnDiff(IColumn source, IColumn target)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            
            if (!string.Equals(source.Name, target.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Cannot compare columns with different names.");
            }
            
            CalculateDifferences();
        }

        /// <inheritdoc/>
        public IColumn Source { get; }

        /// <inheritdoc/>
        public IColumn Target { get; }

        /// <inheritdoc/>
        public bool NameChanged { get; private set; }

        /// <inheritdoc/>
        public bool DataTypeChanged { get; private set; }

        /// <inheritdoc/>
        public bool NullabilityChanged { get; private set; }

        /// <inheritdoc/>
        public bool DefaultValueChanged { get; private set; }

        /// <inheritdoc/>
        public bool ComputedExpressionChanged { get; private set; }

        /// <inheritdoc/>
        public bool IdentitySpecificationChanged { get; private set; }

        /// <inheritdoc/>
        public bool CollationChanged { get; private set; }

        /// <inheritdoc/>
        public bool PrecisionChanged { get; private set; }

        /// <inheritdoc/>
        public bool ScaleChanged { get; private set; }

        /// <inheritdoc/>
        public bool MaxLengthChanged { get; private set; }

        /// <inheritdoc/>
        public bool OrdinalPositionChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsPartOfPrimaryKeyChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsPartOfUniqueConstraintChanged { get; private set; }

        /// <inheritdoc/>
        public bool IsPartOfForeignKeyChanged { get; private set; }

        /// <inheritdoc/>
        public bool HasDifferences
        {
            get
            {
                if (!_hasDifferences.HasValue)
                {
                    _hasDifferences = NameChanged ||
                                     DataTypeChanged ||
                                     NullabilityChanged ||
                                     DefaultValueChanged ||
                                     ComputedExpressionChanged ||
                                     IdentitySpecificationChanged ||
                                     CollationChanged ||
                                     PrecisionChanged ||
                                     ScaleChanged ||
                                     MaxLengthChanged ||
                                     OrdinalPositionChanged ||
                                     IsPartOfPrimaryKeyChanged ||
                                     IsPartOfUniqueConstraintChanged ||
                                     IsPartOfForeignKeyChanged;
                }

                return _hasDifferences.Value;
            }
        }

        /// <inheritdoc/>
        public string GetSummary()
        {
            if (!HasDifferences)
            {
                return $"No differences found for column {Source.Name}.";
            }

            var builder = new StringBuilder();
            builder.AppendLine($"Column: {Source.Name}");
            
            if (DataTypeChanged)
            {
                builder.AppendLine($"- Data type changed: {Source.DataType} -> {Target.DataType}");
            }
            
            if (NullabilityChanged)
            {
                builder.AppendLine($"- Nullability changed: {(Source.IsNullable ? "NULL" : "NOT NULL")} -> {(Target.IsNullable ? "NULL" : "NOT NULL")}");
            }
            
            if (DefaultValueChanged)
            {
                builder.AppendLine($"- Default value changed: {Source.DefaultValue} -> {Target.DefaultValue}");
            }
            
            return builder.ToString();
        }

        /// <inheritdoc/>
        public string GetDetailedReport()
        {
            if (!HasDifferences)
            {
                return $"No differences found for column {Source.Name}.";
            }

            var builder = new StringBuilder();
            builder.AppendLine($"Column: {Source.Name}");
            
            if (DataTypeChanged)
            {
                builder.AppendLine($"- Data type changed: {Source.DataType} -> {Target.DataType}");
            }
            
            if (NullabilityChanged)
            {
                builder.AppendLine($"- Nullability changed: {(Source.IsNullable ? "NULL" : "NOT NULL")} -> {(Target.IsNullable ? "NULL" : "NOT NULL")}");
            }
            
            if (DefaultValueChanged)
            {
                builder.AppendLine($"- Default value changed: {Source.DefaultValue} -> {Target.DefaultValue}");
            }
            
            if (ComputedExpressionChanged)
            {
                builder.AppendLine("- Computed expression changed");
                builder.AppendLine($"  From: {Source.ComputedColumnExpression}");
                builder.AppendLine($"  To: {Target.ComputedColumnExpression}");
            }
            
            if (IdentitySpecificationChanged)
            {
                builder.AppendLine("- Identity specification changed");
                builder.AppendLine($"  From: {(Source.IsIdentity ? "IDENTITY" : "NOT IDENTITY")}");
                builder.AppendLine($"  To: {(Target.IsIdentity ? "IDENTITY" : "NOT IDENTITY")}");
            }
            
            if (CollationChanged)
            {
                builder.AppendLine($"- Collation changed: {Source.Collation} -> {Target.Collation}");
            }
            
            if (PrecisionChanged)
            {
                builder.AppendLine($"- Precision changed: {Source.Precision} -> {Target.Precision}");
            }
            
            if (ScaleChanged)
            {
                builder.AppendLine($"- Scale changed: {Source.Scale} -> {Target.Scale}");
            }
            
            if (MaxLengthChanged)
            {
                builder.AppendLine($"- Max length changed: {Source.MaxLength} -> {Target.MaxLength}");
            }
            
            if (OrdinalPositionChanged)
            {
                builder.AppendLine($"- Ordinal position changed: {Source.OrdinalPosition} -> {Target.OrdinalPosition}");
            }
            
            if (IsPartOfPrimaryKeyChanged)
            {
                builder.AppendLine($"- Primary key membership changed: {Source.IsPartOfPrimaryKey} -> {Target.IsPartOfPrimaryKey}");
            }
            
            if (IsPartOfUniqueConstraintChanged)
            {
                builder.AppendLine($"- Unique constraint membership changed: {Source.IsPartOfUniqueConstraint} -> {Target.IsPartOfUniqueConstraint}");
            }
            
            if (IsPartOfForeignKeyChanged)
            {
                builder.AppendLine($"- Foreign key membership changed: {Source.IsPartOfForeignKey} -> {Target.IsPartOfForeignKey}");
            }
            
            return builder.ToString();
        }

        private void CalculateDifferences()
        {
            // Names are already verified to be the same in the constructor, but case might be different
            NameChanged = !string.Equals(Source.Name, Target.Name, StringComparison.Ordinal);
            
            // Check for data type changes
            DataTypeChanged = !string.Equals(Source.DataType, Target.DataType, StringComparison.OrdinalIgnoreCase);
            
            // Check for nullability changes
            NullabilityChanged = Source.IsNullable != Target.IsNullable;
            
            // Check for default value changes
            DefaultValueChanged = !string.Equals(Source.DefaultValue, Target.DefaultValue, StringComparison.OrdinalIgnoreCase);
            
            // Check for computed expression changes
            ComputedExpressionChanged = !string.Equals(Source.ComputedColumnExpression, Target.ComputedColumnExpression, StringComparison.OrdinalIgnoreCase);
            
            // Check for identity specification changes
            IdentitySpecificationChanged = Source.IsIdentity != Target.IsIdentity;
            
            // Check for collation changes
            CollationChanged = !string.Equals(Source.Collation, Target.Collation, StringComparison.OrdinalIgnoreCase);
            
            // Check for precision changes
            PrecisionChanged = Source.Precision != Target.Precision;
            
            // Check for scale changes
            ScaleChanged = Source.Scale != Target.Scale;
            
            // Check for max length changes
            MaxLengthChanged = Source.MaxLength != Target.MaxLength;
            
            // Check for ordinal position changes
            OrdinalPositionChanged = Source.OrdinalPosition != Target.OrdinalPosition;
            
            // Check for primary key membership changes
            IsPartOfPrimaryKeyChanged = Source.IsPartOfPrimaryKey != Target.IsPartOfPrimaryKey;
            
            // Check for unique constraint membership changes
            IsPartOfUniqueConstraintChanged = Source.IsPartOfUniqueConstraint != Target.IsPartOfUniqueConstraint;
            
            // Check for foreign key membership changes
            IsPartOfForeignKeyChanged = Source.IsPartOfForeignKey != Target.IsPartOfForeignKey;
        }
    }
}
