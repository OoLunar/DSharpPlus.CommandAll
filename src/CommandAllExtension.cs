using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.AsyncEvents;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.CommandAll.Commands.Executors;
using DSharpPlus.CommandAll.EventArgs;
using DSharpPlus.CommandAll.Exceptions;
using DSharpPlus.CommandAll.Managers;
using DSharpPlus.CommandAll.Parsers;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.CommandAll
{
    /// <summary>
    /// Because not everyone can decide between slash commands and text commands.
    /// </summary>
    public sealed class CommandAllExtension : BaseExtension
    {
        /// <summary>
        /// The services used when utilizing dependency injection.
        /// </summary>
        public IServiceProvider ServiceProvider { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.CommandManager"/>
        public ICommandManager CommandManager { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.ArgumentConverterManager"/>
        public IArgumentConverterManager ArgumentConverterManager { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.CommandOverloadParser"/>
        public ICommandOverloadParser CommandOverloadParser { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.CommandExecutor"/>
        public ICommandExecutor CommandExecutor { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.PrefixParser"/>
        public IPrefixParser PrefixParser { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.TextArgumentParser"/>
        public ITextArgumentParser TextArgumentParser { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.DebugGuildId"/>
        public ulong? DebugGuildId { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.ParameterNamingStrategy"/>
        public CommandParameterNamingStrategy ParameterNamingStrategy { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.PromptTimeout"/>
        public TimeSpan PromptTimeout { get; init; }

        /// <inheritdoc cref="CommandAllConfiguration.FilteringStrategy"/>
        public CommandFilteringStrategy FilteringStrategy { get; init; }

        /// <summary>
        /// Executed everytime a command is finished executing.
        /// </summary>
        public event AsyncEventHandler<CommandAllExtension, CommandExecutedEventArgs> CommandExecuted { add => _commandExecuted.Register(value); remove => _commandExecuted.Unregister(value); }
        internal readonly AsyncEvent<CommandAllExtension, CommandExecutedEventArgs> _commandExecuted = new("COMMANDALL_COMMAND_EXECUTED", EverythingWentWrongErrorHandler);

        /// <summary>
        /// Executed before registering slash commands.
        /// </summary>
        public event AsyncEventHandler<CommandAllExtension, ConfigureCommandsEventArgs> ConfigureCommands { add => _configureCommands.Register(value); remove => _configureCommands.Unregister(value); }
        internal readonly AsyncEvent<CommandAllExtension, ConfigureCommandsEventArgs> _configureCommands = new("COMMANDALL_CONFIGURE_COMMANDS", EverythingWentWrongErrorHandler);

        /// <summary>
        /// Executed everytime a command errored and <see cref="BaseCommand.OnErrorAsync(CommandContext, Exception)"/> also errored.
        /// </summary>
        public event AsyncEventHandler<CommandAllExtension, CommandErroredEventArgs> CommandErrored { add => _commandErrored.Register(value); remove => _commandErrored.Unregister(value); }
        internal readonly AsyncEvent<CommandAllExtension, CommandErroredEventArgs> _commandErrored = new("COMMANDALL_COMMAND_ERRORED", EverythingWentWrongErrorHandler);

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
            FilteringStrategy = configuration.FilteringStrategy;

            // Add the default converters to the argument converter manager.
            ArgumentConverterManager.AddArgumentConverters(typeof(CommandAllExtension).Assembly);

            // Attempt to get the user defined logging, otherwise setup a null logger since the D#+ Default Logger is internal.
            _logger = ServiceProvider.GetRequiredService<ILogger<CommandAllExtension>>();
        }

        /// <summary>
        /// Sets up the extension to use the specified <see cref="DiscordClient"/>.
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
                _logger.LogError("The client is missing the MessageContents intent, which is required for text commands to execute. You've requested for text command support, which will not function.");
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

        public void AddCommands(Assembly assembly) => CommandManager.AddCommands(this, assembly);
        public void AddCommands(params Type[] types) => CommandManager.AddCommands(this, types);

        /// <summary>
        /// Registers the event handlers that are used to handle commands. This is called when the <see cref="DiscordClient"/> is ready.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="eventArgs">Unused.</param>
        private async Task DiscordClient_ReadyAsync(DiscordClient sender, ReadyEventArgs eventArgs)
        {
            // Run once, only on shard 0
            if (sender.ShardId != 0)
            {
                return;
            }

            // Prevent the event handler from being executed multiple times.
            Client.Ready -= DiscordClient_ReadyAsync;

            await _configureCommands.InvokeAsync(this, new ConfigureCommandsEventArgs(this, CommandManager));
            await CommandManager.RegisterCommandsAsync(this);
            Client.InteractionCreated += DiscordClient_InteractionCreatedAsync;
            Client.MessageCreated += DiscordClient_MessageCreatedAsync;
            _logger.LogInformation("CommandAll Extension is now ready to handle all commands.");
        }

        /// <summary>
        /// Used to parse text commands and execute them.
        /// </summary>
        /// <param name="client">Unused.</param>
        /// <param name="eventArgs">Used to read <see cref="DiscordMessage.Content"/> to parse and execute commands.</param>
        private Task DiscordClient_MessageCreatedAsync(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            // Ensure that text commands are enabled.
            // If the guild is null (private channel), then the CommandFilteringStrategy.AcceptDMs is required.
            // If it is not null, then the flag CommandFilteringStrategy.AcceptGuilds is required.
            if (!FilteringStrategy.HasFlag(CommandFilteringStrategy.AcceptTextCommands | (eventArgs.Guild is null ? CommandFilteringStrategy.AcceptDMs : CommandFilteringStrategy.AcceptGuilds)))
            {
                return Task.CompletedTask;
            }
            // If the debug guild id is set, then only allow commands to be executed in that guild.
            // If a prefix could not be found, then return.
            else if ((DebugGuildId is not null && eventArgs.Guild?.Id != DebugGuildId) || !PrefixParser.TryRemovePrefix(this, eventArgs.Message.Content, out string? commandString))
            {
                return Task.CompletedTask;
            }
            // Try to find the command, resolve to subcommand if needed. Removes the command from the string, leaving the args
            else if (!CommandManager.TryFindCommand(commandString, out Command? command, out string? rawArguments))
            {
                return _commandErrored.InvokeAsync(this, new CommandErroredEventArgs(new CommandContext(eventArgs.Channel, eventArgs.Author, null, eventArgs.Message, eventArgs.Guild, this, null!, CommandInvocationType.TextCommand), new CommandNotFoundException("Command was not found.", commandString)));
            }
            else
            {
                CommandContext context = new(this, command, eventArgs.Message, rawArguments);
                return CommandExecutor.ExecuteAsync(context);
            }
        }

        /// <summary>
        /// Handles applications commands.
        /// </summary>
        /// <param name="client">Unused.</param>
        /// <param name="eventArgs">Used to determine if the interaction is an application command and conditionally executes commands with the provided data.</param>
        private Task DiscordClient_InteractionCreatedAsync(DiscordClient client, InteractionCreateEventArgs eventArgs)
        {
            // Ensure that text commands are enabled.
            // If the guild is null (private channel), then the CommandFilteringStrategy.AcceptDMs is required.
            // If it is not null, then the flag CommandFilteringStrategy.AcceptGuilds is required.
            if (!FilteringStrategy.HasFlag(CommandFilteringStrategy.AcceptSlashCommands | (eventArgs.Interaction.Guild is null ? CommandFilteringStrategy.AcceptDMs : CommandFilteringStrategy.AcceptGuilds)))
            {
                return Task.CompletedTask;
            }
            // If the bot is in debug mode, only execute commands in the debug guild.
            // Also return if the interaction is not an application command.
            // or if the command is not found.
            else if ((DebugGuildId is not null && eventArgs.Interaction.GuildId != DebugGuildId) || eventArgs.Interaction.Type is not InteractionType.ApplicationCommand || !CommandManager.TryFindCommand(eventArgs.Interaction.Data.Id, out Command? command))
            {
                return Task.CompletedTask;
            }
            else
            {
                DiscordInteractionDataOption[] options = eventArgs.Interaction.Data.Options?.ToArray() ?? Array.Empty<DiscordInteractionDataOption>();
                while (options.Any())
                {
                    DiscordInteractionDataOption firstOption = options[0];
                    if (firstOption.Type is not ApplicationCommandOptionType.SubCommandGroup and not ApplicationCommandOptionType.SubCommand)
                    {
                        break;
                    }

                    command = command.Subcommands.First(x => x.Aliases.Contains(firstOption.Name));
                    options = firstOption.Options?.ToArray() ?? Array.Empty<DiscordInteractionDataOption>();
                }

                CommandContext context = new(this, command, eventArgs.Interaction, options);
                return CommandExecutor.ExecuteAsync(context);
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
        public override void Dispose() => throw new NotImplementedException();
    }
}
