using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    public class PrefixParser : IPrefixParser
    {
        public IReadOnlyList<string> Prefixes { get; init; } = new[] { "!" };

        public PrefixParser(params string[] prefixes) => Prefixes = prefixes.Length == 0 ? new[] { "!" } : prefixes;

        public bool TryRemovePrefix(string message, [NotNullWhen(true)] out string? messageWithoutPrefix)
        {
            foreach (string prefix in Prefixes)
            {
                if (message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    messageWithoutPrefix = message[prefix.Length..].Trim();
                    return true;
                }
            }

            messageWithoutPrefix = null;
            return false;
        }
    }
}
