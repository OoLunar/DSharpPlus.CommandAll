using System;
using System.Linq;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using Humanizer;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// A command parameter.
    /// </summary>
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
        public readonly Type ArgumentConverterType;

        /// <summary>
        /// The slash metadata for this parameter.
        /// </summary>
        public readonly CommandParameterSlashMetadata SlashMetadata;

        /// <summary>
        /// The name that's used when registering this parameter with Discord.
        /// </summary>
        public readonly string SlashName;

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

            Name = builder.Name;
            Description = builder.Description;
            Overload = overload ?? throw new ArgumentNullException(nameof(overload));
            ParameterInfo = builder.ParameterInfo!;
            Flags = builder.Flags;
            DefaultValue = builder.DefaultValue;
            ArgumentConverterType = builder.ArgumentConverterType!;
            builder.SlashMetadata.OptionType = ArgumentConverterType.GetProperty(nameof(IArgumentConverter.OptionType))!.GetValue(null) as ApplicationCommandOptionType? ?? throw new PropertyNullException(nameof(ArgumentConverterType));
            SlashMetadata = new(builder.SlashMetadata);
            SlashName = builder.CommandAllExtension.ParameterNamingStrategy switch
            {
                CommandParameterNamingStrategy.SnakeCase => Name.Underscore(),
                CommandParameterNamingStrategy.KebabCase => Name.Kebaberize(),
                CommandParameterNamingStrategy.LowerCase => Name.ToLowerInvariant(),
                _ => throw new NotImplementedException("Unknown command parameter naming strategy.")
            };
        }

        public override string ToString() => $"{Overload.Command.FullName} {ParameterInfo.ParameterType.Name} {Name}";
        public static implicit operator DiscordApplicationCommandOption(CommandParameter parameter) => new(
            parameter.SlashName,
            parameter.Description.Truncate(100),
            parameter.SlashMetadata.OptionType,
            parameter.DefaultValue.HasValue,
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
