using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;

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
            stringBuilder.AppendFormat($"{nameof(CommandSlashMetadata)}: ");
            if (GuildId.HasValue)
            {
                stringBuilder.Append($"{nameof(GuildId)}: {GuildId.Value}, ");
            }

            if (RequiredPermissions.HasValue)
            {
                stringBuilder.Append($"{nameof(RequiredPermissions)}: {RequiredPermissions.Value}, ");
            }

            stringBuilder.Append($"{nameof(LocalizedNames)}: {LocalizedNames.Count:N0}, {nameof(LocalizedDescriptions)}: {LocalizedDescriptions.Count:N0}");
            return stringBuilder.ToString();
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
    }
}
