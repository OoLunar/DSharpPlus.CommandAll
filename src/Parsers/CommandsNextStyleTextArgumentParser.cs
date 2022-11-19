using System;
using System.Collections.Generic;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    public class CommandsNextStyleTextArgumentParser : ITextArgumentParser
    {
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
                switch (character)
                {
                    // TODO: Respect quote chars from config
                    case '"' when argumentState is ArgumentState.None or ArgumentState.Quoted:
                        if (argumentState == ArgumentState.None)
                        {
                            argumentState |= ArgumentState.Quoted;
                        }
                        else if (argumentState == ArgumentState.Quoted)
                        {
                            argumentState &= ~ArgumentState.Quoted;
                        }
                        break;
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
                args.Add(messageSpan[..i].ToString());
            }

            arguments = args.AsReadOnly();
            return true;
        }
    }
}
