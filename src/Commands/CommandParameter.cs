using System;
using System.Linq;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using Humanizer;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class CommandParameter
    {
        public readonly string Name;
        public readonly string Description;
        public readonly CommandOverload Overload;
        public readonly ParameterInfo ParameterInfo;
        public readonly CommandParameterFlags Flags;
        public readonly object? DefaultValue;
        public readonly Type ArgumentConverterType;
        public readonly CommandParameterSlashMetadata SlashMetadata;
        public Type Type => ParameterInfo.ParameterType;

        public CommandParameter(CommandParameterBuilder builder, CommandOverload overload)
        {
            builder.Verify();
            if (builder.ArgumentConverterType is null && !overload.Flags.HasFlag(CommandOverloadFlags.Disabled))
            {
                throw new PropertyNullException(nameof(builder.ArgumentConverterType));
            }

            Name = builder.Name;
            Description = builder.Description;
            Overload = overload ?? throw new ArgumentNullException(nameof(overload));
            ParameterInfo = builder.ParameterInfo!;
            Flags = builder.Flags;
            DefaultValue = builder.DefaultValue;
            ArgumentConverterType = builder.ArgumentConverterType!;
            builder.SlashMetadata.OptionType = ArgumentConverterType.GetProperty(nameof(IArgumentConverter.OptionType))!.GetValue(null) as ApplicationCommandOptionType? ?? throw new PropertyNullException(nameof(ArgumentConverterType));
            SlashMetadata = new(builder.SlashMetadata);
        }

        public override string ToString() => $"{Overload.Command.FullName} {Type.Name} {Name}";
        public static implicit operator DiscordApplicationCommandOption(CommandParameter parameter) => new(
            parameter.Name.Underscore(),
            parameter.Description,
            parameter.SlashMetadata.OptionType,
            parameter.DefaultValue is DBNull,
            parameter.SlashMetadata.Choices,
            null,
            parameter.SlashMetadata.ChannelTypes,
            parameter.ArgumentConverterType is null,
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? parameter.SlashMetadata.MinValue : null,
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? parameter.SlashMetadata.MaxValue : null,
            parameter.SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
            parameter.SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)parameter.SlashMetadata.MinValue : null,
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)parameter.SlashMetadata.MaxValue : null);
    }
}
