using System.Collections.Generic;
using System.Globalization;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    /// <summary>
    /// A builder for slash command metadata.
    /// </summary>
    public interface ISlashMetadataBuilder : IBuilder
    {
        /// <summary>
        /// The localized names of the command, subcommand or command parameter.
        /// </summary>
        Dictionary<CultureInfo, string> LocalizedNames { get; set; }

        /// <summary>
        /// The localized descriptions of the command, subcommand or command parameter.
        /// </summary>
        Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; }
    }
}
