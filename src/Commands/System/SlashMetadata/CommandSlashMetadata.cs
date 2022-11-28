using System.Collections.Generic;
using System.Globalization;
using DSharpPlus;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;

namespace OoLunar.DSharpPlus.CommandAll.Commands.System.SlashMetadata
{
    /// <summary>
    /// The metadata used when registering a command as a slash command.
    /// </summary>
    public sealed class CommandSlashMetadata
    {
        /// <summary>
        /// The guild ID to register the command for. If null, the command will be registered globally.
        /// </summary>
        /// <remarks>
        /// Only valid for commands, not subcommands.
        /// </remarks>
        public ulong? GuildId { get; set; }

        /// <summary>
        /// The required permissions for the command to be executed.
        /// </summary>
        /// <remarks>
        /// Only valid for commands, not subcommands.
        /// </remarks>
        public Permissions? RequiredPermissions { get; set; }

        /// <summary>
        /// The localized names for the command.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizedNames { get; set; } = new();

        /// <summary>
        /// The localized descriptions for the command.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; } = new();

        /// <summary>
        /// Creates a new instance of <see cref="CommandSlashMetadata"/>.
        /// </summary>
        /// <param name="builder">The builder to create the metadata from.</param>
        public CommandSlashMetadata(CommandSlashMetadataBuilder builder)
        {
            builder.Verify();
            builder.NormalizeTranslations();
            GuildId = builder.GuildId;
            RequiredPermissions = builder.RequiredPermissions;
            LocalizedNames = builder.LocalizedNames;
            LocalizedDescriptions = builder.LocalizedDescriptions;
        }
    }
}
