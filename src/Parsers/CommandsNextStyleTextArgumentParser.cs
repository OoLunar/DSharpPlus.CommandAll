using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    public class CommandsNextStyleTextArgumentParser : ITextArgumentParser
    {
        private readonly char[] _quoteCharacters;
        private readonly ILogger<CommandsNextStyleTextArgumentParser> _logger;

        public CommandsNextStyleTextArgumentParser(CommandAllConfiguration configuration)
        {
            _quoteCharacters = configuration.QuoteCharacters ?? throw new ArgumentNullException(nameof(configuration));
            _logger = configuration.ServiceCollection.BuildServiceProvider().GetService<ILogger<CommandsNextStyleTextArgumentParser>>() ?? NullLogger<CommandsNextStyleTextArgumentParser>.Instance;
        }

        public bool TryExtractArguments(string message, out IReadOnlyList<string> arguments)
        {
            _logger.LogTrace("Parsing arguments from message: {Message}", message);
            if (message is null)
            {
                _logger.LogWarning("Message is null. This isn't supposed to happen.");
                arguments = Array.Empty<string>();
                return false;
            }
            else if (message == string.Empty)
            {
                // We do this for no parameter overloads such as HelloWorldAsync(CommandContext context)
                _logger.LogTrace("Message is empty, returning empty list of arguments. This happens when there are no parameters required for a message.");
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
                        _logger.LogDebug("Adding {Argument} as an argument.", messageSpan[..i].ToString());
                        args.Add(messageSpan[..i].ToString());
                        messageSpan = messageSpan[(i + 1)..];
                        i = -1;
                        break;
                }

                if (backtickCount != 0 && character != '`')
                {
                    backtickCount = 0;
                    argumentState &= ~ArgumentState.Backticked;
                }

                if (argumentState.HasFlag(ArgumentState.Escaped) && character != '\\')
                {
                    argumentState &= ~ArgumentState.Escaped;
                }
            }

            if (i != -1)
            {
                _logger.LogDebug("Adding {Argument} as an argument.", messageSpan.ToString());
                args.Add(messageSpan[..i].ToString());
            }

            _logger.LogDebug("Returning {Arguments} as arguments.", args);
            arguments = args.AsReadOnly();
            return true;
        }
    }
}
