using System;

namespace DSharpPlus.CommandAll.Commands.Enums
{
    /// <summary>
    /// Flags for commands.
    /// </summary>
    [Flags]
    public enum CommandFlags
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None,

        /// <summary>
        /// The command is disabled.
        /// </summary>
        Disabled = 1 << 1,

        /// <summary>
        /// The command can be executed in DM's.
        /// </summary>
        AllowDirectMessages = 1 << 2
    }
}
