namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines the actions that can be taken when a referenced entity is updated or deleted.
    /// </summary>
    public enum ReferentialAction
    {
        /// <summary>
        /// No action is taken.
        /// </summary>
        NoAction,

        /// <summary>
        /// The dependent entity is also deleted when the principal entity is deleted (cascade delete).
        /// </summary>
        Cascade,

        /// <summary>
        /// The values of foreign key properties in the dependent entity are set to null when the principal entity is deleted.
        /// </summary>
        SetNull,

        /// <summary>
        /// The values of foreign key properties in the dependent entity are set to their default values when the principal entity is deleted.
        /// </summary>
        SetDefault,

        /// <summary>
        /// If any dependent entities exist, the delete operation is rejected.
        /// </summary>
        Restrict
    }
}
