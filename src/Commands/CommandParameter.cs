using System;
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
        public readonly ApplicationCommandOptionType OptionType;
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
            OptionType = ArgumentConverterType.GetProperty(nameof(IArgumentConverter.OptionType))!.GetValue(null) as ApplicationCommandOptionType? ?? throw new PropertyNullException(nameof(ArgumentConverterType));
        }

        public override string ToString() => $"{Overload.Command.FullName} {Type.Name} {Name}";
        public static implicit operator DiscordApplicationCommandOption(CommandParameter parameter) => new(parameter.Name.Underscore(), parameter.Description, parameter.OptionType, parameter.DefaultValue is null);
    }
}
