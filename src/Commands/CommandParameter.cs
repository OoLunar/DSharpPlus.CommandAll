using System;
using System.Reflection;
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
        public Type Type => ParameterInfo.ParameterType;

        public CommandParameter(CommandParameterBuilder builder, CommandOverload overload)
        {
            builder.Verify();
            if (builder.ArgumentConverterType is null)
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
        }
    }
}
