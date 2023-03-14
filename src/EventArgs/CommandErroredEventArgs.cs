using System;
using DSharpPlus.AsyncEvents;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.EventArgs
{
    /// <summary>
    /// Represents an event that is fired when a command errored.
    /// </summary>
    public sealed class CommandErroredEventArgs : AsyncEventArgs
    {
        /// <summary>
        /// The context of a command that errored.
        /// </summary>
        public readonly CommandContext Context;

        /// <summary>
        /// The exception that was thrown.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// Creates a new instance of <see cref="CommandErroredEventArgs"/>.
        /// </summary>
        /// <param name="context">The context of a command that errored.</param>
        /// <param name="exception">The exception that was thrown.</param>
        public CommandErroredEventArgs(CommandContext context, Exception exception)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }
}
