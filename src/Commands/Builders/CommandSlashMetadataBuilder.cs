using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace DSharpPlus.CommandAll.Commands.Builders.SlashMetadata
{
    /// <summary>
    /// A builder for slash command metadata.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class CommandSlashMetadataBuilder : SlashMetadataBuilder
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

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            if (GuildId.HasValue)
            {
                stringBuilder.Append($"GuildId: {GuildId.Value}");
            }

            if (RequiredPermissions.HasValue)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append($"RequiredPermissions: {RequiredPermissions.Value}");
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append(", ");
            }

            stringBuilder.Append(base.ToString());
            return stringBuilder.ToString();
        }

        public override bool Equals(object? obj) => obj is CommandSlashMetadataBuilder builder && EqualityComparer<CommandAllExtension>.Default.Equals(CommandAllExtension, builder.CommandAllExtension) && EqualityComparer<Dictionary<CultureInfo, string>>.Default.Equals(LocalizedNames, builder.LocalizedNames) && EqualityComparer<Dictionary<CultureInfo, string>>.Default.Equals(LocalizedDescriptions, builder.LocalizedDescriptions) && GuildId == builder.GuildId && RequiredPermissions == builder.RequiredPermissions;
        public override int GetHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(CommandAllExtension);
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
