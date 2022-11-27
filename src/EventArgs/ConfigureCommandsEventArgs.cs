using System;
using Emzi0767.Utilities;
using OoLunar.DSharpPlus.CommandAll.Managers;

namespace OoLunar.DSharpPlus.CommandAll.EventArgs
{
    /// <summary>
    /// Represents an event that is fired when the commands need to be configured.
    /// </summary>
    public sealed class ConfigureCommandsEventArgs : AsyncEventArgs
    {
        /// <summary>
        /// The extension whose commands needs configuring.
        /// </summary>
        public readonly CommandAllExtension Extension;

        /// <summary>
        /// The command manager that holds the command builders.
        /// </summary>
        public readonly ICommandManager CommandManager;

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
