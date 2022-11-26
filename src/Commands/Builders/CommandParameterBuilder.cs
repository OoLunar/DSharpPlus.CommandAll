using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    public sealed class CommandParameterBuilder : IBuilder
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public CommandParameterFlags Flags { get; set; }
        public Optional<object?> DefaultValue { get; set; }
        public Type? ArgumentConverterType { get; set; }
        public ParameterInfo? ParameterInfo { get; set; }
        public CommandParameterSlashMetadataBuilder SlashMetadata { get; set; } = new();

        [MemberNotNull(nameof(Name), nameof(Description), nameof(ParameterInfo))]
        public void Verify()
        {
            if (!TryVerify(out Exception? error))
            {
                throw error;
            }
        }

        [MemberNotNullWhen(true, nameof(Name), nameof(Description), nameof(ParameterInfo))]
        public bool TryVerify() => TryVerify(out _);

        [MemberNotNullWhen(true, nameof(Name), nameof(Description), nameof(ParameterInfo))]
        public bool TryVerify([NotNullWhen(false)] out Exception? error)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                error = new PropertyNullException(nameof(Name));
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Description))
            {
                error = new PropertyNullException(nameof(Description));
                return false;
            }
            else if (ParameterInfo is null)
            {
                error = new PropertyNullException(nameof(ParameterInfo));
                return false;
            }
            else if (Flags.HasFlag(CommandParameterFlags.Params) && !Flags.HasFlag(CommandParameterFlags.Optional))
            {
                error = new InvalidPropertyStateException(nameof(Flags), $"The {nameof(CommandParameterFlags.Params)} flag must have the {nameof(CommandParameterFlags.Optional)} flag set.");
                return false;
            }
            else if (Flags.HasFlag(CommandParameterFlags.Optional) && DefaultValue.HasValue && !ParameterInfo.ParameterType.IsAssignableFrom(DefaultValue.GetType()))
            {
                error = new InvalidPropertyTypeException(nameof(DefaultValue), DefaultValue.GetType(), ParameterInfo.ParameterType.GetType());
                return false;
            }
            else if (ArgumentConverterType is not null)
            {
                Type? argumentConverterInterface = ArgumentConverterType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IArgumentConverter<>));
                if (argumentConverterInterface is null)
                {
                    error = new InvalidPropertyTypeException(nameof(ArgumentConverterType), ArgumentConverterType, typeof(IArgumentConverter<>));
                    return false;
                }
                else if (argumentConverterInterface.GenericTypeArguments[0] != ParameterInfo.ParameterType)
                {
                    error = new InvalidPropertyTypeException(nameof(ArgumentConverterType), ArgumentConverterType, typeof(IArgumentConverter<>).MakeGenericType(ParameterInfo.ParameterType));
                    return false;
                }
            }

            error = null;
            return true;
        }

        public static CommandParameterBuilder Parse(ParameterInfo parameterInfo) => TryParse(parameterInfo, out CommandParameterBuilder? commandParameterBuilder) ? commandParameterBuilder : throw new ArgumentException("Parameter is not a valid command parameter.", nameof(parameterInfo));
        public static bool TryParse(ParameterInfo parameterInfo, [NotNullWhen(true)] out CommandParameterBuilder? builder) => TryParse(parameterInfo, out builder, out _);
        public static bool TryParse(ParameterInfo parameterInfo, [NotNullWhen(true)] out CommandParameterBuilder? builder, [NotNullWhen(false)] out Exception? error)
        {
            if (parameterInfo is null)
            {
                error = new ArgumentNullException(nameof(parameterInfo));
                builder = null;
                return false;
            }

            builder = new()
            {
                Name = parameterInfo.Name!,
                ParameterInfo = parameterInfo,
                DefaultValue = parameterInfo.DefaultValue is DBNull ? Optional.FromNoValue<object?>() : Optional.FromValue(parameterInfo.DefaultValue),
                SlashMetadata = new()
            };

            foreach (object attribute in parameterInfo.GetCustomAttributes())
            {
                switch (attribute)
                {
                    case DescriptionAttribute description:
                        builder.Description = description.Description ?? string.Empty;
                        break;
                    case ArgumentConverterAttribute argumentConverter:
                        builder.ArgumentConverterType = argumentConverter.ArgumentConverterType;
                        builder.SlashMetadata.OptionType = argumentConverter.ArgumentConverterType.GetProperty(nameof(IArgumentConverter.OptionType))!.GetValue(null) as ApplicationCommandOptionType?;
                        break;
                    case MinMaxAttribute minMax:
                        builder.SlashMetadata.MinValue = minMax.MinValue;
                        builder.SlashMetadata.MaxValue = minMax.MaxValue;
                        break;
                    case ChannelTypesAttribute channelTypes:
                        builder.SlashMetadata.ChannelTypes = channelTypes.ChannelTypes.ToList();
                        break;
                    case ParamArrayAttribute:
                        builder.Flags |= CommandParameterFlags.Params;
                        break;
                }
            }

            if (parameterInfo.IsOptional)
            {
                builder.Flags |= CommandParameterFlags.Optional;
            }

            return builder.TryVerify(out error);
        }

        public override string ToString() => $"{ParameterInfo!.ParameterType.Name} {Name} ({ParameterInfo.Member})";
    }
}
