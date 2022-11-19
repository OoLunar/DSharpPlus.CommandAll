using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    public class CommandManager : ICommandManager
    {
        public IReadOnlyDictionary<string, Command> Commands => _commands.AsReadOnly();
        public IArgumentConverterManager ArgumentConverterManager { get; init; }
        public ICommandOverloadParser CommandOverloadHandler { get; init; }

        private readonly Dictionary<string, Command> _commands = new();
        private readonly ILogger<CommandManager> _logger = NullLogger<CommandManager>.Instance;

        public CommandManager(IArgumentConverterManager argumentConverterManager, ICommandOverloadParser commandOverloadHandler, ILogger<CommandManager>? logger = null)
        {
            ArgumentConverterManager = argumentConverterManager ?? throw new ArgumentNullException(nameof(argumentConverterManager));
            CommandOverloadHandler = commandOverloadHandler ?? throw new ArgumentNullException(nameof(commandOverloadHandler));
            _logger = logger ?? NullLogger<CommandManager>.Instance;
        }

        public void AddCommand<T>() where T : BaseCommand => AddCommand(typeof(T));
        public void AddCommand(Type commandType) => AddCommands(new[] { commandType });
        public void AddCommands(Assembly assembly) => AddCommands(assembly.GetTypes());
        public void AddCommands(IEnumerable<Type> commandTypes)
        {
            foreach (Type type in commandTypes)
            {
                // Skip classes that don't inherit BaseCommand or are nested inside other classes.
                if (!type.IsSubclassOf(typeof(BaseCommand)) || type.IsNested)
                {
                    continue;
                }
                else if (!CommandBuilder.TryParse(type, out IEnumerable<CommandBuilder>? builders, out string? parseError))
                {
                    _logger.LogWarning("{Class}.{Method} failed to parse command {Type}. Skipping. Error: {Error}", nameof(CommandBuilder), nameof(CommandBuilder.TryParse), type, parseError);
                    continue;
                }
                else
                {
                    foreach (CommandBuilder builder in builders)
                    {
                        if (!builder.TryBuild(out Command? command, out string? buildError))
                        {
                            _logger.LogWarning("{Class}.{Method} failed to build command {Type}. Skipping. Error: {Error}", nameof(CommandBuilder), nameof(CommandBuilder.TryBuild), type, buildError);
                            continue;
                        }
                        else
                        {
                            foreach (CommandOverload overload in command.Overloads)
                            {
                                ArgumentConverterManager.AddParameters(overload.Parameters);
                            }

                            foreach (string alias in command.Aliases)
                            {
                                if (_commands.ContainsKey(alias))
                                {
                                    _logger.LogWarning("Failed to register command {Type}. Command name {Name} is already registered. Skipping.", type, command.Name);
                                    continue;
                                }

                                _commands.Add(alias, command);
                            }
                        }
                    }
                }
            }
        }

        public bool TryFindCommand(string commandString, [NotNullWhen(true)] out string? rawArguments, [NotNullWhen(true)] out Command? command)
        {
            if (string.IsNullOrWhiteSpace(commandString))
            {
                command = null;
                rawArguments = null;
                return false;
            }

            string[] split = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!_commands.TryGetValue(split[0], out command))
            {
                rawArguments = null;
                return false;
            }

            int i = 1; // Start at 1 because the first element is the command name. Incremented after each subcommand is found.
            while (command.Subcommands.Count != 0 && i < split.Length)
            {
                foreach (Command subcommand in command.Subcommands)
                {
                    if (subcommand.Aliases.Contains(split[i]))
                    {
                        command = subcommand;
                        i++;
                        break;
                    }
                }
            }

            rawArguments = string.Join(' ', split.Skip(i));
            return true;
        }
    }
}
