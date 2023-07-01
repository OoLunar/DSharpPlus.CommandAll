using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DSharpPlus.CommandAll.Parsers
{
    /// <inheritdoc cref="ITextArgumentParser"/>
    public partial class RegexTextParser : ITextArgumentParser
    {
        public static readonly Regex QuoteMatcher = ArgumentMatcherRegex();

        /// <inheritdoc/>
        public bool TryExtractArguments(CommandAllExtension extension, string message, out IReadOnlyList<string> arguments)
        {
            if (message is null)
            {
                arguments = Array.Empty<string>();
                return false;
            }
            else if (message == string.Empty)
            {
                // We do this for no parameter overloads such as HelloWorldAsync(CommandContext context)
                arguments = Array.Empty<string>();
                return true;
            }

            List<string> args = new();
            List<Match> matches = ArgumentMatcherRegex().Matches(message).ToList();
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    args.Add(match.Value);
                }
            }

            arguments = args;
            return true;
        }

        [GeneratedRegex("(?:(?:\"|\\'|«|»|‘|“|„|‟)[^\"\\']+(?:\"|\\'|«|»|‘|“|„|‟)|```[^`]+```|\\S+)")]
        private static partial Regex ArgumentMatcherRegex();
    }
}
