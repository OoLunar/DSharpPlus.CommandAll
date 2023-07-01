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
        private readonly CommandAllConfiguration _configuration;

        public RegexTextParser(CommandAllConfiguration configuration) => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

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
                if (!match.Success)
                {
                    continue;
                }
                else if (_configuration.QuoteCharacters.Contains(match.Value[0]) && _configuration.QuoteCharacters.Contains(match.Value[^1]) && match.Value[0] == match.Value[^1])
                {
                    args.Add(match.Value[1..^1]);
                }
                else
                {
                    args.Add(match.Value);
                }
            }

            arguments = args;
            return true;
        }

        [GeneratedRegex("""(?<!\\)((?:(["'«»‘“„‟]).*?[^\\]\2)|(```(?:.*?[\n]?)*?```)|(`.*`)|(\S+))""")]
        private static partial Regex ArgumentMatcherRegex();
    }
}
