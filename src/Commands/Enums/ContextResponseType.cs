using System;

namespace DSharpPlus.CommandAll.Commands.Enums
{
    /// <summary>
    /// The type of response that has been sent to the user.
    /// </summary>
    [Flags]
    public enum ContextResponseType
    {
        /// <summary>
        /// There has been no response yet.
        /// </summary>
        None = 0,

        /// <summary>
        /// The command has been delayed, however no message has been created yet.
        /// </summary>
        Delayed = 1 << 1,

        /// <summary>
        /// A message has been created.
        /// </summary>
        Created = 1 << 2,

        /// <summary>
        /// The message has been updated.
        /// </summary>
        Updated = 1 << 3,

        /// <summary>
        /// The message has been deleted.
        /// </summary>
        Deleted = 1 << 4,

        /// <summary>
        /// The bot has requested more information form the user, usually by sending a message or modal.
        /// </summary>
        Prompt = 1 << 5,
    }
}
