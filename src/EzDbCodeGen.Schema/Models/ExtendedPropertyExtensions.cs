using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Extension methods for managing extended properties on database objects.
    /// </summary>
    public static class ExtendedPropertyExtensions
    {
        private static readonly Dictionary<object, Dictionary<string, string>> _extendedProperties = new();

        /// <summary>
        /// Sets an extended property on a table.
        /// </summary>
        /// <param name="table">The table to set the property on.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        public static void SetExtendedProperty(this Table table, string propertyName, string propertyValue)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));
            }

            SetExtendedPropertyInternal(table, propertyName, propertyValue);
        }

        /// <summary>
        /// Sets an extended property on a column.
        /// </summary>
        /// <param name="column">The column to set the property on.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        public static void SetExtendedProperty(this Column column, string propertyName, string propertyValue)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));
            }

            SetExtendedPropertyInternal(column, propertyName, propertyValue);
        }

        /// <summary>
        /// Gets an extended property from a database object.
        /// </summary>
        /// <param name="obj">The database object.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The property value, or null if not found.</returns>
        public static string GetExtendedProperty(this object obj, string propertyName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));
            }

            if (_extendedProperties.TryGetValue(obj, out var properties) && 
                properties.TryGetValue(propertyName, out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Gets all extended properties for a database object.
        /// </summary>
        /// <param name="obj">The database object.</param>
        /// <returns>A dictionary of property names and values.</returns>
        public static Dictionary<string, string> GetExtendedProperties(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (_extendedProperties.TryGetValue(obj, out var properties))
            {
                return new Dictionary<string, string>(properties);
            }

            return new Dictionary<string, string>();
        }

        private static void SetExtendedPropertyInternal(object obj, string propertyName, string propertyValue)
        {
            if (!_extendedProperties.TryGetValue(obj, out var properties))
            {
                properties = new Dictionary<string, string>();
                _extendedProperties[obj] = properties;
            }

            properties[propertyName] = propertyValue;
        }
    }
}
