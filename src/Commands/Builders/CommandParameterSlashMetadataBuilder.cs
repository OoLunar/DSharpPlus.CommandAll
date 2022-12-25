using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Exceptions;
using DSharpPlus.Entities;
using Humanizer;

namespace DSharpPlus.CommandAll.Commands.Builders.SlashMetadata
{
    /// <summary>
    /// A builder for slash parameter metadata.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class CommandParameterSlashMetadataBuilder : SlashMetadataBuilder
    {
        /// <inheritdoc cref="CommandParameterSlashMetadata.OptionType"/>
        public ApplicationCommandOptionType? OptionType { get; set; }

        /// <inheritdoc cref="CommandParameterSlashMetadata.Choices"/>
        public List<DiscordApplicationCommandOptionChoice>? Choices { get; set; }

        /// <inheritdoc cref="CommandParameterSlashMetadata.ChannelTypes"/>
        public List<ChannelType>? ChannelTypes { get; set; }

        /// <inheritdoc cref="CommandParameterSlashMetadata.MinValue"/>
        public object? MinValue { get; set; }

        /// <inheritdoc cref="CommandParameterSlashMetadata.MaxValue"/>
        public object? MaxValue { get; set; }

        /// <inheritdoc cref="CommandParameterSlashMetadata.AutoCompleteProvider"/>
        public Type? AutoCompleteProvider { get; set; }

        /// <inheritdoc cref="CommandParameterSlashMetadata.IsRequired"/>
        public bool IsRequired { get; set; }

        /// <inheritdoc cref="CommandParameterSlashMetadata.ParameterLimitAttribute"/>
        public ParameterLimitAttribute? ParameterLimitAttribute { get; set; }

        /// <inheritdoc/>
        public CommandParameterSlashMetadataBuilder(CommandAllExtension commandAllExtension) : base(commandAllExtension) { }

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
            if (OptionType is ApplicationCommandOptionType.SubCommand or ApplicationCommandOptionType.SubCommandGroup)
            {
                error = new InvalidPropertyStateException(nameof(OptionType), "OptionType cannot be SubCommand or SubCommandGroup!");
                return false;
            }
            else if (ChannelTypes is not null && OptionType is not ApplicationCommandOptionType.Channel)
            {
                error = new InvalidPropertyStateException(nameof(OptionType), "OptionType must be Channel when ChannelTypes is set!");
                return false;
            }

            if (MinValue is not null)
            {
                if (OptionType is ApplicationCommandOptionType.Integer && MinValue is not int)
                {
                    error = new InvalidPropertyStateException(nameof(MinValue), "MinValue must be an int when OptionType is Integer!");
                    return false;
                }
                else if (OptionType is ApplicationCommandOptionType.Number && MinValue is not double)
                {
                    error = new InvalidPropertyStateException(nameof(MinValue), "MinValue must be a double when OptionType is Number!");
                    return false;
                }
                else if (OptionType is not ApplicationCommandOptionType.Integer and not ApplicationCommandOptionType.Number)
                {
                    error = new InvalidPropertyStateException(nameof(MinValue), "MinValue can only be set when OptionType is Integer or Number!");
                    return false;
                }
            }

            if (MaxValue is not null)
            {
                if (OptionType is ApplicationCommandOptionType.Integer && MaxValue is not int)
                {
                    error = new InvalidPropertyStateException(nameof(MaxValue), "MaxValue must be an int when OptionType is Integer!");
                    return false;
                }
                else if (OptionType is ApplicationCommandOptionType.Number && MaxValue is not double)
                {
                    error = new InvalidPropertyStateException(nameof(MaxValue), "MaxValue must be a double when OptionType is Number!");
                    return false;
                }
                else if (OptionType is not ApplicationCommandOptionType.Integer and not ApplicationCommandOptionType.Number)
                {
                    error = new InvalidPropertyStateException(nameof(MaxValue), "MaxValue can only be set when OptionType is Integer or Number!");
                    return false;
                }
            }

            if (AutoCompleteProvider is not null && OptionType is not ApplicationCommandOptionType.String or ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number)
            {
                error = new InvalidPropertyStateException(nameof(AutoCompleteProvider), "AutoCompleteProvider can only be set when OptionType is String, Integer or Number!");
                return false;
            }

            error = null;
            return true;
        }

        public override string ToString() => $"{nameof(CommandParameterSlashMetadataBuilder)}: {(OptionType.HasValue ? OptionType.Value.Humanize() : string.Empty)}, Is Required: {IsRequired}";
        public override bool Equals(object? obj) => obj is CommandParameterSlashMetadataBuilder builder && EqualityComparer<CommandAllExtension>.Default.Equals(CommandAllExtension, builder.CommandAllExtension) && EqualityComparer<Dictionary<CultureInfo, string>>.Default.Equals(LocalizedNames, builder.LocalizedNames) && EqualityComparer<Dictionary<CultureInfo, string>>.Default.Equals(LocalizedDescriptions, builder.LocalizedDescriptions) && OptionType == builder.OptionType && EqualityComparer<List<DiscordApplicationCommandOptionChoice>?>.Default.Equals(Choices, builder.Choices) && EqualityComparer<List<ChannelType>?>.Default.Equals(ChannelTypes, builder.ChannelTypes) && EqualityComparer<object?>.Default.Equals(MinValue, builder.MinValue) && EqualityComparer<object?>.Default.Equals(MaxValue, builder.MaxValue) && EqualityComparer<Type?>.Default.Equals(AutoCompleteProvider, builder.AutoCompleteProvider) && IsRequired == builder.IsRequired && EqualityComparer<ParameterLimitAttribute?>.Default.Equals(ParameterLimitAttribute, builder.ParameterLimitAttribute);
        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(CommandAllExtension);
            hash.Add(LocalizedNames);
            hash.Add(LocalizedDescriptions);

            if (OptionType.HasValue)
            {
                hash.Add(OptionType.Value);
            }

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
