using System;
using System.Diagnostics.CodeAnalysis;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders.SlashMetadata
{
    /// <summary>
    /// A builder for slash subcommand/group metadata.
    /// </summary>
    public sealed class CommandOverloadSlashMetadataBuilder : ISlashMetadataBuilder
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
    }
}
