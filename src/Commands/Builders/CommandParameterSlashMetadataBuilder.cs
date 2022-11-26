using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    public sealed class CommandParameterSlashMetadataBuilder : ISlashMetadataBuilder
    {
        public Dictionary<CultureInfo, string> LocalizedNames { get; set; } = new();
        public Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; } = new();
        public ApplicationCommandOptionType? OptionType { get; set; }
        public List<DiscordApplicationCommandOptionChoice>? Choices { get; set; }
        public List<ChannelType>? ChannelTypes { get; set; }
        public object? MinValue { get; set; }
        public object? MaxValue { get; set; }
        public Type? AutoCompleteProvider { get; set; }

        public void Verify()
        {
            if (!TryVerify(out Exception? error))
            {
                throw error;
            }
        }

        public bool TryVerify() => TryVerify(out _);
        public bool TryVerify([NotNullWhen(false)] out Exception? error)
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
    }
}
