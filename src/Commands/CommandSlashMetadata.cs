using System;
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
    public sealed class CommandSlashMetadata
    {
        /// <summary>
        /// The guild ID to register the command for. If null, the command will be registered globally.
        /// </summary>
        /// <remarks>
        /// Only valid for commands, not subcommands.
        /// </remarks>
        public readonly ulong? GuildId;

        /// <summary>
        /// The required permissions for the command to be executed.
        /// </summary>
        /// <remarks>
        /// Only valid for commands, not subcommands.
        /// </remarks>
        public readonly Permissions? RequiredPermissions;

        /// <summary>
        /// The localized names for the command.
        /// </summary>
        public readonly IReadOnlyDictionary<CultureInfo, string> LocalizedNames;

        /// <summary>
        /// The localized descriptions for the command.
        /// </summary>
        public readonly IReadOnlyDictionary<CultureInfo, string> LocalizedDescriptions;

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

        public override bool Equals(object? obj) => obj is CommandSlashMetadata metadata && GuildId == metadata.GuildId && RequiredPermissions == metadata.RequiredPermissions && EqualityComparer<IReadOnlyDictionary<CultureInfo, string>>.Default.Equals(LocalizedNames, metadata.LocalizedNames) && EqualityComparer<IReadOnlyDictionary<CultureInfo, string>>.Default.Equals(LocalizedDescriptions, metadata.LocalizedDescriptions);
        public override int GetHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(LocalizedNames);
            hashCode.Add(LocalizedDescriptions);

            if (GuildId.HasValue)
            {
                hashCode.Add(GuildId.Value);
            }

            if (RequiredPermissions.HasValue)
            {
                hashCode.Add(RequiredPermissions.Value);
            }

            return hashCode.ToHashCode();
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
