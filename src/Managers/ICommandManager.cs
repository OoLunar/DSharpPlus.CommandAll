using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    /// <summary>
    /// Manages commands, allowing them to be built, registered, and searched.
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// The commands registered to this manager.
        /// </summary>
        /// <remarks>
        /// An empty <see cref="IReadOnlyDictionary{TKey, TValue}"/> until <see cref="BuildCommands"/> is called.
        /// </remarks>
        IReadOnlyDictionary<string, Command> Commands { get; }

        /// <summary>
        /// The commands registered to this manager.
        /// </summary>
        /// <remarks>
        /// An empty <see cref="IReadOnlyDictionary{TKey, TValue}"/> until <see cref="DiscordClient.Ready"/> is called and <see cref="DiscordClient.BulkOverwriteGlobalApplicationCommandsAsync(IEnumerable{DiscordApplicationCommand})"/> has returned.
        /// </remarks>
        IReadOnlyDictionary<ulong, Command> SlashCommandsIndex { get; }

        /// <summary>
        /// The dictionary that contains builders for commands.
        /// </summary>
        Dictionary<string, CommandBuilder> CommandBuilders { get; set; }

        /// <summary>
        /// Searches a singular type for commands and adds them to the <see cref="CommandBuilders"/> dictionary.
        /// </summary>
        /// <typeparam name="T">The type to search for commands.</typeparam>
        void AddCommand<T>(CommandAllExtension extension) where T : BaseCommand;

        /// <inheritdoc cref="AddCommand{T}"/>
        /// <param name="type">The type to search for commands.</param>
        void AddCommand(CommandAllExtension extension, Type type);

        /// <summary>
        /// Searches an assembly for commands and adds them to the <see cref="CommandBuilders"/> dictionary.
        /// </summary>
        /// <param name="assembly">The assembly to search for commands.</param>
        void AddCommands(CommandAllExtension extension, Assembly assembly);

        /// <summary>
        /// Searches a collection of types for commands and adds them to the <see cref="CommandBuilders"/> dictionary.
        /// </summary>
        /// <param name="types">The types to search for commands.</param>
        void AddCommands(CommandAllExtension extension, IEnumerable<Type> types);

        /// <summary>
        /// Builds the commands from the <see cref="CommandBuilders"/> dictionary, copying the built commands to the <see cref="Commands"/> dictionary.
        /// </summary>
        void BuildCommands();

        /// <summary>
        /// Iterates over the <see cref="Commands"/> values converts them to slash commands.
        /// </summary>
        IEnumerable<DiscordApplicationCommand> BuildSlashCommands();

        /// <summary>
        /// Searches for a command by name or aliases, removing the <see cref="Command.FullName"/> from the start of the string.
        /// </summary>
        /// <param name="commandString">The string that contains both the command name and command arguments.</param>
        /// <param name="rawArguments">The arguments that were extracted from <paramref name="commandString"/>.</param>
        /// <param name="command">The command that was found.</param>
        /// <returns>The command that was found, or null if no command was found.</returns>
        bool TryFindCommand(string commandString, [NotNullWhen(true)] out string? rawArguments, [NotNullWhen(true)] out Command? command);

        /// <inheritdoc cref="TryFindCommand(string, out string?, out Command?)"/>
        /// <param name="commandBuilder">The command builder that was found.</param>
        bool TryFindCommand(string commandString, [NotNullWhen(true)] out string? rawArguments, [NotNullWhen(true)] out CommandBuilder? commandBuilder);

        /// <summary>
        /// Fills the <see cref="SlashCommandsIndex"/> dictionary with the commands from <see cref="Commands"/>.
        /// </summary>
        /// <param name="commands">The commands to fill the index with.</param>
        void RegisterSlashCommands(IEnumerable<DiscordApplicationCommand> commands);
    }
}
