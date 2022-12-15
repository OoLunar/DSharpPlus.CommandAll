using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders.SlashMetadata
{
    /// <summary>
    /// A builder for slash subcommand/group metadata.
    /// </summary>
    [DebuggerDisplay("ToString(),nq")]
    public sealed class CommandOverloadSlashMetadataBuilder : SlashMetadataBuilder
    {
        /// <inheritdoc/>
        public CommandOverloadSlashMetadataBuilder(CommandAllExtension commandAllExtension) : base(commandAllExtension) { }

        /// <inheritdoc/>
        public override void Verify()
        {
            if (!TryVerify(out Exception? error))
            {
                throw error;
            }
        }

        /// <inheritdoc/>
        public override bool TryVerify() => TryVerify(out _);


        /// <inheritdoc/>
        public override bool TryVerify([NotNullWhen(false)] out Exception? error)
        {
            error = null;
            return true;
        }

        public override bool Equals(object? obj) => obj is CommandOverloadSlashMetadataBuilder builder && EqualityComparer<CommandAllExtension>.Default.Equals(CommandAllExtension, builder.CommandAllExtension) && EqualityComparer<Dictionary<CultureInfo, string>>.Default.Equals(LocalizedNames, builder.LocalizedNames) && EqualityComparer<Dictionary<CultureInfo, string>>.Default.Equals(LocalizedDescriptions, builder.LocalizedDescriptions);
        public override int GetHashCode() => HashCode.Combine(CommandAllExtension, LocalizedNames, LocalizedDescriptions);
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            if (LocalizedNames.TryGetValue(CultureInfo.CurrentCulture, out string? name))
            {
                stringBuilder.Append($"Name: {name}");
            }

            if (LocalizedDescriptions.TryGetValue(CultureInfo.CurrentCulture, out string? description))
            {
                // In case the name is null, we don't want to start with a comma.
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.Append($"Description: {description}");
            }

            return stringBuilder.ToString();
        }
    }
}
