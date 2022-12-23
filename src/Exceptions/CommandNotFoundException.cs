using System;

namespace DSharpPlus.CommandAll.Exceptions
{
    public sealed class CommandNotFoundException : CommandAllException
    {
        public readonly string CommandString;

        internal CommandNotFoundException(string message, string commandString) : base($"{message}: {commandString}") => CommandString = commandString ?? throw new ArgumentNullException(nameof(commandString));
    }
}
