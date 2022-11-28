using System;
using System.Diagnostics.CodeAnalysis;
using DSharpPlus;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders.SlashMetadata
{
    /// <summary>
    /// A builder for slash command metadata.
    /// </summary>
    public sealed class CommandSlashMetadataBuilder : ISlashMetadataBuilder
    {
        /// <inheritdoc cref="CommandSlashMetadata.GuildId"/>
        public ulong? GuildId { get; set; }

        /// <inheritdoc cref="CommandSlashMetadata.RequiredPermissions"/>
        public Permissions? RequiredPermissions { get; set; }

        /// <inheritdoc/>
        public CommandSlashMetadataBuilder(CommandAllExtension commandAllExtension) : base(commandAllExtension) { }

        /// <inheritdoc/>
        public override void Verify() { }

        /// <inheritdoc/>
        public override bool TryVerify() => true;

        /// <inheritdoc/>
        public override bool TryVerify([NotNullWhen(false)] out Exception? error)
        {
            error = null;
            return true;
        }
    }
}
