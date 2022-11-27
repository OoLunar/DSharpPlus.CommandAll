using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    /// <inheritdoc cref="IPrefixParser"/>
    /// <remarks>
    /// Additionally supports mentioning the bot directly.
    /// </remarks>
    public class PrefixParser : IPrefixParser
    {
        /// <summary>
        /// The multiple prefixes that can be used to invoke a command. Defaults to <c>!</c>
        /// </summary>
        public IReadOnlyList<string> Prefixes { get; init; } = new[] { "!" };

        /// <summary>
        /// The strings that can be used to execute a command. Defaults to <c>!</c>.
        public PrefixParser(params string[] prefixes) => Prefixes = prefixes.Length == 0 ? new[] { "!" } : prefixes;

        /// <inheritdoc/>
        public bool TryRemovePrefix(CommandAllExtension extension, string message, [NotNullWhen(true)] out string? messageWithoutPrefix)
        {
            foreach (string prefix in Prefixes)
            {
                if (message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    messageWithoutPrefix = message[prefix.Length..].Trim();
                    return true;
                }
            }

            // Mention check
            if (message.StartsWith(extension.Client.CurrentUser.Mention, StringComparison.OrdinalIgnoreCase))
            {
                messageWithoutPrefix = message[extension.Client.CurrentUser.Mention.Length..].Trim();
                return true;
            }

            messageWithoutPrefix = null;
            return false;
        }
    }
}
