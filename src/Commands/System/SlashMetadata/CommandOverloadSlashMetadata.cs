using System.Collections.Generic;
using System.Globalization;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;

namespace OoLunar.DSharpPlus.CommandAll.Commands.System.SlashMetadata
{
    /// <summary>
    /// The metadata used when registering a command as a slash command.
    /// </summary>
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
    }
}
