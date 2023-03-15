using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// The metadata used when registering a command as a slash command.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed record CommandSlashMetadata
    {
        /// <summary>
        /// The guild ID to register the command for. If null, the command will be registered globally.
        /// </summary>
        /// <remarks>
        /// Only valid for commands, not subcommands.
        /// </remarks>
        public ulong? GuildId { get; init; }

        /// <summary>
        /// The required permissions for the command to be executed.
        /// </summary>
        /// <remarks>
        /// Only valid for commands, not subcommands.
        /// </remarks>
        public Permissions? RequiredPermissions { get; init; }

        /// <summary>
        /// The localized names for the command.
        /// </summary>
        public IReadOnlyDictionary<CultureInfo, string> LocalizedNames { get; init; }

        /// <summary>
        /// The localized descriptions for the command.
        /// </summary>
        public IReadOnlyDictionary<CultureInfo, string> LocalizedDescriptions { get; init; }

        /// <summary>
        /// The id of the <see cref="Command"/> when it's registered to Discord in it's <see cref="DiscordApplicationCommand"/> form.
        /// </summary>
        /// <remarks>
        /// Only available on top level commands and after the <see cref="DiscordClient.GuildDownloadCompleted"/> event has fired.
        /// </remarks>
        public ulong? ApplicationCommandId { get; set; }

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
            LocalizedNames = builder.LocalizedNames.AsReadOnly();
            LocalizedDescriptions = builder.LocalizedDescriptions.AsReadOnly();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            if (GuildId.HasValue)
            {
                stringBuilder.AppendFormat("Guild Id: {0}", GuildId.Value);
            }

            if (RequiredPermissions.HasValue)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.AppendFormat("Required Permissions: {0}");
            }

            if (LocalizedNames.Count != 0)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.AppendFormat("Localized Names: {0}", LocalizedNames.Count);
            }

            if (LocalizedDescriptions.Count != 0)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.AppendFormat("Localized Descriptions: {0}", LocalizedDescriptions.Count);
            }

            return stringBuilder.ToString();
        }
    }
}
