using System;
using System.Collections.Generic;
using System.Globalization;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;

namespace OoLunar.DSharpPlus.CommandAll.Commands.System.SlashMetadata
{
    /// <summary>
    /// Slash command metadata for a command parameter.
    /// </summary>
    public sealed class CommandParameterSlashMetadata
    {
        /// <summary>
        /// The localized names of the parameter.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizedNames { get; set; } = new();

        /// <summary>
        /// The localized descriptions of the parameter.
        /// </summary>
        public Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; } = new();

        /// <summary>
        /// The parameter type.
        /// </summary>
        public ApplicationCommandOptionType OptionType { get; set; }

        /// <summary>
        /// The valid choices for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public List<DiscordApplicationCommandOptionChoice>? Choices { get; set; }

        /// <summary>
        /// The valid channel types for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.Channel"/> parameters.
        /// </remarks>
        public List<ChannelType>? ChannelTypes { get; set; }

        /// <summary>
        /// The minimum value for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public object? MinValue { get; set; }

        /// <summary>
        /// The maximum value for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public object? MaxValue { get; set; }

        /// <summary>
        /// The autocomplete provider for the parameter.
        /// </summary>
        public Type? AutoCompleteProvider { get; set; }

        public CommandParameterSlashMetadata(CommandParameterSlashMetadataBuilder builder)
        {
            builder.Verify();
            builder.NormalizeTranslations();
            LocalizedNames = builder.LocalizedNames;
            LocalizedDescriptions = builder.LocalizedDescriptions;
            OptionType = builder.OptionType!.Value;
            Choices = builder.Choices;
            ChannelTypes = builder.ChannelTypes;
            MinValue = builder.MinValue;
            MaxValue = builder.MaxValue;
            AutoCompleteProvider = builder.AutoCompleteProvider;
        }
    }
}
