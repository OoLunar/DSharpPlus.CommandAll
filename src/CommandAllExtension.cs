using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Emzi0767.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Commands.Executors;
using OoLunar.DSharpPlus.CommandAll.EventArgs;
using OoLunar.DSharpPlus.CommandAll.Managers;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll
{
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

        public event AsyncEventHandler<CommandAllExtension, CommandExecutedEventArgs> CommandExecuted { add => _commandExecuted.Register(value); remove => _commandExecuted.Unregister(value); }
        private readonly AsyncEvent<CommandAllExtension, CommandExecutedEventArgs> _commandExecuted = new("COMMANDALL_COMMAND_EXECUTED", TimeSpan.MaxValue, EverythingWentWrongErrorHandler);

        public event AsyncEventHandler<CommandAllExtension, ConfigureCommandsEventArgs> ConfigureCommands { add => _configureCommands.Register(value); remove => _configureCommands.Unregister(value); }
        private readonly AsyncEvent<CommandAllExtension, ConfigureCommandsEventArgs> _configureCommands = new("COMMANDALL_CONFIGURE_COMMANDS", TimeSpan.MaxValue, EverythingWentWrongErrorHandler);

        public event AsyncEventHandler<CommandAllExtension, CommandErroredEventArgs> CommandErrored { add => _commandErrored.Register(value); remove => _commandErrored.Unregister(value); }
        private readonly AsyncEvent<CommandAllExtension, CommandErroredEventArgs> _commandErrored = new("COMMANDALL_COMMAND_ERRORED", TimeSpan.MaxValue, EverythingWentWrongErrorHandler);

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

            // Add the default converters to the argument converter manager.
            ArgumentConverterManager.AddArgumentConverters(typeof(CommandAllExtension).Assembly.DefinedTypes.Where(type => type.Namespace == "OoLunar.DSharpPlus.CommandAll.Converters"));

            // Attempt to get the user defined logging, otherwise setup a null logger since the D#+ Default Logger is internal.
            _logger = ServiceProvider.GetService<ILogger<CommandAllExtension>>() ?? NullLogger<CommandAllExtension>.Instance;
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

        /// <summary>
        /// Registers the event handlers that are used to handle commands. This is called when the <see cref="Client"/> is ready.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="eventArgs">Unused.</param>
        private async Task DiscordClient_ReadyAsync(DiscordClient sender, ReadyEventArgs eventArgs)
        {
            // Prevent the event handler from being executed multiple times.
            Client.Ready -= DiscordClient_ReadyAsync;

            await _configureCommands.InvokeAsync(this, new ConfigureCommandsEventArgs(this, CommandManager));
            foreach (CommandBuilder command in CommandManager.CommandBuilders.Values)
            {
                SaturateParametersRecursively(command);
            }
            CommandManager.BuildCommands();

            Client.MessageCreated += DiscordClient_MessageCreatedAsync;
            Client.InteractionCreated += DiscordClient_InteractionCreatedAsync;
            _logger.LogInformation("CommandAll Extension is now ready to handle all commands.");
        }

        /// <summary>
        /// Used to parse text commands and execute them.
        /// </summary>
        /// <param name="client">Unused.</param>
        /// <param name="eventArgs">Used to read <see cref="DiscordMessage.Content"/> to parse and execute commands.</param>
        private async Task DiscordClient_MessageCreatedAsync(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            // Attempt to parse the prefix, find the command, check if it's disabled, and create a new command context to pass to the command executor.
            if (!PrefixParser.TryRemovePrefix(this, eventArgs.Message.Content, out string? commandString) // Remove the prefix
                || !CommandManager.TryFindCommand(commandString, out string? rawArguments, out Command? command) // Try to find the command, resolve to subcommand if needed. Removes the command from the string, leaving the args
                || command.Flags.HasFlag(CommandFlags.Disabled)) // Check if the command is disabled due to argument converter issues.
            {
                return;
            }

            await CommandExecutor.ExecuteAsync(new CommandContext(this, command, eventArgs.Message, rawArguments));
        }

        /// <summary>
        /// Handles applications commands.
        /// </summary>
        /// <param name="client">Unused.</param>
        /// <param name="eventArgs">Used to determine if the interaction is an application command and conditionally executes commands with the provided data.</param>
        private Task DiscordClient_InteractionCreatedAsync(DiscordClient client, InteractionCreateEventArgs eventArgs) =>
            eventArgs.Interaction.Type is not InteractionType.ApplicationCommand || !CommandManager.TryFindCommand(eventArgs.Interaction.Data.Name, out _, out Command? command)
                ? Task.CompletedTask
                : CommandExecutor.ExecuteAsync(new CommandContext(this, command, eventArgs.Interaction));

        private static void EverythingWentWrongErrorHandler<TArgs>(AsyncEvent<CommandAllExtension, TArgs> asyncEvent, Exception error, AsyncEventHandler<CommandAllExtension, TArgs> handler, CommandAllExtension sender, TArgs eventArgs) where TArgs : AsyncEventArgs => sender._logger.LogError(error, "Event handler '{Method}' for event {AsyncEvent} threw an unhandled exception.", handler.Method, asyncEvent.Name);

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
