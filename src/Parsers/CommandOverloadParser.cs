using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    /// <inheritdoc cref="ICommandOverloadParser"/>
    public class CommandOverloadParser : ICommandOverloadParser
    {
        /// <summary>
        /// The logger used to log messages.
        /// </summary>
        private readonly ILogger<CommandOverloadParser> _logger = NullLogger<CommandOverloadParser>.Instance;

        /// <inheritdoc/>
        public CommandOverloadParser(ILogger<CommandOverloadParser>? logger = null) => _logger = logger ?? NullLogger<CommandOverloadParser>.Instance;

        /// <inheritdoc/>
        public bool TryParseOverload(Command command, IEnumerable<string> arguments, [NotNullWhen(true)] out CommandOverload? overload)
        {
            _logger.LogDebug("Attempting to find a valid overload for command {CommandName} with arguments {Arguments}", command.Name, arguments);
            int argCount = arguments.Count();
            foreach (CommandOverload commandOverload in command.Overloads)
            {
                // Skip disabled overloads
                if (commandOverload.Flags.HasFlag(CommandOverloadFlags.Disabled))
                {
                    _logger.LogDebug("Skipping disabled overload {Overload}", commandOverload);
                    continue;
                }

                bool skipOverload = false;
                int i;
                for (i = 1; i < commandOverload.Parameters.Count; i++) // i = 1 to skip the first parameter, which should always be CommandContext
                {
                    // Check if there is a parameter with the same name as the argument.
                    // If there is not, check if the parameter is optional.
                    // If it is not, skip the overload.
                    CommandParameter parameter = commandOverload.Parameters[i];
                    if (i > argCount && !parameter.Flags.HasFlag(CommandParameterFlags.Optional))
                    {
                        _logger.LogDebug("Skipping overload {Overload} because it does not have a value for non-optional parameter {Parameter}", commandOverload, parameter);
                        skipOverload = true;
                        break;
                    }
                }

                // If there were more arguments provided than parameters, skip the overload
                if (argCount > i && !commandOverload.Parameters[^1].Flags.HasFlag(CommandParameterFlags.Params))
                {
                    _logger.LogDebug("Skipping overload {Overload} because it has more arguments than parameters", commandOverload);
                    skipOverload = true;
                }

                if (!skipOverload)
                {
                    _logger.LogDebug("Found a valid overload {Overload}", commandOverload);
                    overload = commandOverload;
                    return true;
                }
            }

            _logger.LogDebug("No overload found for command {Command} with provided arguments {Arguments}", command, arguments);
            overload = null;
            return false;
        }
    }
}
