using System;
using System.Collections.Generic;
using System.Linq;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    /// <inheritdoc cref="ITextArgumentParser"/>
    public class CommandsNextStyleTextArgumentParser : ITextArgumentParser
    {
        /// The characters that can be used to quote arguments. Defaults to <see cref="CommandAllConfiguration.QuoteCharacters"/>.
        private readonly char[] _quoteCharacters;

        /// <summary>
        /// The configuration used to grab the quote characters.
        /// </summary>
        public CommandsNextStyleTextArgumentParser(CommandAllConfiguration configuration) => _quoteCharacters = configuration.QuoteCharacters ?? throw new ArgumentNullException(nameof(configuration));

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
            ArgumentState argumentState = ArgumentState.None;
            int i, backtickCount = 0; // backtickCount should never exceed 5 and should be reset on the 6th backtick (an empty codeblock)
            ReadOnlySpan<char> messageSpan = message.AsSpan();
            char quoteCharacter = '\0';
            for (i = 0; i < messageSpan.Length; i++)
            {
                char character = messageSpan[i];

                if (argumentState.HasFlag(ArgumentState.Quoted))
                {
                    if (character == '\\')
                    {
                        argumentState |= ArgumentState.Escaped;
                    }
                    else if (_quoteCharacters.Contains(character) && character == quoteCharacter && !argumentState.HasFlag(ArgumentState.Escaped))
                    {
                        quoteCharacter = '\0';
                        argumentState &= ~ArgumentState.Quoted;
                    }
                    else if (argumentState.HasFlag(ArgumentState.Escaped))
                    {
                        argumentState &= ~ArgumentState.Escaped;
                    }
                }
                else if (argumentState.HasFlag(ArgumentState.Backticked))
                {
                    if (character == '`')
                    {
                        backtickCount++;
                    }
                    else if (backtickCount == 6)
                    {
                        argumentState &= ~ArgumentState.Backticked;
                        backtickCount = 0;
                    }
                    else
                    {
                        backtickCount = 0;
                    }
                }
                else if (_quoteCharacters.Contains(character))
                {
                    quoteCharacter = character;
                    argumentState |= ArgumentState.Quoted;
                }
                else if (character == '`')
                {
                    argumentState |= ArgumentState.Backticked;
                }
                else if (character == ' ' && !argumentState.HasFlag(ArgumentState.Quoted))
                {
                    args.Add(YieldArgument(messageSpan, i));
                    messageSpan = messageSpan[(i + 1)..];
                    i = -1;
                }
            }

            if (i != -1)
            {
                args.Add(YieldArgument(messageSpan, i));
            }

            arguments = args.AsReadOnly();
            return true;
        }

        private string YieldArgument(ReadOnlySpan<char> text, int i) => text.IndexOfAny(_quoteCharacters) == 0
            ? text[1..(i - 1)].ToString()
            : text[..i].Trim().ToString();
    }
}
