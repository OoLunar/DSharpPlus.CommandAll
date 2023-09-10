using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DSharpPlus.CommandAll.Commands.Builders;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.CommandAll.Converters;
using DSharpPlus.CommandAll.Exceptions;
using DSharpPlus.Entities;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// A command parameter.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed record CommandParameter
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// The overload that this parameter belongs too.
        /// </summary>
        public CommandOverload Overload { get; init; }

        /// <summary>
        /// The parameter info that this parameter is based off of.
        /// </summary>
        public ParameterInfo ParameterInfo { get; init; }

        /// <summary>
        /// The flags for this parameter.
        /// </summary>
        public CommandParameterFlags Flags { get; init; }

        /// <summary>
        /// The default value for this parameter, if any.
        /// </summary>
        public object? DefaultValue { get; init; }

        /// <summary>
        /// The argument converter for this parameter.
        /// </summary>
        /// <remarks>
        /// This is null when <see cref="CommandOverload.Flags"/> has the <see cref="CommandOverloadFlags.Disabled"/> flag set.
        /// </remarks>
        public Type? ArgumentConverterType { get; init; }

        /// <summary>
        /// The slash metadata for this parameter.
        /// </summary>
        public CommandParameterSlashMetadata SlashMetadata { get; init; }

        /// <summary>
        /// The slash names for this parameter. While normally containing a single element, it may have multiple elements if the parameter is a <see cref="CommandParameterFlags.Params"/> parameter.
        /// </summary>
        public IReadOnlyList<string> SlashNames { get; init; }

        /// <summary>
        /// The same parameter repeated multiple times until it reaches the maximum amount of parameters.
        /// </summary>
        /// <remarks>
        /// This only has a value when <see cref="CommandParameterFlags.Params"/> is set.
        /// </remarks>
        public DiscordApplicationCommandOption[]? SlashOptions { get; init; }

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
            DefaultValue = builder.ParameterInfo.HasDefaultValue ? builder.ParameterInfo.DefaultValue : (builder.ParameterInfo.ParameterType.IsValueType ? Activator.CreateInstance(builder.ParameterInfo.ParameterType) : null);
            ArgumentConverterType = builder.ArgumentConverterType;

            builder.SlashMetadata.OptionType = ArgumentConverterType?.GetProperty(nameof(IArgumentConverter.OptionType))?.GetValue(ActivatorUtilities.CreateInstance(builder.CommandAllExtension.ServiceProvider, ArgumentConverterType!)) as ApplicationCommandOptionType?;
            builder.SlashMetadata.IsRequired = builder.SlashMetadata.IsRequired || !ParameterInfo.HasDefaultValue;
            SlashMetadata = new(builder.SlashMetadata);

            List<string> slashNames = new()
            {
                (builder.CommandAllExtension.ParameterNamingStrategy switch
                {
                    CommandParameterNamingStrategy.SnakeCase => Name.Underscore(),
                    CommandParameterNamingStrategy.KebabCase => Name.Kebaberize(),
                    CommandParameterNamingStrategy.LowerCase => Name.ToLowerInvariant(),
                    _ => throw new NotImplementedException("Unknown command parameter naming strategy.")
                }).Truncate(32, "…")
            };

            if (Flags.HasFlag(CommandParameterFlags.Params))
            {
                int minimumRequiredOptions = (SlashMetadata.ParameterLimitAttribute?.MinimumElementCount ?? (overload.Method.GetParameters().Length - 1)) - 1;
                SlashOptions = new DiscordApplicationCommandOption[(SlashMetadata.ParameterLimitAttribute?.MaximumElementCount ?? 25) - minimumRequiredOptions];
                for (int i = 0; i < SlashOptions.Length; i++)
                {
                    if (i == 0)
                    {
                        slashNames[0] = $"{slashNames[0]}_{i + 1}";
                    }
                    else
                    {
                        slashNames.Add(i switch
                        {
                            < 10 => $"{slashNames[i - 1][..^1]}{i + 1}",
                            _ => $"{slashNames[i - 1][..^2]}{i + 1}",
                        });
                    }

                    SlashOptions[i] = new(
                        slashNames[i],
                        Description,
                        SlashMetadata.OptionType,
                        i < minimumRequiredOptions, // Required until the minimum amount of parameters is reached.
                        SlashMetadata.Choices,
                        null,
                        SlashMetadata.ChannelTypes,
                        ArgumentConverterType is null || Flags.HasFlag(CommandParameterFlags.AutoComplete),
                        SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? SlashMetadata.MinValue : null,
                        SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? SlashMetadata.MaxValue : null,
                        SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
                        SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
                        SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)SlashMetadata.MinValue : null,
                        SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)SlashMetadata.MaxValue : null
                    );
                }
            }

            SlashNames = slashNames.AsReadOnly();
        }

        public override string ToString() => $"{ParameterInfo.Name} {ParameterInfo?.ParameterType.Name}{(Flags == 0 ? string.Empty : $" ({Flags})")}";

        public static implicit operator DiscordApplicationCommandOption(CommandParameter parameter) => new(
            parameter.SlashNames[0],
            parameter.Description,
            parameter.SlashMetadata.OptionType,
            parameter.SlashMetadata.IsRequired,
            parameter.SlashMetadata.Choices,
            null,
            parameter.SlashMetadata.ChannelTypes,
            parameter.ArgumentConverterType is null || parameter.Flags.HasFlag(CommandParameterFlags.AutoComplete),
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? parameter.SlashMetadata.MinValue : null,
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.Integer or ApplicationCommandOptionType.Number ? parameter.SlashMetadata.MaxValue : null,
            parameter.SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
            parameter.SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)parameter.SlashMetadata.MinValue : null,
            parameter.SlashMetadata.OptionType is ApplicationCommandOptionType.String ? (int?)parameter.SlashMetadata.MaxValue : null
        );
    }
}
