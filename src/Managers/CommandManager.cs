using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    public class CommandManager : ICommandManager
    {
        public IReadOnlyDictionary<string, Command> Commands { get; private set; } = new Dictionary<string, Command>();
        public Dictionary<string, CommandBuilder> CommandBuilders { get; set; } = new();
        private readonly ILogger<CommandManager> _logger = NullLogger<CommandManager>.Instance;

        public CommandManager(ILogger<CommandManager>? logger = null) => _logger = logger ?? NullLogger<CommandManager>.Instance;

        public void AddCommand<T>() where T : BaseCommand => AddCommand(typeof(T));
        public void AddCommand(Type commandType) => AddCommands(new[] { commandType });
        public void AddCommands(Assembly assembly) => AddCommands(assembly.GetExportedTypes());
        public void AddCommands(IEnumerable<Type> commandTypes)
        {
            foreach (Type commandType in commandTypes)
            {
                if (!commandType.IsNested && CommandBuilder.TryParse(commandType, out IEnumerable<CommandBuilder>? commandBuilders))
                {
                    foreach (CommandBuilder commandBuilder in commandBuilders)
                    {
                        if (CommandBuilders.TryGetValue(commandBuilder.Name!, out CommandBuilder? existingCommandBuilder))
                        {
                            _logger.LogError("Command {ExistingCommandBuilder} already has the name {ExistingCommandBuilderName}. Unable to add {CommandBuilder}", existingCommandBuilder, existingCommandBuilder.Name, commandBuilder);
                        }
                        else
                        {
                            CommandBuilders.Add(commandBuilder.Name!, commandBuilder);
                        }
                    }
                }
            }
        }

        public void BuildCommands()
        {
            Dictionary<string, Command> commands = new();
            foreach (CommandBuilder commandBuilder in CommandBuilders.Values)
            {
                if (!commandBuilder.TryVerify(out Exception? error))
                {
                    _logger.LogError(error, "Failed to verify command builder {CommandBuilder}", commandBuilder);
                    continue;
                }
                else if (commands.TryGetValue(commandBuilder.Name!, out Command? existingCommand))
                {
                    _logger.LogError("Command {ExistingCommand} already has the name {ExistingCommandName}. Unable to add {CommandBuilder}", existingCommand, existingCommand.Name, commandBuilder);
                    continue;
                }

                Command command = new(commandBuilder);
                commands.Add(command.Name, command);
                foreach (string alias in command.Aliases)
                {
                    if (commands.TryGetValue(alias, out Command? existingAliasCommand))
                    {
                        _logger.LogError("Command {ExistingAliasCommand} already has the alias {ExistingAliasCommandAlias}. Unable to add {CommandBuilder}", existingAliasCommand, alias, commandBuilder);
                        continue;
                    }
                    commands.Add(alias, command);
                }
            }

            Commands = commands;
        }

        public bool TryFindCommand(string commandString, [NotNullWhen(true)] out string? rawArguments, [NotNullWhen(true)] out Command? command)
        {
            if (string.IsNullOrWhiteSpace(commandString))
            {
                command = null;
                rawArguments = null;
                return false;
            }
            else if (Commands is null)
            {
                command = null;
                rawArguments = null;
                return false;
            }

            string[] split = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!Commands.TryGetValue(split[0], out command))
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

        public IEnumerable<DiscordApplicationCommand> BuildSlashCommands()
        {
            List<DiscordApplicationCommand> slashCommands = new();
            foreach (Command command in Commands.Values.Distinct())
            {
                slashCommands.Add((DiscordApplicationCommand)command);
            }
            return slashCommands;
        }
    }
}
