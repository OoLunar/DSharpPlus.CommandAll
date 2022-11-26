using System;
using System.Collections.Generic;
using System.Globalization;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class CommandParameterSlashMetadata
    {
        public Dictionary<CultureInfo, string> LocalizedNames { get; set; } = new();
        public Dictionary<CultureInfo, string> LocalizedDescriptions { get; set; } = new();
        public ApplicationCommandOptionType OptionType { get; set; }
        public List<DiscordApplicationCommandOptionChoice>? Choices { get; set; }
        public List<ChannelType>? ChannelTypes { get; set; }
        public object? MinValue { get; set; }
        public object? MaxValue { get; set; }
        public Type? AutoCompleteProvider { get; set; }

        public CommandParameterSlashMetadata(CommandParameterSlashMetadataBuilder builder)
        {
            builder.Verify();
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
