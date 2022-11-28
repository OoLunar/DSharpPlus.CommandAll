using System;
using System.Collections.Generic;
using System.Globalization;
using Humanizer;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;

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

        /// <summary>
        /// Edits the localizations to abide by <see cref="CommandAllExtension.ParameterNamingStrategy"/>. Additionally truncates names to 32 characters and the descriptions to 100 characters.
        /// </summary>
        public virtual void NormalizeTranslations()
        {
            foreach ((CultureInfo culture, string name) in LocalizedNames)
            {
                LocalizedNames[culture] = (CommandAllExtension.ParameterNamingStrategy switch
                {
                    CommandParameterNamingStrategy.SnakeCase => name.Underscore(),
                    CommandParameterNamingStrategy.KebabCase => name.Kebaberize(),
                    CommandParameterNamingStrategy.LowerCase => name.ToLowerInvariant(),
                    _ => throw new NotImplementedException("Unknown command parameter naming strategy.")
                }).Truncate(32, "-");
            }

            foreach ((CultureInfo culture, string description) in LocalizedDescriptions)
            {
                LocalizedDescriptions[culture] = description.Truncate(100, "…");
            }
        }
    }
}
