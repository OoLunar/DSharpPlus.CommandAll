using System;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.CommandAll.Commands.Executors;
using DSharpPlus.CommandAll.Managers;
using DSharpPlus.CommandAll.Parsers;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.CommandAll
{
    /// <summary>
    /// The configuration copied to an instance of <see cref="CommandAllExtension"/>.
    /// </summary>
    public sealed record CommandAllConfiguration
    {
        /// <summary>
        /// The services to be built and used by the <see cref="CommandAllExtension"/>.
        /// </summary>
        public IServiceCollection ServiceCollection { get; set; }

        /// <summary>
        /// The command executor used to run commands. Defaults to <see cref="CommandExecutor"/>.
        /// </summary>
        public ICommandExecutor CommandExecutor { get; set; }

        /// <summary>
        /// The command manager used to store and build commands. Defaults to <see cref="CommandManager"/>.
        /// </summary>
        public ICommandManager CommandManager { get; set; }

        /// <summary>
        /// The argument manager used to store argument converters and assign them to parameters. Defaults to <see cref="ArgumentConverterManager"/>.
        /// </summary>
        public IArgumentConverterManager ArgumentConverterManager { get; set; }

        /// <summary>
        /// The overload parser used to determine which overload a text command should use. Defaults to <see cref="CommandOverloadParser"/>.
        /// </summary>
        public ICommandOverloadParser CommandOverloadParser { get; set; }

        /// <summary>
        /// The prefix parser used to remove the prefixes from a command. Defaults to <see cref="PrefixParser"/>. Supports multiple prefixes and directly mentioning the bot.
        /// </summary>
        public IPrefixParser PrefixParser { get; set; }

        /// <summary>
        /// The parser used to separate a string into multiple arguments. Defaults to <see cref="TextArgumentParser"/>.
        /// </summary>
        public ITextArgumentParser TextArgumentParser { get; set; }

        /// <summary>
        /// The characters used to determine which character is a "quote." Used by the <see cref="TextArgumentParser"/>. Defaults to <c>"</c>, <c>'</c>, <c>«</c>, <c>»</c>, <c>‘</c>, <c>“</c>, <c>„</c> <c>‟</c>.
        /// </summary>
        public char[] QuoteCharacters { get; set; }

        /// <summary>
        /// The guild id used when registering slash commands in debug mode. If not set, slash commands will not be registered.
        /// </summary>
        public ulong? DebugGuildId { get; set; }

        /// <summary>
        /// How long <see cref="Commands.CommandContext.PromptAsync(TextInputComponent[])"/> should wait PER TEXT COMPONENT before timing out. Defaults to 30 seconds per <see cref="TextInputComponent"/>.
        /// </summary>
        public TimeSpan PromptTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc cref="CommandParameterNamingStrategy"/>
        public CommandParameterNamingStrategy ParameterNamingStrategy { get; set; } = CommandParameterNamingStrategy.SnakeCase;

        /// <inheritdoc cref="CommandFilteringStrategy"/>
        /// <remarks>
        /// If <see cref="CommandFilteringStrategy.AcceptAnywhere"/> is specified and <see cref="DebugGuildId"/> is set, <see cref="DebugGuildId"/> still takes priority over the flag.
        /// </remarks>
        public CommandFilteringStrategy FilteringStrategy { get; set; } = CommandFilteringStrategy.AcceptAll;

        /// <summary>
        /// Creates a new instance of <see cref="CommandAllConfiguration"/> to be copied by the <see cref="CommandAllExtension"/>.
        /// </summary>
        /// <param name="serviceDescriptors">The services to be built and used by the <see cref="CommandAllExtension"/>.</param>
        public CommandAllConfiguration(IServiceCollection? serviceDescriptors = null)
        {
            ServiceCollection = serviceDescriptors ?? new ServiceCollection();
            QuoteCharacters = new[] { '"', '\'', '«', '»', '‘', '“', '„', '‟' };

            IServiceProvider serviceProvider = ServiceCollection.BuildServiceProvider();
            ArgumentConverterManager = new ArgumentConverterManager(serviceProvider.GetRequiredService<ILogger<ArgumentConverterManager>>());
            CommandOverloadParser = new CommandOverloadParser(serviceProvider.GetRequiredService<ILogger<CommandOverloadParser>>());
            PrefixParser = new PrefixParser();
            CommandExecutor = new CommandExecutor(serviceProvider.GetRequiredService<ILogger<CommandExecutor>>());
            CommandManager = new CommandManager(serviceProvider.GetRequiredService<ILogger<CommandManager>>());
            TextArgumentParser = new RegexTextParser(this);
        }
    }
}
