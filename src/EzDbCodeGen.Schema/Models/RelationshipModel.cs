using System.Collections.Generic;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a relationship between two tables in a database schema.
    /// </summary>
    public class RelationshipModel : IRelationship
    {
        /// <summary>
        /// Gets or sets the name of the relationship.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the source table of the relationship.
        /// </summary>
        public ITable SourceTable { get; set; } = null!;

        /// <summary>
        /// Gets the target table of the relationship.
        /// </summary>
        public ITable TargetTable { get; set; } = null!;

        /// <summary>
        /// Gets the type of the relationship.
        /// </summary>
        public EzDbCodeGen.Schema.Interfaces.RelationshipType Type { get; set; }

        /// <summary>
        /// Gets the foreign key that defines the relationship.
        /// </summary>
        public IForeignKey? ForeignKey { get; set; }

        /// <summary>
        /// Gets the join table for many-to-many relationships.
        /// </summary>
        public ITable? JoinTable { get; set; }

        /// <summary>
        /// Gets the source navigation property name.
        /// </summary>
        public string SourceNavigationPropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets the target navigation property name.
        /// </summary>
        public string TargetNavigationPropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the source navigation property is a collection.
        /// </summary>
        public bool IsSourceCollection { get; set; }

        /// <summary>
        /// Gets a value indicating whether the target navigation property is a collection.
        /// </summary>
        public bool IsTargetCollection { get; set; }

        /// <summary>
        /// Gets the cascade delete behavior for this relationship.
        /// </summary>
        public ReferentialAction DeleteBehavior { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is a self-referencing relationship.
        /// </summary>
        public bool IsSelfReferencing { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is an inheritance relationship.
        /// </summary>
        public bool IsInheritance { get; set; }

        /// <summary>
        /// Gets the inheritance type for inheritance relationships.
        /// </summary>
        public InheritanceType? InheritanceType { get; set; }

        /// <summary>
        /// Gets the discriminator column for TPH inheritance relationships.
        /// </summary>
        public IColumn? DiscriminatorColumn { get; set; }

        /// <summary>
        /// Gets the discriminator value for the derived table in TPH inheritance relationships.
        /// </summary>
        public string? DiscriminatorValue { get; set; }

        // Legacy properties - kept for backward compatibility
        
        /// <summary>
        /// Gets or sets the parent table in the relationship.
        /// This is an alias for SourceTable.
        /// </summary>
        public ITable ParentTable 
        { 
            get => SourceTable; 
            set => SourceTable = value; 
        }

        /// <summary>
        /// Gets or sets the child table in the relationship.
        /// This is an alias for TargetTable.
        /// </summary>
        public ITable ChildTable 
        { 
            get => TargetTable; 
            set => TargetTable = value; 
        }

        /// <summary>
        /// Gets or sets the list of parent columns in the relationship.
        /// </summary>
        public IList<IColumn> ParentColumns { get; set; } = new List<IColumn>();

        /// <summary>
        /// Gets or sets the list of child columns in the relationship.
        /// </summary>
        public IList<IColumn> ChildColumns { get; set; } = new List<IColumn>();

        /// <summary>
        /// Gets or sets a value indicating whether this is a many-to-many relationship.
        /// </summary>
        public bool IsManyToMany 
        { 
            get => Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.ManyToMany; 
            set { if (value) Type = EzDbCodeGen.Schema.Interfaces.RelationshipType.ManyToMany; } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a one-to-one relationship.
        /// </summary>
        public bool IsOneToOne 
        { 
            get => Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToOne; 
            set { if (value) Type = EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToOne; } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a one-to-many relationship.
        /// </summary>
        public bool IsOneToMany 
        { 
            get => Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToMany; 
            set { if (value) Type = EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToMany; } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a many-to-one relationship.
        /// </summary>
        public bool IsManyToOne 
        { 
            get => Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.ManyToOne; 
            set { if (value) Type = EzDbCodeGen.Schema.Interfaces.RelationshipType.ManyToOne; } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether this relationship is required (no nulls allowed).
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this relationship enforces referential integrity.
        /// </summary>
        public bool EnforcesReferentialIntegrity { get; set; }

        /// <summary>
        /// Gets or sets the cascade delete rule.
        /// This is a legacy property. Use DeleteBehavior instead.
        /// </summary>
        public string CascadeDelete { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the cascade update rule.
        /// </summary>
        public string CascadeUpdate { get; set; } = string.Empty;
    }
}