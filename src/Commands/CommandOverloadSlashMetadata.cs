using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;

namespace DSharpPlus.CommandAll.Commands.System.SlashMetadata
{
    /// <summary>
    /// The metadata used when registering a command as a slash command.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class CommandOverloadSlashMetadata
    {
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
        public CommandOverloadSlashMetadata(CommandOverloadSlashMetadataBuilder builder)
        {
            builder.Verify();
            builder.NormalizeTranslations();
            LocalizedNames = builder.LocalizedNames.AsReadOnly();
            LocalizedDescriptions = builder.LocalizedDescriptions.AsReadOnly();
        }

        public override string ToString() => $"{nameof(CommandOverloadSlashMetadata)}: {nameof(LocalizedNames)}: {LocalizedNames.Count:N0}, {nameof(LocalizedDescriptions)}: {LocalizedDescriptions.Count:N0}";
        public override bool Equals(object? obj) => obj is CommandOverloadSlashMetadata metadata && EqualityComparer<IReadOnlyDictionary<CultureInfo, string>>.Default.Equals(LocalizedNames, metadata.LocalizedNames) && EqualityComparer<IReadOnlyDictionary<CultureInfo, string>>.Default.Equals(LocalizedDescriptions, metadata.LocalizedDescriptions);
        public override int GetHashCode() => HashCode.Combine(LocalizedNames, LocalizedDescriptions);
    }
}
