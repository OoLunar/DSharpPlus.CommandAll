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
        public override bool Equals(object? obj) => obj is CommandParameterSlashMetadata metadata && EqualityComparer<IReadOnlyDictionary<CultureInfo, string>>.Default.Equals(LocalizedNames, metadata.LocalizedNames) && EqualityComparer<IReadOnlyDictionary<CultureInfo, string>>.Default.Equals(LocalizedDescriptions, metadata.LocalizedDescriptions) && OptionType == metadata.OptionType && EqualityComparer<IReadOnlyList<DiscordApplicationCommandOptionChoice>?>.Default.Equals(Choices, metadata.Choices) && EqualityComparer<IReadOnlyList<ChannelType>?>.Default.Equals(ChannelTypes, metadata.ChannelTypes) && EqualityComparer<object?>.Default.Equals(MinValue, metadata.MinValue) && EqualityComparer<object?>.Default.Equals(MaxValue, metadata.MaxValue) && EqualityComparer<Type?>.Default.Equals(AutoCompleteProvider, metadata.AutoCompleteProvider) && IsRequired == metadata.IsRequired && EqualityComparer<ParameterLimitAttribute?>.Default.Equals(ParameterLimitAttribute, metadata.ParameterLimitAttribute);
        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(LocalizedNames);
            hash.Add(LocalizedDescriptions);
            hash.Add(OptionType);

            if (Choices is not null)
            {
                hash.Add(Choices);
            }

            if (ChannelTypes is not null)
            {
                hash.Add(ChannelTypes);
            }

            if (MinValue is not null)
            {
                hash.Add(MinValue);
            }

            if (MaxValue is not null)
            {
                hash.Add(MaxValue);
            }

            if (AutoCompleteProvider is not null)
            {
                hash.Add(AutoCompleteProvider);
            }

            hash.Add(IsRequired);

            if (ParameterLimitAttribute is not null)
            {
                hash.Add(ParameterLimitAttribute);
            }

            return hash.ToHashCode();
        }
    }
}
