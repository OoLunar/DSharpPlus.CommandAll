using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;
using DSharpPlus.CommandAll.Exceptions;
using DSharpPlus.Entities;
using Humanizer;

namespace DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// Slash command metadata for a command parameter.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed record CommandParameterSlashMetadata
    {
        /// <summary>
        /// The localized names of the parameter.
        /// </summary>
        public IReadOnlyDictionary<CultureInfo, string> LocalizedNames { get; init; }

        /// <summary>
        /// The localized descriptions of the parameter.
        /// </summary>
        public IReadOnlyDictionary<CultureInfo, string> LocalizedDescriptions { get; init; }

        /// <summary>
        /// The parameter type.
        /// </summary>
        public ApplicationCommandOptionType OptionType { get; init; }

        /// <summary>
        /// The valid choices for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public IReadOnlyList<DiscordApplicationCommandOptionChoice>? Choices { get; init; }

        /// <summary>
        /// The valid channel types for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.Channel"/> parameters.
        /// </remarks>
        public IReadOnlyList<ChannelType>? ChannelTypes { get; init; }

        /// <summary>
        /// The minimum value for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public object? MinValue { get; init; }

        /// <summary>
        /// The maximum value for the parameter.
        /// </summary>
        /// <remarks>
        /// Can only be used for <see cref="ApplicationCommandOptionType.String"/>, <see cref="ApplicationCommandOptionType.Integer"/> and <see cref="ApplicationCommandOptionType.Number"/> parameters.
        /// </remarks>
        public object? MaxValue { get; init; }

        /// <summary>
        /// The autocomplete provider for the parameter.
        /// </summary>
        public Type? AutoCompleteProvider { get; init; }

        /// <summary>
        /// Whether the parameter is required.
        /// </summary>
        public bool IsRequired { get; init; }

        /// <summary>
        /// The minimum and maximum amount of elements for this parameter.
        /// </summary>
        /// <remarks>
        /// This is DIRECTLY TIED to the <see cref="Enums.CommandParameterFlags.Params"/> flag and is MEANT FOR THE <see langword="params"/> KEYWORD ONLY.
        /// </remarks>
        public ParameterLimitAttribute? ParameterLimitAttribute { get; init; }

        public CommandParameterSlashMetadata(CommandParameterSlashMetadataBuilder builder)
        {
            builder.Verify();
            builder.NormalizeTranslations();
            LocalizedNames = builder.LocalizedNames.AsReadOnly();
            LocalizedDescriptions = builder.LocalizedDescriptions.AsReadOnly();
            OptionType = builder.OptionType ?? throw new PropertyNullException(nameof(builder.OptionType));
            Choices = builder.Choices?.AsReadOnly();
            ChannelTypes = builder.ChannelTypes?.AsReadOnly();
            MinValue = builder.MinValue;
            MaxValue = builder.MaxValue;
            AutoCompleteProvider = builder.AutoCompleteProvider;
            IsRequired = builder.IsRequired;
            ParameterLimitAttribute = builder.ParameterLimitAttribute;
        }

        public override string ToString() => $"{nameof(CommandParameterSlashMetadataBuilder)}: {OptionType.Humanize()}, Is Required: {IsRequired}";
    }
}
