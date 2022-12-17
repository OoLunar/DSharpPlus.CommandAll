using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using Humanizer;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Commands.System.SlashMetadata;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands.System.Commands
{
    /// <summary>
    /// A command parameter.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class CommandParameter
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The overload that this parameter belongs too.
        /// </summary>
        public readonly CommandOverload Overload;

        /// <summary>
        /// The parameter info that this parameter is based off of.
        /// </summary>
        public readonly ParameterInfo ParameterInfo;

        /// <summary>
        /// The flags for this parameter.
        /// </summary>
        public readonly CommandParameterFlags Flags;

        /// <summary>
        /// The default value for this parameter, if any.
        /// </summary>
        public readonly Optional<object?> DefaultValue;

        /// <summary>
        /// The argument converter for this parameter.
        /// </summary>
        /// <remarks>
        /// This is null when <see cref="Overload.Flags"/> has the <see cref="CommandOverloadFlags.Disabled"/> flag set.
        /// </remarks>
        public readonly Type? ArgumentConverterType;

        /// <summary>
        /// The slash metadata for this parameter.
        /// </summary>
        public readonly CommandParameterSlashMetadata SlashMetadata;

        /// <summary>
        /// The name that's used when registering this parameter with Discord.
        /// </summary>
        public readonly string SlashName;

        /// <summary>
        /// The same parameter repeated multiple times until it reaches the maximum amount of parameters.
        /// </summary>
        /// <remarks>
        /// This only has a value when <see cref="CommandParameterFlags.Params"/> is set.
        /// </remarks>
        public readonly DiscordApplicationCommandOption[]? SlashOptions;

        /// <summary>
        /// Creates a new command parameter.
        /// </summary>
        /// <param name="builder">The builder used to create this parameter.</param>
        /// <param name="overload">The overload that this parameter belongs too.</param>
        public CommandParameter(CommandParameterBuilder builder, CommandOverload overload)
        {
            builder.Verify();
            if (builder.ArgumentConverterType is null && !overload.Flags.HasFlag(CommandOverloadFlags.Disabled))
            {
                throw new PropertyNullException(nameof(builder.ArgumentConverterType));
            }

            Name = builder.Name.Truncate(32, "…");
            Description = builder.Description.Truncate(100, "…");
            Overload = overload ?? throw new ArgumentNullException(nameof(overload));
            ParameterInfo = builder.ParameterInfo!;
            Flags = builder.Flags;
            DefaultValue = builder.DefaultValue;
            ArgumentConverterType = builder.ArgumentConverterType;

            builder.SlashMetadata.OptionType = ArgumentConverterType?.GetProperty(nameof(IArgumentConverter.OptionType))?.GetValue(null) as ApplicationCommandOptionType?;
            builder.SlashMetadata.IsRequired = builder.SlashMetadata.IsRequired || !DefaultValue.HasValue;
            SlashMetadata = new(builder.SlashMetadata);

            SlashName = (builder.CommandAllExtension.ParameterNamingStrategy switch
            {
                CommandParameterNamingStrategy.SnakeCase => Name.Underscore(),
                CommandParameterNamingStrategy.KebabCase => Name.Kebaberize(),
                CommandParameterNamingStrategy.LowerCase => Name.ToLowerInvariant(),
                _ => throw new NotImplementedException("Unknown command parameter naming strategy.")
            }).Truncate(32, "…");

            if (Flags.HasFlag(CommandParameterFlags.Params))
            {
                int minimumRequiredOptions = (SlashMetadata.ParameterLimitAttribute?.MinimumElementCount ?? (overload.Method.GetParameters().Length - 1)) - 1;
                SlashOptions = new DiscordApplicationCommandOption[(SlashMetadata.ParameterLimitAttribute?.MaximumElementCount ?? 25) - minimumRequiredOptions];
                for (int i = 0; i < SlashOptions.Length; i++)
                {
                    SlashOptions[i] = new(
                        SlashName = i switch
                        {
                            0 => $"{SlashName}_{i + 1}",
                            < 10 => $"{SlashName[..^1]}{i + 1}",
                            _ => $"{SlashName[..^2]}{i + 1}",
                        },
                        Description,
                        SlashMetadata.OptionType,
                        i < minimumRequiredOptions, // Required until the minimum amount of parameters is reached.
                        SlashMetadata.Choices,
                        null,
                        SlashMetadata.ChannelTypes,
                        ArgumentConverterType is null,
                        SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? SlashMetadata.MinValue : null,
                        SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? SlashMetadata.MaxValue : null,
                        SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
                        SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
                        SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)SlashMetadata.MinValue : null,
                        SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)SlashMetadata.MaxValue : null
                    );
                }
            }
        }

        public override string ToString() => $"{ParameterInfo.Name} {ParameterInfo?.ParameterType.Name}{(Flags == 0 ? string.Empty : $" ({Flags})")}";
        public override bool Equals(object? obj) => obj is CommandParameter parameter && Name == parameter.Name && Description == parameter.Description && EqualityComparer<CommandOverload>.Default.Equals(Overload, parameter.Overload) && EqualityComparer<ParameterInfo>.Default.Equals(ParameterInfo, parameter.ParameterInfo) && Flags == parameter.Flags && DefaultValue.Equals(parameter.DefaultValue) && EqualityComparer<Type?>.Default.Equals(ArgumentConverterType, parameter.ArgumentConverterType) && EqualityComparer<CommandParameterSlashMetadata>.Default.Equals(SlashMetadata, parameter.SlashMetadata) && SlashName == parameter.SlashName && EqualityComparer<DiscordApplicationCommandOption[]?>.Default.Equals(SlashOptions, parameter.SlashOptions);
        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(Name);
            hash.Add(Description);
            hash.Add(Overload);
            hash.Add(ParameterInfo);
            hash.Add(Flags);

            if (DefaultValue.IsDefined(out object? value))
            {
                hash.Add(value);
            }

            if (ArgumentConverterType is not null)
            {
                hash.Add(ArgumentConverterType);
            }

            hash.Add(SlashMetadata);
            hash.Add(SlashName);

            if (SlashOptions is not null)
            {
                hash.Add(SlashOptions);
            }

            return hash.ToHashCode();
        }

        public static implicit operator DiscordApplicationCommandOption(CommandParameter parameter) => new(
            parameter.SlashName,
            parameter.Description,
            parameter.SlashMetadata.OptionType,
            parameter.SlashMetadata.IsRequired,
            parameter.SlashMetadata.Choices,
            null,
            parameter.SlashMetadata.ChannelTypes,
            parameter.ArgumentConverterType is null,
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? parameter.SlashMetadata.MinValue : null,
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? parameter.SlashMetadata.MaxValue : null,
            parameter.SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
            parameter.SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)parameter.SlashMetadata.MinValue : null,
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)parameter.SlashMetadata.MaxValue : null
        );
    }
}
