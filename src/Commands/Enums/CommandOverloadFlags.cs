using System;

namespace DSharpPlus.CommandAll.Commands.Enums
{
    /// <summary>
    /// Flags for command overloads.
    /// </summary>
    [Flags]
    public enum CommandOverloadFlags
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// The command overload is disabled.
        /// </summary>
        Disabled = 1 << 1,

        /// <summary>
        /// This overload is the default overload and should be registered as a slash command.
        /// </summary>
        SlashPreferred = 1 << 2,
    }
}
