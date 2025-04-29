using System;
using System.Collections.Generic;

namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a database schema model containing tables, views, and other schema elements.
/// </summary>
public interface ISchemaModel
{
    /// <summary>
    /// Gets the collection of tables in the schema.
    /// </summary>
    IList<ITable> Tables { get; }

    /// <summary>
    /// Gets the collection of views in the schema.
    /// </summary>
    IList<IView> Views { get; }

    /// <summary>
    /// Gets the collection of stored procedures in the schema.
    /// </summary>
    IList<IStoredProcedure> StoredProcedures { get; }

    /// <summary>
    /// Gets the collection of functions in the schema.
    /// </summary>
    IList<IFunction> Functions { get; }

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    string DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the version of the schema.
    /// </summary>
    string Version { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the schema was extracted.
    /// </summary>
    DateTime ExtractionTimestamp { get; set; }
    
    /// <summary>
    /// Gets or sets the provider that extracted this schema.
    /// </summary>
    string Provider { get; set; }
}
