using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    /// <inheritdoc cref="ICommandManager" />
    public class CommandManager : ICommandManager
    {
        /// <inheritdoc />
        public IReadOnlyDictionary<string, Command> Commands { get; private set; } = new Dictionary<string, Command>();

        /// <inheritdoc />
        public Dictionary<string, CommandBuilder> CommandBuilders { get; set; } = new();

        /// <summary>
        /// Used to log when a command is or isn't found.
        /// </summary>
        private readonly ILogger<CommandManager> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="CommandManager"/>.
        /// </summary>
        public CommandManager(ILogger<CommandManager>? logger = null) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <inheritdoc />
        public void AddCommand<T>(CommandAllExtension extension) where T : BaseCommand => AddCommand(extension, typeof(T));

        /// <inheritdoc />
        public void AddCommand(CommandAllExtension extension, Type type) => AddCommands(extension, new[] { type });

        /// <inheritdoc />
        public void AddCommands(CommandAllExtension extension, Assembly assembly) => AddCommands(extension, assembly.GetExportedTypes());

        /// <inheritdoc />
        public void AddCommands(CommandAllExtension extension, IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (type.IsNested || type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                if (CommandBuilder.TryParse(extension, type, out IEnumerable<CommandBuilder>? commandBuilders, out Exception? error))
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
                else if (typeof(BaseCommand).IsAssignableFrom(type))
                {
                    _logger.LogError(error, "Unable to parse {Type}", type);
                }
            }
        }

        /// <inheritdoc />
        public void BuildCommands()
        {
            Dictionary<string, Command> commands = new();
            foreach (CommandBuilder commandBuilder in CommandBuilders.Values)
            {
                try
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
                catch (CommandAllException error)
                {
                    _logger.LogError(error, "Failed to build command builder {CommandBuilder}", commandBuilder);
                }
            }

            Commands = commands;
        }

        /// <inheritdoc />
        public IEnumerable<DiscordApplicationCommand> BuildSlashCommands()
        {
            List<DiscordApplicationCommand> slashCommands = new();
            foreach (Command command in Commands.Values.Distinct())
            {
                slashCommands.Add((DiscordApplicationCommand)command);
            }
            return slashCommands;
        }

        /// <inheritdoc />
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
                bool found = false;
                foreach (Command subcommand in command.Subcommands)
                {
                    if (subcommand.Aliases.Contains(split[i]))
                    {
                        found = true;
                        command = subcommand;
                        i++;
                        break;
                    }
                }

                if (!found)
                {
                    rawArguments = null;
                    return false;
                }
            }

            rawArguments = string.Join(' ', split.Skip(i));
            return true;
        }

        /// <inheritdoc />
        public bool TryFindCommand(string commandString, [NotNullWhen(true)] out string? rawArguments, [NotNullWhen(true)] out CommandBuilder? commandBuilder)
        {
            if (string.IsNullOrWhiteSpace(commandString))
            {
                commandBuilder = null;
                rawArguments = null;
                return false;
            }
            else if (CommandBuilders is null)
            {
                CommandBuilders = new();
                commandBuilder = null;
                rawArguments = null;
                return false;
            }

            string[] split = commandString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!CommandBuilders.TryGetValue(split[0], out commandBuilder))
            {
                rawArguments = null;
                return false;
            }

            int i = 1; // Start at 1 because the first element is the command name. Incremented after each subcommand is found.
            while (commandBuilder.Subcommands.Count != 0 && i < split.Length)
            {
                foreach (CommandBuilder subBuilder in commandBuilder.Subcommands)
                {
                    if (subBuilder.Aliases.Contains(split[i]))
                    {
                        commandBuilder = subBuilder;
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
