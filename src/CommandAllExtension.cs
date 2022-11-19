using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Executors;
using OoLunar.DSharpPlus.CommandAll.Managers;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll
{
    public sealed class CommandAllExtension : BaseExtension
    {
        public readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// The client associated with this extension.
        /// </summary>
        public DiscordClient DiscordClient { get; private set; } = null!;

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
            // Add the default converters to the argument converter manager.
            ArgumentConverterManager.AddArgumentConverters(typeof(CommandAllExtension).Assembly.DefinedTypes.Where(type => type.Namespace == "OoLunar.DSharpPlus.CommandAll.Converters"));
            CommandOverloadParser = configuration.CommandOverloadParser;
            CommandExecutor = configuration.CommandExecutor;
            PrefixParser = configuration.PrefixParser;
            TextArgumentParser = configuration.TextArgumentParser;
            // Attempt to get the user defined logging, otherwise setup a null logger since the D#+ Default Logger is internal.
            _logger = ServiceProvider.GetService<ILogger<CommandAllExtension>>() ?? NullLogger<CommandAllExtension>.Instance;
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
            else if (DiscordClient is not null)
            {
                throw new InvalidOperationException("CommandAll Extension is already initialized.");
            }

            DiscordClient = client;

            // If the client has already been initialized, register the event handlers.
            if (DiscordClient.Guilds.Count != 0)
            {
                DiscordClient_ReadyAsync(DiscordClient, null!);
            }
            else
            {
                DiscordClient.Ready += DiscordClient_ReadyAsync;
            }
        }

        /// <summary>
        /// Registers the event handlers that are used to handle commands. This is called when the <see cref="DiscordClient"/> is ready.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="eventArgs">Unused.</param>
        private Task DiscordClient_ReadyAsync(DiscordClient sender, ReadyEventArgs eventArgs)
        {
            // Prevent the event handler from being executed multiple times.
            DiscordClient.Ready -= DiscordClient_ReadyAsync;

            // TODO: Parallelize this?
            // Disable the overloads which don't have argument converters for a specific parameter type.
            foreach (CommandOverload overload in CommandManager.Commands.Values.SelectMany(x => x.Overloads))
            {
                // If the overload was previously disabled, just skip over it.
                if (overload.Flags.HasFlag(CommandFlags.Disabled))
                {
                    continue;
                }

                // Iterate over the parameters, checking if there are missing argument converters. If there is, disable the overload and log an error.
                foreach (CommandParameter parameter in overload.Parameters)
                {
                    if (parameter.ArgumentConverterType is null && !parameter.Type.IsAssignableFrom(typeof(CommandContext)) && !overload.Flags.HasFlag(CommandFlags.Disabled))
                    {
                        _logger.LogError("Method {Method} has parameter {Parameter} of type {ParameterType} that does not have an argument converter. Disabling the overload.", overload.Method, parameter.Name, parameter.Type);
                        overload.Flags |= CommandFlags.Disabled;
                        break;
                    }
                }
            }

            DiscordClient.MessageCreated += DiscordClient_MessageCreatedAsync;
            DiscordClient.InteractionCreated += DiscordClient_InteractionCreatedAsync;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Used to parse text commands and execute them.
        /// </summary>
        /// <param name="client">Unused.</param>
        /// <param name="eventArgs">Used to read <see cref="DiscordMessage.Content"/> to parse and execute commands.</param>
        private async Task DiscordClient_MessageCreatedAsync(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            // Attempt to parse the prefix, find the command, check if it's disabled, and create a new command context to pass to the command executor.
            if (!PrefixParser.TryRemovePrefix(eventArgs.Message.Content, out string? commandString) // Remove the prefix
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
    }
}
