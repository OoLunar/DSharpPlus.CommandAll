using System;
using DSharpPlus.AsyncEvents;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.EventArgs
{
    /// <summary>
    /// Represents an event that is fired when a command is executed.
    /// </summary>
    public sealed class CommandExecutedEventArgs : AsyncEventArgs
    {
        /// <summary>
        /// The context of a command that was executed.
        /// </summary>
        public readonly CommandContext Context;

        /// <summary>
        /// Creates a new instance of <see cref="CommandExecutedEventArgs"/>.
        /// </summary>
        /// <param name="context">The context of a command that was executed.</param>
        public CommandExecutedEventArgs(CommandContext context) => Context = context ?? throw new ArgumentNullException(nameof(context));
    }
}
