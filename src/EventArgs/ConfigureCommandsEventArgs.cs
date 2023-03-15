using System;
using DSharpPlus.AsyncEvents;
using DSharpPlus.CommandAll.Managers;

namespace DSharpPlus.CommandAll.EventArgs
{
    /// <summary>
    /// Represents an event that is fired when the commands need to be configured.
    /// </summary>
    public sealed class ConfigureCommandsEventArgs : AsyncEventArgs
    {
        /// <summary>
        /// The extension whose commands needs configuring.
        /// </summary>
        public CommandAllExtension Extension { get; init; }

        /// <summary>
        /// The command manager that holds the command builders.
        /// </summary>
        public ICommandManager CommandManager { get; init; }

        /// <summary>
        /// Creates a new instance of <see cref="ConfigureCommandsEventArgs"/>.
        /// </summary>
        /// <param name="extension">The extension whose commands needs configuring.</param>
        /// <param name="commandManager">The command manager that holds the command builders.</param>
        public ConfigureCommandsEventArgs(CommandAllExtension extension, ICommandManager commandManager)
        {
            Extension = extension ?? throw new ArgumentNullException(nameof(extension));
            CommandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
        }
    }
}
