using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DSharpPlus;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    /// <summary>
    /// A builder for slash command metadata.
    /// </summary>
    public sealed class CommandSlashMetadataBuilder : ISlashMetadataBuilder
    {
        /// <inheritdoc cref="CommandSlashMetadata.IsSubcommand"/>
        public bool IsSubcommand { get; set; }

        /// <inheritdoc cref="CommandSlashMetadata.GuildId"/>
        public ulong? GuildId { get; set; }

        /// <inheritdoc cref="CommandSlashMetadata.RequiredPermissions"/>
        public Permissions? RequiredPermissions { get; set; }

        /// <inheritdoc cref="CommandSlashMetadata.LocalizedNames"/>
        public Dictionary<CultureInfo, string> LocalizedNames { get; set; } = new();

        /// <inheritdoc cref="CommandSlashMetadata.LocalizedDescriptions"/>
        public Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSlashMetadataBuilder"/> class.
        /// </summary>
        /// <param name="isSubcommand">Whether the command is a subcommand.</param>
        public CommandSlashMetadataBuilder(bool isSubcommand) => IsSubcommand = isSubcommand;

        /// <inheritdoc/>
        public void Verify() { }

        /// <inheritdoc/>
        public bool TryVerify() => true;

        /// <inheritdoc/>
        public bool TryVerify([NotNullWhen(false)] out Exception? error)
        {
            error = null;
            return true;
        }
    }
}
