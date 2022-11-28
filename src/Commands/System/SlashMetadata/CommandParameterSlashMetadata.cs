using System;
using System.Collections.Generic;
using System.Globalization;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Attributes;
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
        public readonly IReadOnlyDictionary<CultureInfo, string> LocalizedNames;

        /// <summary>
        /// The localized descriptions of the parameter.
        /// </summary>
        public readonly IReadOnlyDictionary<CultureInfo, string> LocalizedDescriptions;

        /// <summary>
        /// The parameter type.
        /// </summary>
        public readonly ApplicationCommandOptionType OptionType;

        /// <summary>
        /// The valid choices for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public readonly IReadOnlyList<DiscordApplicationCommandOptionChoice>? Choices;

        /// <summary>
        /// The valid channel types for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.Channel"/> parameters.
        /// </remarks>
        public readonly IReadOnlyList<ChannelType>? ChannelTypes;

        /// <summary>
        /// The minimum value for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public readonly object? MinValue;

        /// <summary>
        /// The maximum value for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public readonly object? MaxValue;

        /// <summary>
        /// The autocomplete provider for the parameter.
        /// </summary>
        public readonly Type? AutoCompleteProvider;

        /// <summary>
        /// Whether the parameter is required.
        /// </summary>
        public readonly bool IsRequired;

        /// <summary>
        /// The minimum and maximum amount of elements for this parameter.
        /// </summary>
        /// <remarks>
        /// This is DIRECTLY TIED to the <see cref="Enums.CommandParameterFlags.Params"/> flag and is MEANT FOR THE `params` KEYWORD ONLY.
        /// </remarks>
        public readonly ParameterLimitAttribute? ParameterLimitAttribute;

        public CommandParameterSlashMetadata(CommandParameterSlashMetadataBuilder builder)
        {
            builder.Verify();
            builder.NormalizeTranslations();
            LocalizedNames = builder.LocalizedNames.AsReadOnly();
            LocalizedDescriptions = builder.LocalizedDescriptions.AsReadOnly();
            OptionType = builder.OptionType!.Value;
            Choices = builder.Choices?.AsReadOnly();
            ChannelTypes = builder.ChannelTypes?.AsReadOnly();
            MinValue = builder.MinValue;
            MaxValue = builder.MaxValue;
            AutoCompleteProvider = builder.AutoCompleteProvider;
            IsRequired = builder.IsRequired;
            ParameterLimitAttribute = builder.ParameterLimitAttribute;
        }
    }
}
