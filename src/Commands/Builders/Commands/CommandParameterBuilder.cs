using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands
{
    /// <summary>
    /// A builder for a command parameter.
    /// </summary>
    public sealed class CommandParameterBuilder : Builder
    {
        /// <inheritdoc cref="CommandParameter.Name"/>
        public string? Name { get; set; }

        /// <inheritdoc cref="CommandParameter.Description"/>
        public string? Description { get; set; }

        /// <inheritdoc cref="CommandParameter.Flags"/>
        public CommandParameterFlags Flags { get; set; }

        /// <inheritdoc cref="CommandParameter.DefaultValue"/>
        public Optional<object?> DefaultValue { get; set; }

        /// <inheritdoc cref="CommandParameter.ParameterInfo"/>
        public ParameterInfo? ParameterInfo { get; set; }

        /// <inheritdoc cref="CommandParameter.ArgumentConverterType"/>
        /// <remarks>
        /// This is could be null if not explicitly set by the user. Before <see cref="CommandAllExtension.ConfigureCommands"/> is called, <see cref="CommandAllExtension.ArgumentConverterManager"/> will be used to find the correct converter, if any. If no converter could be found, the constructor for <see cref="CommandParameter"/> will throw an exception.
        /// </remarks>
        public Type? ArgumentConverterType { get; set; }

        /// <inheritdoc cref="CommandParameter.SlashMetadata"/>
        public CommandParameterSlashMetadataBuilder SlashMetadata { get; set; }

        /// <inheritdoc/>
        public CommandParameterBuilder(CommandAllExtension commandAllExtension) : base(commandAllExtension) => SlashMetadata = new(commandAllExtension);

        /// <inheritdoc/>
        [MemberNotNull(nameof(Name), nameof(Description), nameof(ParameterInfo))]
        public override void Verify()
        {
            if (!TryVerify(out Exception? error))
            {
                throw error;
            }
        }

        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(Name), nameof(Description), nameof(ParameterInfo))]
        public override bool TryVerify() => TryVerify(out _);

        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(Name), nameof(Description), nameof(ParameterInfo))]
        public override bool TryVerify([NotNullWhen(false)] out Exception? error)
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
            else if (Flags.HasFlag(CommandParameterFlags.Optional) && DefaultValue.HasValue && DefaultValue.Value is not null && !ParameterInfo.ParameterType.IsAssignableFrom(DefaultValue.Value!.GetType()))
            {
                error = new InvalidPropertyTypeException(nameof(DefaultValue), DefaultValue.GetType(), ParameterInfo.ParameterType.GetType());
                return false;
            }
            else if (ParameterInfo.ParameterType.IsArray && !Flags.HasFlag(CommandParameterFlags.Params))
            {
                error = new InvalidPropertyStateException(nameof(Flags), "Parameter is an array, but Flags does not contain Params. Add the 'params' keyword to the parameter to fix this.");
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

                if (Flags.HasFlag(CommandParameterFlags.Params))
                {
                    if (!Flags.HasFlag(CommandParameterFlags.Optional))
                    {
                        error = new InvalidPropertyStateException(nameof(Flags), $"The {nameof(CommandParameterFlags.Params)} flag must have the {nameof(CommandParameterFlags.Optional)} flag set.");
                        return false;
                    }
                    else if (!ParameterInfo.ParameterType.IsArray)
                    {
                        error = new InvalidPropertyStateException(nameof(ParameterInfo), $"The {nameof(ParameterInfo.ParameterType)} must be an array if the {nameof(CommandParameterFlags.Params)} flag is set.");
                        return false;
                    }
                    else if (!ParameterInfo.ParameterType.GetElementType()!.IsAssignableFrom(argumentConverterInterface.GetGenericArguments()[0]))
                    {
                        error = new InvalidPropertyStateException(nameof(ParameterInfo), $"The {nameof(ParameterInfo.ParameterType)} must be an array of the type {argumentConverterInterface.GetGenericArguments()[0]} if the {nameof(CommandParameterFlags.Params)} flag is set.");
                        return false;
                    }
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

        /// <summary>
        /// Attempts to parse a <see cref="CommandParameter"/> from a <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameterInfo">The <see cref="ParameterInfo"/> to parse.</param>
        public static CommandParameterBuilder Parse(CommandAllExtension commandAllExtension, ParameterInfo parameterInfo) => TryParse(commandAllExtension, parameterInfo, out CommandParameterBuilder? commandParameterBuilder, out Exception? error) ? commandParameterBuilder : throw error;

        /// <inheritdoc cref="Parse(ParameterInfo)"/>
        /// <param name="commandParameterBuilder">The <see cref="CommandParameterBuilder"/> that was parsed.</param>
        /// <returns>Whether or not the <see cref="ParameterInfo"/> was successfully parsed.</returns>
        public static bool TryParse(CommandAllExtension commandAllExtension, ParameterInfo parameterInfo, [NotNullWhen(true)] out CommandParameterBuilder? builder) => TryParse(commandAllExtension, parameterInfo, out builder, out _);

        /// <inheritdoc cref="TryParse(ParameterInfo, out CommandParameterBuilder?)"/>
        /// <param name="error">The error that occurred while parsing the <see cref="ParameterInfo"/>.</param>
        public static bool TryParse(CommandAllExtension commandAllExtension, ParameterInfo parameterInfo, [NotNullWhen(true)] out CommandParameterBuilder? builder, [NotNullWhen(false)] out Exception? error)
        {
            if (parameterInfo is null)
            {
                error = new ArgumentNullException(nameof(parameterInfo));
                builder = null;
                return false;
            }

            builder = new(commandAllExtension)
            {
                Name = parameterInfo.Name!,
                ParameterInfo = parameterInfo,
                DefaultValue = parameterInfo.DefaultValue is DBNull ? Optional.FromNoValue<object?>() : Optional.FromValue(parameterInfo.DefaultValue),
                SlashMetadata = new(commandAllExtension)
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
                    case ParameterLimitAttribute parameterLimit:
                        builder.SlashMetadata.ParameterLimitAttribute = parameterLimit;
                        break;
                    case ParamArrayAttribute:
                        builder.Flags |= CommandParameterFlags.Params | CommandParameterFlags.Optional;
                        builder.DefaultValue = Array.CreateInstance(parameterInfo.ParameterType.GetElementType()!, 0);
                        break;
                    case RemainderTextAttribute:
                        if (builder.ParameterInfo.ParameterType != typeof(string))
                        {
                            error = new InvalidPropertyStateException(nameof(RemainderTextAttribute), $"The {nameof(RemainderTextAttribute)} can only be used on a parameter of type {nameof(String)}.");
                            return false;
                        }

                        builder.Flags |= CommandParameterFlags.RemainderText;
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
