using System;
using DSharpPlus.CommandAll.Commands.Builders;

namespace DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when a property has an invalid value.
    /// </summary>
    public sealed class InvalidCommandStateException : CommandAllException
    {
        /// <summary>
        /// Creates a new instance of <see cref="InvalidCommandStateException"/>.
        /// </summary>
        /// <param name="propertyName">The problem command.</param>
        /// <param name="message">A message detailing why the value on the property is invalid.</param>
        internal InvalidCommandStateException(CommandBuilder command, Exception innerException) : base($"Command '{command.FullName}' has an invalid state.", innerException) { }
    }
}
