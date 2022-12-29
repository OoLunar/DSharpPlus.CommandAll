using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;
using DSharpPlus.CommandAll.Commands.Converters;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.CommandAll.Exceptions;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Builders
{
    /// <summary>
    /// A builder for a command parameter.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
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

        /// <inheritdoc cref="CommandParameter.ArgumentConverter"/>
        /// <remarks>
        /// This is could be null if not explicitly set by the user. Before <see cref="CommandAllExtension.ConfigureCommands"/> is called, <see cref="CommandAllExtension.ArgumentConverterManager"/> will be used to find the correct converter, if any. If no converter could be found, the constructor for <see cref="CommandParameter"/> will throw an exception.
        /// </remarks>
        public ArgumentConverterDefinition? ArgumentConverter { get; set; }

        /// <inheritdoc cref="CommandParameter.SlashMetadata"/>
        public CommandParameterSlashMetadataBuilder SlashMetadata { get; set; }

        /// <inheritdoc cref="CommandParameter.Overload"/>
        public CommandOverloadBuilder? OverloadBuilder { get; set; }

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
            else if (ArgumentConverter is not null)
            {
                IArgumentConverter converter = ArgumentConverter.GetOrCreateConverter(CommandAllExtension.ServiceProvider);
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
                    else if (!converter.CanConvert(ParameterInfo.ParameterType.GetElementType()!))
                    {
                        error = new InvalidPropertyStateException(nameof(ParameterInfo), $"Argument converter '{converter.GetType().FullName}' cannot convert the parameter type {ParameterInfo.ParameterType}.");
                        return false;
                    }
                }
                else if (!converter.CanConvert(ParameterInfo.ParameterType))
                {
                    error = new InvalidPropertyStateException(nameof(ArgumentConverter), $"Argument converter '{converter.GetType().FullName}' cannot convert the parameter type {ParameterInfo.ParameterType}.");
                    return false;
                }
            }

            error = null;
            return true;
        }

        /// <inheritdoc cref="TryParse(CommandAllExtension, ParameterInfo, out CommandParameterBuilder?, out Exception?)"/>
        /// <summary>
        /// Parses a <see cref="CommandParameterBuilder"/> from a <see cref="ParameterInfo"/>.
        /// </summary>
        /// <returns>The parsed <see cref="CommandParameterBuilder"/>.</returns>
        public static CommandParameterBuilder Parse(CommandAllExtension commandAllExtension, ParameterInfo parameterInfo) => TryParse(commandAllExtension, parameterInfo, out CommandParameterBuilder? commandParameterBuilder, out Exception? error) ? commandParameterBuilder : throw error;

        /// <inheritdoc cref="TryParse(CommandAllExtension, ParameterInfo, out CommandParameterBuilder?, out Exception?)"/>
        public static bool TryParse(CommandAllExtension commandAllExtension, ParameterInfo parameterInfo, [NotNullWhen(true)] out CommandParameterBuilder? builder) => TryParse(commandAllExtension, parameterInfo, out builder, out _);

        /// <summary>
        /// Attempts to parse a <see cref="CommandParameterBuilder"/> from a <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="commandAllExtension">The <see cref="CommandAllExtension"/> to use when grabbing configuration values.</param>
        /// <param name="parameterInfo">The <see cref="ParameterInfo"/> to parse from.</param>
        /// <param name="builder">The <see cref="CommandParameterBuilder"/> that was parsed.</param>
        /// <param name="error">The <see cref="Exception"/> that was found when parsing the <see cref="CommandOverloadBuilder"/>. Not thrown.</param>
        /// <returns>Whether or not the <see cref="CommandParameterBuilder"/> was parsed successfully.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameterInfo"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidPropertyStateException">The <see cref="CommandParameterBuilder"/> has an invalid state. See the error message for more details.</exception>
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

            ArgumentConverterDefinition? converter;
            if (parameterInfo.GetCustomAttribute<ArgumentConverterAttribute>() is ArgumentConverterAttribute argumentConverterAttribute)
            {
                if (!commandAllExtension.ArgumentConverterManager.TryGetConverter(argumentConverterAttribute.ArgumentConverterType, out converter))
                {
                    error = new InvalidPropertyStateException(nameof(ArgumentConverter), $"Argument converter '{argumentConverterAttribute.ArgumentConverterType.FullName}' is not registered.");
                    return false;
                }
            }
            else if (!commandAllExtension.ArgumentConverterManager.TryGetConverter(parameterInfo.ParameterType, out converter))
            {
                error = new InvalidPropertyStateException(nameof(ArgumentConverter), $"Argument converter for type '{parameterInfo.ParameterType.FullName}' is not found.");
                return false;
            }

            builder.ArgumentConverter = converter;
            builder.SlashMetadata.OptionType = converter!.GetOrCreateConverter(commandAllExtension.ServiceProvider).OptionType;

            foreach (object attribute in parameterInfo.GetCustomAttributes())
            {
                switch (attribute)
                {
                    case DescriptionAttribute description:
                        builder.Description = description.Description ?? string.Empty;
                        break;
                    case MinMaxAttribute minMax:
                        builder.SlashMetadata.MinValue = minMax.MinValue;
                        builder.SlashMetadata.MaxValue = minMax.MaxValue;
                        break;
                    case ChannelTypesAttribute channelTypes:
                        builder.SlashMetadata.ChannelTypes = channelTypes.ChannelTypes.ToList();
                        break;
                    case ParamArrayAttribute:
                        builder.Flags |= CommandParameterFlags.Params | CommandParameterFlags.Optional;
                        builder.DefaultValue = Array.CreateInstance(parameterInfo.ParameterType.GetElementType()!, 0);
                        break;
                    case RequiredByAttribute requiredBy:
                        builder.SlashMetadata.IsRequired = requiredBy.RequiredBy.HasFlag(RequiredBy.SlashCommand);
                        break;
                    case RemainingTextAttribute:
                        if (builder.ParameterInfo.ParameterType != typeof(string))
                        {
                            error = new InvalidPropertyStateException(nameof(RemainingTextAttribute), $"The {nameof(RemainingTextAttribute)} can only be used on a parameter of type {nameof(String)}.");
                            return false;
                        }

                        builder.Flags |= CommandParameterFlags.RemainingText;
                        break;
                }
            }

            if (builder.Flags.HasFlag(CommandParameterFlags.Params))
            {
                builder.Flags |= CommandParameterFlags.TrimExcess;

                if (parameterInfo.GetCustomAttribute<ParameterLimitAttribute>() is ParameterLimitAttribute parameterLimit)
                {
                    builder.SlashMetadata.ParameterLimitAttribute = parameterLimit;

                    // Add TrimExcess unless explicitly set to false
                    if (!parameterLimit.TrimExcess)
                    {
                        builder.Flags &= ~CommandParameterFlags.TrimExcess;
                    }
                }
            }

            if (parameterInfo.IsOptional)
            {
                builder.Flags |= CommandParameterFlags.Optional;
            }

            if (parameterInfo.ParameterType.IsEnum)
            {
                string[] enumNames = Enum.GetNames(parameterInfo.ParameterType);
                Array enumValues = Enum.GetValuesAsUnderlyingType(parameterInfo.ParameterType);

                for (int i = 0; i < enumNames.Length; i++)
                {
                    builder.SlashMetadata.Choices ??= new();
                    builder.SlashMetadata.Choices.Add(new(enumNames[i], Convert.ToDouble(enumValues.GetValue(i), CultureInfo.InvariantCulture)));
                }

                builder.ArgumentConverter ??= commandAllExtension.ArgumentConverterManager.TryGetConverter(typeof(EnumArgumentConverter), out converter)
                    ? converter
                    : commandAllExtension.ArgumentConverterManager.AddArgumentConverters(typeof(EnumArgumentConverter))[0];
            }

            error = null;
            return true;
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(CommandAllExtension);

            if (Name is not null)
            {
                hash.Add(Name);
            }

            if (Description is not null)
            {
                hash.Add(Description);
            }

            if (DefaultValue.IsDefined(out object? value))
            {
                hash.Add(value);
            }

            if (ParameterInfo is not null)
            {
                hash.Add(ParameterInfo);
            }

            if (ArgumentConverter is not null)
            {
                hash.Add(ArgumentConverter);
            }

            hash.Add(SlashMetadata);
            return hash.ToHashCode();
        }
    }
}
