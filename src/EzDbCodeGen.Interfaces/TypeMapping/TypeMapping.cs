namespace EzDbCodeGen.Interfaces.TypeMapping
{
    /// <summary>
    /// Represents a mapping between a database type and a programming language type.
    /// </summary>
    public class TypeMapping
    {
        /// <summary>
        /// Gets or sets the database type name.
        /// </summary>
        public string DatabaseTypeName { get; set; }

        /// <summary>
        /// Gets or sets the programming language type name.
        /// </summary>
        public string LanguageTypeName { get; set; }

        /// <summary>
        /// Gets or sets the programming language for this mapping.
        /// </summary>
        public ProgrammingLanguage Language { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is a primitive type.
        /// </summary>
        public bool IsPrimitive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is a value type.
        /// </summary>
        public bool IsValueType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is a collection type.
        /// </summary>
        public bool IsCollection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is an enum type.
        /// </summary>
        public bool IsEnum { get; set; }

        /// <summary>
        /// Gets or sets the precision of the type (for numeric types).
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// Gets or sets the scale of the type (for numeric types).
        /// </summary>
        public int? Scale { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the type (for string types).
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the database provider for this mapping.
        /// </summary>
        public DatabaseProvider Provider { get; set; }

        /// <summary>
        /// Gets the fully qualified type name, including nullable annotation if appropriate.
        /// </summary>
        public string FullTypeName
        {
            get
            {
                if (Language == ProgrammingLanguage.CSharp)
                {
                    // C# nullable reference types syntax
                    if (IsNullable && !IsValueType)
                    {
                        return $"{LanguageTypeName}?";
                    }
                    // C# nullable value types syntax
                    else if (IsNullable && IsValueType)
                    {
                        return $"{LanguageTypeName}?";
                    }
                }
                else if (Language == ProgrammingLanguage.TypeScript)
                {
                    // TypeScript nullable syntax
                    if (IsNullable)
                    {
                        return $"{LanguageTypeName} | null";
                    }
                }

                return LanguageTypeName;
            }
        }

        /// <summary>
        /// Gets the default value for this type.
        /// </summary>
        public string DefaultValue
        {
            get
            {
                if (Language == ProgrammingLanguage.CSharp)
                {
                    if (IsNullable)
                    {
                        return "null";
                    }
                    else if (LanguageTypeName == "string")
                    {
                        return "string.Empty";
                    }
                    else if (LanguageTypeName == "bool")
                    {
                        return "false";
                    }
                    else if (LanguageTypeName == "DateTime")
                    {
                        return "DateTime.MinValue";
                    }
                    else if (LanguageTypeName == "Guid")
                    {
                        return "Guid.Empty";
                    }
                    else if (IsCollection)
                    {
                        return "new()";
                    }
                    else if (IsValueType)
                    {
                        return "default";
                    }
                    else
                    {
                        return "null";
                    }
                }
                else if (Language == ProgrammingLanguage.TypeScript)
                {
                    if (IsNullable)
                    {
                        return "null";
                    }
                    else if (LanguageTypeName == "string")
                    {
                        return "''";
                    }
                    else if (LanguageTypeName == "boolean")
                    {
                        return "false";
                    }
                    else if (LanguageTypeName == "Date")
                    {
                        return "new Date(0)";
                    }
                    else if (IsCollection)
                    {
                        return "[]";
                    }
                    else
                    {
                        return "null";
                    }
                }

                return "null";
            }
        }

        /// <summary>
        /// Creates a new instance of the TypeMapping class.
        /// </summary>
        public TypeMapping()
        {
        }

        /// <summary>
        /// Creates a new instance of the TypeMapping class.
        /// </summary>
        /// <param name="databaseTypeName">The database type name.</param>
        /// <param name="languageTypeName">The programming language type name.</param>
        /// <param name="language">The programming language.</param>
        /// <param name="provider">The database provider.</param>
        /// <param name="isNullable">Whether the type is nullable.</param>
        /// <param name="isValueType">Whether the type is a value type.</param>
        public TypeMapping(string databaseTypeName, string languageTypeName, ProgrammingLanguage language,
            DatabaseProvider provider, bool isNullable = false, bool isValueType = false)
        {
            DatabaseTypeName = databaseTypeName;
            LanguageTypeName = languageTypeName;
            Language = language;
            Provider = provider;
            IsNullable = isNullable;
            IsValueType = isValueType;
        }
    }
}
