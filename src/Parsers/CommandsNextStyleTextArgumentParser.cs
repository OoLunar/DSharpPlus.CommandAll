using System;
using System.Collections.Generic;
using System.Linq;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    public class CommandsNextStyleTextArgumentParser : ITextArgumentParser
    {
        private readonly char[] _quoteCharacters;

        public CommandsNextStyleTextArgumentParser(CommandAllConfiguration configuration) => _quoteCharacters = configuration.QuoteCharacters ?? throw new ArgumentNullException(nameof(configuration));

        public bool TryExtractArguments(string message, out IReadOnlyList<string> arguments)
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

            for (i = 0; i < messageSpan.Length; i++)
            {
                char character = messageSpan[i];

                if (argumentState.HasFlag(ArgumentState.Escaped) && character != '\\')
                {
                    argumentState &= ~ArgumentState.Escaped;
                }

                if (_quoteCharacters.Contains(character) && argumentState is ArgumentState.None or ArgumentState.Quoted)
                {
                    if (argumentState == ArgumentState.None)
                    {
                        argumentState |= ArgumentState.Quoted;
                    }
                    else if (argumentState == ArgumentState.Quoted)
                    {
                        argumentState &= ~ArgumentState.Quoted;
                    }
                }

                if (character == ' ' && argumentState is not ArgumentState.Whitespace)
                {
                    argumentState |= ArgumentState.Whitespace;
                    continue;
                }
                else if (character == ' ')
                {
                    continue;
                }
                else
                {
                    argumentState &= ~ArgumentState.Whitespace;
                }

                // Unsure if this should be in an else statement, in case if the quote characters contain any of the below characters.
                switch (character)
                {
                    case '\\' when argumentState is ArgumentState.None:
                        argumentState &= ArgumentState.Escaped;
                        break;
                    case '`' when argumentState is ArgumentState.None or ArgumentState.Backticked:
                        if (backtickCount == 5)
                        {
                            backtickCount = 0;
                            argumentState &= ~ArgumentState.Backticked;
                        }
                        break;
                    case ' ' when argumentState is ArgumentState.None:
                        args.Add(YieldArgument(messageSpan, i));
                        messageSpan = messageSpan[(i + 1)..];
                        i = -1;
                        break;
                }

                if (backtickCount != 0 && character != '`')
                {
                    backtickCount = 0;
                    argumentState &= ~ArgumentState.Backticked;
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
