using System;

namespace DSharpPlus.CommandAll.Commands.Enums
{
    /// <summary>
    /// The strategy used to determine which conditions should a command be executed.
    /// </summary>
    [Flags]
    public enum CommandFilteringStrategy
    {
        /// <summary>
        /// Commands should not be executed under any circumstances.
        /// </summary>
        AcceptNone = 0,

        /// <summary>
        /// Commands should be executed in DMs.
        /// </summary>
        AcceptDMs = 1 << 1,

        /// <summary>
        /// Commands should be executed in guilds.
        /// </summary>
        AcceptGuilds = 1 << 2,

        /// <summary>
        /// Text commands should be executed.
        /// </summary>
        AcceptTextCommands = 1 << 3,

        /// <summary>
        /// Slash commands should be executed.
        /// </summary>
        AcceptSlashCommands = 1 << 4,

        /// <summary>
        /// Commands should be executed anywhere.
        /// </summary>
        AcceptAnywhere = AcceptDMs | AcceptGuilds,

        /// <summary>
        /// All types of commands should be executed.
        /// </summary>
        AcceptAllCommands = AcceptTextCommands | AcceptSlashCommands,

        /// <summary>
        /// If the command is found, it should be executed.
        /// </summary>
        AcceptAll = AcceptAnywhere | AcceptAllCommands
    }
}
