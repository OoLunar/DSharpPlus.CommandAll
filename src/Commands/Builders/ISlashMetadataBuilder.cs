using System.Collections.Generic;
using System.Globalization;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    /// <summary>
    /// A builder for slash command metadata.
    /// </summary>
    public abstract class ISlashMetadataBuilder : Builder
    {
        /// <summary>
        /// The localized names of the command, subcommand or command parameter.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizedNames { get; set; } = new();

        /// <summary>
        /// The localized descriptions of the command, subcommand or command parameter.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; } = new();

        /// <inheritdoc/>
        protected ISlashMetadataBuilder(CommandAllExtension commandAllExtension) : base(commandAllExtension) { }
    }
}
