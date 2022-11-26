using System;
using System.Linq;

namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    public sealed class InvalidPropertyTypeException : CommandAllException
    {
        internal InvalidPropertyTypeException(string parameterName, Type foundType, Type expectedType) : this(parameterName, foundType, new[] { expectedType }) { }
        internal InvalidPropertyTypeException(string parameterName, Type foundType, params Type[] expectedTypes) : base($"Property '{parameterName}' was expected to have a type of {string.Join(", ", expectedTypes.Select(type => type.Name))}, instead {foundType} was found. The actual type must be assignable to/inherit from the expected type.") { }
    }
}
