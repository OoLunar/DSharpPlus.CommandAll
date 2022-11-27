using System;
using System.Linq;

namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when a property has an invalid type.
    /// </summary>
    public sealed class InvalidPropertyTypeException : CommandAllException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidPropertyTypeException"/>.
        /// </summary>
        /// <param name="propertyName">The problem property.</param>
        /// <param name="actualType">The actual type.</param>
        /// <param name="expectedType">The expected type.</param>
        internal InvalidPropertyTypeException(string propertyName, Type actualType, Type expectedType) : this(propertyName, actualType, new[] { expectedType }) { }

        /// <inheritdoc cref="InvalidPropertyTypeException(string, Type, Type)"/>
        /// <param name="expectedTypes">The expected types.</param>
        internal InvalidPropertyTypeException(string propertyName, Type actualType, params Type[] expectedTypes) : base($"Property '{propertyName}' was expected to have a type of {string.Join(", ", expectedTypes.Select(type => type.Name))}, instead {actualType} was found. The actual type must be assignable to/inherit from the expected type.") { }
    }
}
