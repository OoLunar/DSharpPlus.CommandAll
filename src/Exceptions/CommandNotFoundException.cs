using System;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when a command is not found.
    /// </summary>
    public sealed class CommandNotFoundException : CommandAllException
    {
        /// <summary>
        /// The full command that was attempted to be executed. Usually just <see cref="DiscordMessage.Content"/>.
        /// </summary>
        public readonly string CommandString;

        /// <summary>
        /// Creates a new <see cref="CommandNotFoundException"/>.
        /// </summary>
        internal CommandNotFoundException(string message, string commandString) : base($"{message}: {commandString}") => CommandString = commandString ?? throw new ArgumentNullException(nameof(commandString));
    }
}
