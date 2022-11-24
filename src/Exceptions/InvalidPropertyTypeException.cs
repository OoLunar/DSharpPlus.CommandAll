using System;

namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    public sealed class InvalidPropertyTypeException : CommandAllException
    {
        /// <inheritdoc cref="Exception(string?)"/>
        internal InvalidPropertyTypeException(string parameterName, Type foundType, Type expectedType) : base($"Property '{parameterName}' was expected to have a type of {expectedType}, instead {foundType} was found. The actual type must be assignable to/inherit from the expected type.") { }
    }
}
