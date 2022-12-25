namespace DSharpPlus.CommandAll.Commands.Enums
{
    /// <summary>
    /// The type of command invocation.
    /// </summary>
    public enum CommandInvocationType : byte
    {
        /// <summary>
        /// The command was created virtually and likely doesn't resemble a real command.
        /// </summary>
        VirtualCommand,

        /// <summary>
        /// The command was invoked by a user sending a Discord message.
        /// </summary>
        TextCommand,

        /// <summary>
        /// The command was invoked by a user sending a Slash Command.
        /// </summary>
        SlashCommand
    }
}
