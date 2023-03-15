using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Builders;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Managers
{
    /// <summary>
    /// Manages commands, allowing them to be built, registered, and searched.
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// Searches an assembly for commands and registers them for use.
        /// </summary>
        /// <param name="assembly">The assembly to search for commands.</param>
        void AddCommands(CommandAllExtension extension, Assembly assembly);

        /// <summary>
        /// Searches a collection of types for commands and registers them for use.
        /// </summary>
        /// <param name="types">The types to search for commands.</param>
        void AddCommands(CommandAllExtension extension, params Type[] types);

        /// <summary>
        /// Builds the <see cref="CommandBuilder"/>'s from <see cref="GetCommandBuilders"/> and registers the commands to Discord.
        /// </summary>
        /// <param name="extension">The extension used to grab the instance of <see cref="ArgumentConverterManager"/> and <see cref="DiscordClient"/> from.</param>
        Task RegisterCommandsAsync(CommandAllExtension extension);

        /// <summary>
        /// Gets the current list of <see cref="CommandBuilder"/>'s that are to be built.
        /// </summary>
        /// <returns>A readonly list of <see cref="CommandBuilder"/>s.</returns>
        IReadOnlyList<CommandBuilder> GetCommandBuilders();

        /// <summary>
        /// Gets the current list of <see cref="Command"/>'s that are registered.
        /// </summary>
        /// <returns>A readonly list of <see cref="Command"/>s.</returns>
        IReadOnlyDictionary<string, Command> GetCommands();

        /// <summary>
        /// Attempts to find a command based on the full command string.
        /// </summary>
        /// <param name="fullCommand">The full command string.</param>
        /// <param name="command">The command that was found.</param>
        /// <param name="rawArguments">The raw arguments that were extracted.</param>
        /// <returns>Whether or not a command was found.</returns>
        bool TryFindCommand(string fullCommand, [NotNullWhen(true)] out Command? command, [NotNullWhen(true)] out string? rawArguments);

        /// <summary>
        /// Attempts to find a command based on the <see cref="DiscordApplicationCommand.Id"/>.
        /// </summary>
        bool TryFindCommand(ulong commandId, [NotNullWhen(true)] out Command? commandFound);
    }
}
