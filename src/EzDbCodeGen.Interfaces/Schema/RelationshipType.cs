namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines the types of relationships that can exist between database tables.
    /// </summary>
    public enum RelationshipType
    {
        /// <summary>
        /// A one-to-many relationship.
        /// </summary>
        OneToMany,

        /// <summary>
        /// A one-to-one relationship.
        /// </summary>
        OneToOne,

        /// <summary>
        /// A many-to-many relationship.
        /// </summary>
        ManyToMany,

        /// <summary>
        /// An inheritance relationship.
        /// </summary>
        Inheritance
    }
}
