using System;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Exceptions
{
    /// <summary>
    /// Thrown when a command is disabled but is attempted to be executed.
    /// </summary>
    public sealed class CommandDisabledException : CommandAllException
    {
        /// <summary>
        /// The command that was disabled.
        /// </summary>
        public Command Command { get; init; }

        /// <summary>
        /// Creates a new instance of <see cref="CommandDisabledException"/>.
        /// </summary>
        public CommandDisabledException(Command command) : base($"Command {command.FullName} is disabled.") => Command = command ?? throw new ArgumentNullException(nameof(command));
    }
}
