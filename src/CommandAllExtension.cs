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

        public readonly ICommandManager CommandManager;
        public readonly IArgumentConverterManager ArgumentConverterManager;
        public readonly ICommandOverloadParser CommandOverloadParser;

        public readonly ICommandExecutor CommandExecutor;
        public readonly IPrefixParser PrefixParser;
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
            ServiceProvider = configuration.ServiceCollection.AddSingleton(configuration).AddSingleton(this).BuildServiceProvider();
            CommandManager = configuration.CommandManager;
            ArgumentConverterManager = configuration.ArgumentConverterManager;
            ArgumentConverterManager.AddArgumentConverters(typeof(CommandAllExtension).Assembly.DefinedTypes.Where(type => type.Namespace == "OoLunar.DSharpPlus.CommandAll.Converters"));
            CommandOverloadParser = configuration.CommandOverloadParser;
            CommandExecutor = configuration.CommandExecutor;
            PrefixParser = configuration.PrefixParser;
            TextArgumentParser = configuration.TextArgumentParser;
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
        /// Delegates the <see cref="CommandContext"/> to the <see cref="ICommandExecutor"/> provided by the <see cref="CommandAllConfiguration"/>.
        /// </summary>
        /// <param name="context">The context provided.</param>
        /// <returns>Whether the command executed successfully without any uncaught exceptions.</returns>
        public Task<bool> ExecuteCommandAsync(CommandContext context) => CommandExecutor.ExecuteAsync(context);

        /// <summary>
        /// Registers the event handlers that are used to handle commands. This is called when the <see cref="DiscordClient"/> is ready.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="eventArgs">Unused.</param>
        private Task DiscordClient_ReadyAsync(DiscordClient sender, ReadyEventArgs eventArgs)
        {
            DiscordClient.Ready -= DiscordClient_ReadyAsync;

            // Disable the overloads which don't have argument converters for a specific parameter type.
            foreach (CommandOverload overload in CommandManager.Commands.Values.SelectMany(x => x.Overloads))
            {
                if (overload.Flags.HasFlag(CommandFlags.Disabled))
                {
                    continue;
                }

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
            if (!PrefixParser.TryRemovePrefix(eventArgs.Message.Content, out string? commandString) // Remove the prefix
                || !CommandManager.TryFindCommand(ref commandString, out Command? command) // Try to find the command, resolve to subcommand if needed. Removes the command from the string, leaving the args
                || command.Flags.HasFlag(CommandFlags.Disabled)) // Check if the command is disabled due to argument converter issues.
            {
                return;
            }
            await ExecuteCommandAsync(new CommandContext(this, command, eventArgs.Message, commandString));
        }

        /// <summary>
        /// Handles applications commands.
        /// </summary>
        /// <param name="client">Unused.</param>
        /// <param name="eventArgs">Used to determine if the interaction is an application command and conditionally executes commands with the provided data.</param>
        private Task DiscordClient_InteractionCreatedAsync(DiscordClient client, InteractionCreateEventArgs eventArgs) => eventArgs.Interaction.Type != InteractionType.ApplicationCommand ? Task.CompletedTask : Task.CompletedTask;
    }
}
