using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using DSharpPlus.CommandAll.Commands.Enums;
using Humanizer;

namespace DSharpPlus.CommandAll.Commands.Builders
{
    /// <summary>
    /// A builder for slash command metadata.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public abstract class SlashMetadataBuilder : Builder
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
        protected SlashMetadataBuilder(CommandAllExtension commandAllExtension) : base(commandAllExtension) { }

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

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            if (LocalizedNames.Count != 0)
            {
                stringBuilder.AppendFormat("Localized Names: {0}", LocalizedNames.Count);
            }

            if (LocalizedDescriptions.Count != 0)
            {
                if (stringBuilder.Length != 0)
                {
                    stringBuilder.Append(", ");
                }

                stringBuilder.AppendFormat("Localized Descriptions: {0}", LocalizedDescriptions.Count);
            }

            return stringBuilder.ToString();
        }
    }
}
