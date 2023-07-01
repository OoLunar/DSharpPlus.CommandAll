using System;

namespace DSharpPlus.CommandAll.Commands.Enums
{
    /// <summary>
    /// The type of command invocation.
    /// </summary>
    [Flags]
    public enum CommandInvocationType
    {
        /// <summary>
        /// The command was created virtually and likely doesn't resemble a real command.
        /// </summary>
        VirtualCommand = 1 << 0,

        /// <summary>
        /// The command was invoked by a user sending a Discord message.
        /// </summary>
        TextCommand = 1 << 1,

        /// <summary>
        /// The command was invoked by a user sending a Slash Command.
        /// </summary>
        SlashCommand = 1 << 2,
    }
}
