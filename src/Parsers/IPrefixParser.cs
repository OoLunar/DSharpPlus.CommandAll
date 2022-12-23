using System.Diagnostics.CodeAnalysis;

namespace DSharpPlus.CommandAll.Parsers
{
    /// <summary>
    /// Attempts to locate and remove the prefix from a text command.
    /// </summary>
    public interface IPrefixParser
    {
        /// <summary>
        /// Attempts to locate and remove the prefix from a text command.
        /// </summary>
        /// <param name="extension">The <see cref="CommandAllExtension"/> invoking this method.</param>
        /// <param name="message">The message to parse the prefix from.</param>
        /// <param name="messageWithoutPrefix">The message without the prefix.</param>
        /// <returns>True if a prefix was found, false otherwise.</returns>
        bool TryRemovePrefix(CommandAllExtension extension, string message, [NotNullWhen(true)] out string? messageWithoutPrefix);
    }
}
