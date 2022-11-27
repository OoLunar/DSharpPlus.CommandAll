using System;
using System.Collections.Generic;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    /// <summary>
    /// Attempts to parse multiple arguments from a singular string.
    /// </summary>
    public interface ITextArgumentParser
    {
        /// <summary>
        /// Attempts to parse multiple arguments from a singular string.
        /// </summary>
        /// <param name="extension">The <see cref="CommandAllExtension"/> invoking this method.</param>
        /// <param name="message">The message content.</param>
        /// <param name="arguments">The arguments parsed from the message.</param>
        /// <returns>True if the arguments were parsed successfully, false otherwise.</returns>
        bool TryExtractArguments(CommandAllExtension extension, string message, out IReadOnlyList<string> arguments);
    }

    [Flags]
    internal enum ArgumentState
    {
        None = 0,
        Quoted = 1 << 1,
        Escaped = 1 << 2,
        Backticked = 1 << 3
    }
}
