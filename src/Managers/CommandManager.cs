using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Builders;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.CommandAll.Managers
{
    /// <inheritdoc cref="ICommandManager" />
    public class CommandManager : ICommandManager
    {
        /// <summary>
        /// The current list of <see cref="Command"/>s that are registered, indexed by their full name.
        /// </summary>
        public IReadOnlyDictionary<string, Command> Commands { get; private set; } = new ReadOnlyDictionary<string, Command>(new Dictionary<string, Command>());
        private readonly List<CommandBuilder> CommandBuilders = new();

        /// <summary>
        /// Used to log when a command is or isn't found.
        /// </summary>
        private readonly ILogger<CommandManager> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="CommandManager"/>.
        /// </summary>
        public CommandManager(ILogger<CommandManager>? logger = null) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <inheritdoc />
        public void AddCommands(CommandAllExtension extension, Assembly assembly) => AddCommands(extension, assembly.GetExportedTypes().Where(type => !type.IsNested && !type.IsAbstract && !type.IsInterface && typeof(BaseCommand).IsAssignableFrom(type)).ToArray());

        /// <inheritdoc />
        public void AddCommands(CommandAllExtension extension, params Type[] types)
        {
            foreach (Type type in types)
            {
                if (type.IsNested || type.IsAbstract || type.IsInterface)
                {
                    continue;
                }
                else if (CommandBuilder.TryParse(extension, type, out IReadOnlyList<CommandBuilder>? commandBuilders, out Exception? error))
                {
                    foreach (CommandBuilder commandBuilder in commandBuilders)
                    {
                        if (!CommandBuilders.Contains(commandBuilder))
                        {
                            CommandBuilders.Add(commandBuilder);
                        }
                    }
                }
                else if (!typeof(BaseCommand).IsAssignableFrom(type))
                {
                    _logger.LogError(error, "Unable to parse {Type}", type);
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>Calling this method will clear the <see cref="GetCommandBuilders"/> list.</remarks>
        public async Task RegisterCommandsAsync(CommandAllExtension extension)
        {
            Dictionary<string, Command> commands = new();
            foreach (CommandBuilder commandBuilder in CommandBuilders)
            {
                // Iterate through all the commands and ensure that all the parameters can be converted.
                if (!extension.ArgumentConverterManager.TrySaturateParameters(commandBuilder.GetAllOverloadBuilders().SelectMany(overload => overload.Parameters), out IEnumerable<CommandParameterBuilder>? failedParameters))
                {
                    foreach (CommandParameterBuilder parameterBuilder in failedParameters)
                    {
                        if (parameterBuilder.OverloadBuilder is null)
                        {
                            throw new InvalidOperationException($"OverloadBuilder is null on parameter {parameterBuilder}.");
                        }
                        else if (parameterBuilder.OverloadBuilder.Command is null)
                        {
                            throw new InvalidOperationException($"Command is null on overload {parameterBuilder.OverloadBuilder}.");
                        }

                        parameterBuilder.OverloadBuilder.Flags |= CommandOverloadFlags.Disabled;
                        if (!parameterBuilder.OverloadBuilder.Command.Flags.HasFlag(CommandFlags.Disabled))
                        {
                            if (parameterBuilder.OverloadBuilder.Command.Overloads.All(x => x.Flags.HasFlag(CommandOverloadFlags.Disabled)))
                            {
                                parameterBuilder.OverloadBuilder.Flags |= CommandOverloadFlags.Disabled;
                                _logger.LogWarning("Disabled overload {CommandOverload} due to missing converters for the following parameters: {FailedParameters}", parameterBuilder.OverloadBuilder, parameterBuilder);
                            }
                            else
                            {
                                parameterBuilder.OverloadBuilder!.Command.Flags |= CommandFlags.Disabled;
                                _logger.LogError("Disabled command {Command} due to all overloads being disabled.", commandBuilder);
                            }
                        }
                    }
                }

                Command? command = new(commandBuilder);
                foreach ((string alias, Command cmd) in command.Walk())
                {
                    if (!commands.TryAdd(alias, cmd))
                    {
                        _logger.LogError("Duplicate command {Command} found.", command);
                    }
                }
            };

            if (extension.Client.ShardId == 0)
            {
                CommandBuilders.Clear();
                IReadOnlyList<DiscordApplicationCommand> slashCommands = extension.DebugGuildId is not null
                    ? await extension.Client.BulkOverwriteGuildApplicationCommandsAsync(extension.DebugGuildId.Value, commands.Values.DistinctBy(x => x.Parent is null).Select(command => (DiscordApplicationCommand)command))
                    : await extension.Client.BulkOverwriteGlobalApplicationCommandsAsync(commands.Values.DistinctBy(x => x.Parent is null).Select(command => (DiscordApplicationCommand)command));

                foreach (DiscordApplicationCommand slashCommand in slashCommands)
                {
                    foreach (Command command in commands.Values)
                    {
                        if (command.SlashMetadata.ApplicationCommandId is null && command.SlashName == slashCommand.Name)
                        {
                            command.SlashMetadata.ApplicationCommandId = slashCommand.Id;
                        }
                    }
                }

            }

            Commands = commands.AsReadOnly();
        }

        /// <inheritdoc />
        /// <remarks>This list is cleared whenever <see cref="RegisterCommandsAsync(CommandAllExtension)"/> is called.</remarks>
        public IReadOnlyList<CommandBuilder> GetCommandBuilders() => CommandBuilders.AsReadOnly();

        /// <inheritdoc />
        public IReadOnlyDictionary<string, Command> GetCommands() => Commands;

        /// <inheritdoc />
        public bool TryFindCommand(string fullCommand, [NotNullWhen(true)] out Command? command, [NotNullWhen(true)] out string? rawArguments)
        {
            string[] fullSplit = fullCommand.Split(' ');
            StringBuilder stringBuilder = new();

            int i;
            for (i = Math.Min(fullSplit.Length, 3); i >= 0; i--)
            {
                string key = string.Join(' ', fullSplit[0..i]);
                if (Commands.ContainsKey(key))
                {
                    stringBuilder.Append(key);
                    break;
                }
            }

            rawArguments = i >= fullSplit.Length ? string.Empty : string.Join(' ', fullSplit[(i + 1)..]);
            return Commands.TryGetValue(stringBuilder.ToString(), out command);
        }

        /// <inheritdoc />
        public bool TryFindCommand(ulong applicationCommandId, [NotNullWhen(true)] out Command? command) =>
            (command = Commands.Values.FirstOrDefault(x => x.SlashMetadata.ApplicationCommandId == applicationCommandId)) is not null;
    }
}
