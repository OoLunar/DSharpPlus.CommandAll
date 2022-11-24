using System;

namespace OoLunar.DSharpPlus.CommandAll.Exceptions
{
    public sealed class PropertyNullException : CommandAllException
    {
        /// <inheritdoc cref="ArgumentNullException(string?)"/>
        internal PropertyNullException(string propertyName) : base($"Property '{propertyName}' is null or whitespace.") { }
    }
}
