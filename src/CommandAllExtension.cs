using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Emzi0767.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Commands.Executors;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;
using OoLunar.DSharpPlus.CommandAll.EventArgs;
using OoLunar.DSharpPlus.CommandAll.Exceptions;
using OoLunar.DSharpPlus.CommandAll.Managers;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll
{
    /// <summary>
    /// Because not everyone can decide between slash commands and text commands.
    /// </summary>
    public sealed class CommandAllExtension : BaseExtension
    {
        /// <summary>
        /// The services used when utilizing dependency injection.
        /// </summary>
        public readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// The command manager, used to register and execute commands.
        /// </summary>
        public readonly ICommandManager CommandManager;

        /// <summary>
        /// The argument converter manager, used to add converters and to assign converters to command overload parameters.
        /// </summary>
        public readonly IArgumentConverterManager ArgumentConverterManager;

        /// <summary>
        /// The command overload parser, which determines which command overload to execute by determining which overload has the most matching parameters.
        /// </summary>
        public readonly ICommandOverloadParser CommandOverloadParser;

        /// <summary>
        /// The command executor, which executes the command overload determined by the command overload parser.
        /// </summary>
        public readonly ICommandExecutor CommandExecutor;

        /// <summary>
        /// The prefix parser, which determines the prefix used to invoke a command.
        /// </summary>
        public readonly IPrefixParser PrefixParser;

        /// <summary>
        /// The text parser, which parses the <see cref="DiscordMessage.Content"/> into arguments. Supports quoted arguments, escaping quotes and multiline codeblocks.
        /// </summary>
        public readonly ITextArgumentParser TextArgumentParser;

        /// <summary>
        /// When the project is built in debug mode, this is the guild that the extension will register slash commands too. If this is null, slash commands will not be registered.
        /// </summary>
        public readonly ulong? DebugGuildId;

        /// <summary>
        /// The strategy used when naming slash commands. Defaults to <see cref="CommandParameterNamingStrategy.SnakeCase"/>.
        /// </summary>
        public readonly CommandParameterNamingStrategy ParameterNamingStrategy;

        /// <summary>
        /// How long <see cref="CommandContext.PromptAsync"/> should wait PER TEXT COMPONENT before timing out. Defaults to 30 seconds per <see cref="TextInputComponent"/>.
        /// </summary>
        public readonly TimeSpan PromptTimeout;

        /// <summary>
        /// Executed everytime a command is finished executing.
        /// </summary>
        public event AsyncEventHandler<CommandAllExtension, CommandExecutedEventArgs> CommandExecuted { add => _commandExecuted.Register(value); remove => _commandExecuted.Unregister(value); }
        internal readonly AsyncEvent<CommandAllExtension, CommandExecutedEventArgs> _commandExecuted = new("COMMANDALL_COMMAND_EXECUTED", TimeSpan.FromSeconds(-1), EverythingWentWrongErrorHandler);

        /// <summary>
        /// Executed before registering slash commands.
        /// </summary>
        public event AsyncEventHandler<CommandAllExtension, ConfigureCommandsEventArgs> ConfigureCommands { add => _configureCommands.Register(value); remove => _configureCommands.Unregister(value); }
        internal readonly AsyncEvent<CommandAllExtension, ConfigureCommandsEventArgs> _configureCommands = new("COMMANDALL_CONFIGURE_COMMANDS", TimeSpan.FromSeconds(-1), EverythingWentWrongErrorHandler);

        /// <summary>
        /// Executed everytime a command errored and <see cref="BaseCommand.OnErrorAsync(CommandContext, Exception)"/> also errored.
        /// </summary>
        public event AsyncEventHandler<CommandAllExtension, CommandErroredEventArgs> CommandErrored { add => _commandErrored.Register(value); remove => _commandErrored.Unregister(value); }
        internal readonly AsyncEvent<CommandAllExtension, CommandErroredEventArgs> _commandErrored = new("COMMANDALL_COMMAND_ERRORED", TimeSpan.FromSeconds(-1), EverythingWentWrongErrorHandler);

        /// <summary>
        /// Used to log messages from this extension.
        /// </summary>
        private readonly ILogger<CommandAllExtension> _logger;

        /// <summary>
        /// Creates a new instance of the <see cref="CommandAllExtension"/> class.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        internal CommandAllExtension(CommandAllConfiguration configuration)
        {
            // Add the configuration and extension to the service collection.
            ServiceProvider = configuration.ServiceCollection.AddSingleton(configuration).AddSingleton(this).BuildServiceProvider();
            CommandManager = configuration.CommandManager;
            ArgumentConverterManager = configuration.ArgumentConverterManager;
            CommandOverloadParser = configuration.CommandOverloadParser;
            CommandExecutor = configuration.CommandExecutor;
            PrefixParser = configuration.PrefixParser;
            TextArgumentParser = configuration.TextArgumentParser;
            DebugGuildId = configuration.DebugGuildId;
            ParameterNamingStrategy = configuration.ParameterNamingStrategy;
            PromptTimeout = configuration.PromptTimeout;

            // Add the default converters to the argument converter manager.
            ArgumentConverterManager.AddArgumentConverters(typeof(CommandAllExtension).Assembly);

            // Attempt to get the user defined logging, otherwise setup a null logger since the D#+ Default Logger is internal.
            _logger = ServiceProvider.GetRequiredService<ILogger<CommandAllExtension>>();
        }

        /// <summary>
        /// Sets up the extension to use the specified <see cref="Client"/>.
        /// </summary>
        /// <param name="client">The client to register our event handlers too.</param>
        protected override void Setup(DiscordClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            else if (Client is not null)
            {
                throw new InvalidOperationException("CommandAll Extension is already initialized.");
            }

            Client = client;
            if (!Client.Intents.HasFlag(DiscordIntents.MessageContents))
            {
                _logger.LogError("The client is missing the MessageContents intent, which is required for text commands to execute.");
            }

            Client.MessageCreated += CommandContext.HandleMessageAsync;
            Client.ModalSubmitted += CommandContext.HandleModalAsync;

            // If the client has already been initialized, register the event handlers.
            if (Client.Guilds.Count != 0)
            {
                DiscordClient_ReadyAsync(Client, null!).GetAwaiter().GetResult();
            }
            else
            {
                Client.Ready += DiscordClient_ReadyAsync;
            }
        }

        public void AddCommand<T>() where T : BaseCommand => CommandManager.AddCommand<T>(this);
        public void AddCommand(Type type) => CommandManager.AddCommand(this, type);
        public void AddCommands(Assembly assembly) => CommandManager.AddCommands(this, assembly);
        public void AddCommands(params Type[] types) => CommandManager.AddCommands(this, types);

        /// <summary>
        /// Registers the event handlers that are used to handle commands. This is called when the <see cref="Client"/> is ready.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="eventArgs">Unused.</param>
        private async Task DiscordClient_ReadyAsync(DiscordClient sender, ReadyEventArgs eventArgs)
        {
            // Prevent the event handler from being executed multiple times.
            Client.Ready -= DiscordClient_ReadyAsync;

            // Iterate through all the commands and ensure that all the parameters can be converted.
            foreach (CommandBuilder commandBuilder in CommandManager.CommandBuilders.Values)
            {
                if (!ArgumentConverterManager.TrySaturateParameters(commandBuilder.GetAllParameters(), out IEnumerable<CommandParameterBuilder>? failedParameters))
                {
                    foreach (CommandParameterBuilder parameterBuilder in failedParameters!)
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
                        if (parameterBuilder.OverloadBuilder.Command.Overloads.All(x => x.Flags.HasFlag(CommandOverloadFlags.Disabled)))
                        {
                            _logger.LogError("Disabling command {Command} due to all overloads being disabled.", commandBuilder);
                            parameterBuilder.OverloadBuilder!.Command.Flags |= CommandFlags.Disabled;
                        }
                        else if (!parameterBuilder.OverloadBuilder.Command.Flags.HasFlag(CommandFlags.Disabled))
                        {
                            _logger.LogWarning("Disabling overload {CommandOverload} due to missing converters for the following parameters: {FailedParameters}", parameterBuilder.OverloadBuilder, parameterBuilder);
                        }
                    }
                }
            }

            await _configureCommands.InvokeAsync(this, new ConfigureCommandsEventArgs(this, CommandManager));
            CommandManager.BuildCommands();
            Client.MessageCreated += DiscordClient_MessageCreatedAsync;

            // Intentionally run this in a separate task so that the Ready event can finish executing and NOT yell at the user.
            _ = Task.Run(async () =>
            {
                IEnumerable<DiscordApplicationCommand> applicationCommands = CommandManager.BuildSlashCommands();

#if DEBUG
                // Don't register slash commands if the debug guild id is null.
                if (DebugGuildId is not null)
                {
                    await Client.BulkOverwriteGuildApplicationCommandsAsync(DebugGuildId.Value, applicationCommands);
                }
                else
                {
                    _logger.LogWarning("DebugGuildId is null, not registering slash commands.");
                }
#else
                await Client.BulkOverwriteGlobalApplicationCommandsAsync(applicationCommands);
#endif

                Client.InteractionCreated += DiscordClient_InteractionCreatedAsync;
                _logger.LogInformation("CommandAll Extension is now ready to handle all commands.");
            });
        }

        /// <summary>
        /// Used to parse text commands and execute them.
        /// </summary>
        /// <param name="client">Unused.</param>
        /// <param name="eventArgs">Used to read <see cref="DiscordMessage.Content"/> to parse and execute commands.</param>
        private Task DiscordClient_MessageCreatedAsync(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            // Remove the prefix
            if (!PrefixParser.TryRemovePrefix(this, eventArgs.Message.Content, out string? commandString))
            {
                return Task.CompletedTask;
            }
            // Try to find the command, resolve to subcommand if needed. Removes the command from the string, leaving the args
            else if (!CommandManager.TryFindCommand(commandString, out string? rawArguments, out Command? command))
            {
                return _commandErrored.InvokeAsync(this, new CommandErroredEventArgs(new CommandContext(eventArgs.Channel, eventArgs.Author, null, eventArgs.Message, eventArgs.Guild, eventArgs.Author as DiscordMember, this, null!, null!), new CommandNotFoundException("Command was not found.", commandString)));
            }
            else if (!command.Flags.HasFlag(CommandFlags.Disabled))
            {
                CommandContext context = new(this, command, eventArgs.Message, rawArguments);
                try
                {
                    return CommandExecutor.ExecuteAsync(context);
                }
                catch (Exception error)
                {
                    return _commandErrored.InvokeAsync(this, new(context, error));
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles applications commands.
        /// </summary>
        /// <param name="client">Unused.</param>
        /// <param name="eventArgs">Used to determine if the interaction is an application command and conditionally executes commands with the provided data.</param>
        private Task DiscordClient_InteractionCreatedAsync(DiscordClient client, InteractionCreateEventArgs eventArgs)
        {
            if (eventArgs.Interaction.Type is not InteractionType.ApplicationCommand)
            {
                return Task.CompletedTask;
            }

            StringBuilder commandName = new(eventArgs.Interaction.Data.Name);
            DiscordInteractionDataOption[] options = eventArgs.Interaction.Data.Options?.ToArray() ?? Array.Empty<DiscordInteractionDataOption>();
            while (options.Any())
            {
                DiscordInteractionDataOption firstOption = options[0];
                if (firstOption.Type is not ApplicationCommandOptionType.SubCommandGroup and not ApplicationCommandOptionType.SubCommand)
                {
                    break;
                }

                _ = commandName.AppendFormat(" {0}", firstOption.Name);
                options = firstOption.Options?.ToArray() ?? Array.Empty<DiscordInteractionDataOption>();
            }

            if (!CommandManager.TryFindCommand(commandName.ToString(), out _, out Command? command))
            {
                return Task.CompletedTask;
            }
            else
            {
                CommandContext context = new(this, command, eventArgs.Interaction, options);
                try
                {
                    return CommandExecutor.ExecuteAsync(context);
                }
                catch (Exception error)
                {
                    return _commandErrored.InvokeAsync(this, new(context, error));
                }
            }
        }

        /// <summary>
        /// The event handler used to log all unhandled exceptions, usually from when <see cref="_commandErrored"/> itself errors.
        /// </summary>
        /// <param name="asyncEvent">The event that errored.</param>
        /// <param name="error">The error that occurred.</param>
        /// <param name="handler">The handler/method that errored.</param>
        /// <param name="sender">The extension.</param>
        /// <param name="eventArgs">The event arguments passed to <paramref name="handler"/>.</param>
        private static void EverythingWentWrongErrorHandler<TArgs>(AsyncEvent<CommandAllExtension, TArgs> asyncEvent, Exception error, AsyncEventHandler<CommandAllExtension, TArgs> handler, CommandAllExtension sender, TArgs eventArgs) where TArgs : AsyncEventArgs => sender._logger.LogError(error, "Event handler '{Method}' for event {AsyncEvent} threw an unhandled exception.", handler.Method, asyncEvent.Name);

        /// <summary>
        /// Attempts to add argument converters to all parameters in the command.
        /// </summary>
        /// <param name="command">The command whose parameters to add argument converters too.</param>
        private void SaturateParametersRecursively(CommandBuilder commandBuilder)
        {
            foreach (CommandOverloadBuilder commandOverloadBuilder in commandBuilder.Overloads)
            {
                if (!ArgumentConverterManager.TrySaturateParameters(commandOverloadBuilder.Parameters, out IEnumerable<CommandParameterBuilder>? failedParameters))
                {
                    commandOverloadBuilder.Flags |= CommandOverloadFlags.Disabled;
                    _logger.LogWarning("Disabling overload {CommandOverload} due to missing converters for the following parameters: {FailedParameters}", commandOverloadBuilder, failedParameters);
                }
            }

            foreach (CommandBuilder subBuilder in commandBuilder.Subcommands)
            {
                SaturateParametersRecursively(subBuilder);
            }

            if (commandBuilder.Overloads.Any() && commandBuilder.Overloads.All(overload => overload.Flags.HasFlag(CommandOverloadFlags.Disabled)))
            {
                commandBuilder.Flags |= CommandFlags.Disabled;
                _logger.LogWarning("Disabling command {Command} due to all overloads being disabled.", commandBuilder);
            }
        }
    }
}
