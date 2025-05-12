namespace EzDbCodeGen.Interfaces.TypeMapping
{
    /// <summary>
    /// Defines a factory for creating data type mappers.
    /// </summary>
    public interface IDataTypeMapFactory
    {
        /// <summary>
        /// Gets a data type mapper for the specified database provider and programming language.
        /// </summary>
        /// <param name="provider">The database provider.</param>
        /// <param name="language">The programming language.</param>
        /// <returns>A data type mapper for the specified database provider and programming language.</returns>
        IDataTypeMapper GetMapper(DatabaseProvider provider, ProgrammingLanguage language);

        /// <summary>
        /// Registers a data type mapper with the factory.
        /// </summary>
        /// <param name="mapper">The data type mapper to register.</param>
        void RegisterMapper(IDataTypeMapper mapper);

        /// <summary>
        /// Gets all registered mappers.
        /// </summary>
        /// <returns>A collection of all registered data type mappers.</returns>
        IReadOnlyCollection<IDataTypeMapper> GetAllMappers();
    }
}
