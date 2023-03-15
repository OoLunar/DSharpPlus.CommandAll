using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands.Builders;
using DSharpPlus.CommandAll.Commands.Checks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;
using Humanizer;

namespace DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// A command overload.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed record CommandOverload
    {
        /// <summary>
        /// The command this overload belongs to.
        /// </summary>
        public Command Command { get; init; }

        /// <summary>
        /// The method this overload calls.
        /// </summary>
        public MethodInfo Method { get; init; }

        /// <summary>
        /// The parameters of this overload, NOT including the command context parameter.
        /// </summary>
        public IReadOnlyList<CommandParameter> Parameters { get; init; }

        /// <summary>
        /// The flags of this overload.
        /// </summary>
        public CommandOverloadFlags Flags { get; init; }

        /// <summary>
        /// The priority of this overload. If this overload is <see cref="CommandOverloadFlags.SlashPreferred"/>, this will be set to 0.
        /// </summary>
        public int Priority { get; init; }

        /// <summary>
        /// The slash metadata of this overload.
        /// </summary>
        public CommandOverloadSlashMetadata SlashMetadata { get; init; }

        /// <summary>
        /// The name that's used when registering this parameter with Discord.
        /// </summary>
        public string SlashName { get; init; }

        /// <summary>
        /// A list of checks that the command should pass before it can be executed.
        /// </summary>
        public IReadOnlyList<CommandCheckAttribute> Checks { get; init; }

        /// <summary>
        /// Creates a new command overload.
        /// </summary>
        /// <param name="builder">The builder used to create this overload.</param>
        /// <param name="command">The command this overload belongs to.</param>
        public CommandOverload(CommandOverloadBuilder builder, Command command)
        {
            builder.Verify();
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Method = builder.Method;
            Priority = builder.Priority;
            Flags = builder.Flags;
            Parameters = builder.Parameters.Select(parameterBuilder => new CommandParameter(parameterBuilder, this)).ToArray();
            SlashMetadata = new(builder.SlashMetadata);
            SlashName = builder.CommandAllExtension.ParameterNamingStrategy switch
            {
                CommandParameterNamingStrategy.SnakeCase => Command.Name.Underscore(),
                CommandParameterNamingStrategy.KebabCase => Command.Name.Kebaberize(),
                CommandParameterNamingStrategy.LowerCase => Command.Name.ToLowerInvariant(),
                _ => throw new NotImplementedException("Unknown command parameter naming strategy.")
            };

            Checks = builder.Checks.ToList().AsReadOnly();
        }

        public override string ToString() => $"{Command.FullName}, {Method.Name}{(Flags == 0 ? string.Empty : $" ({Flags.Humanize()})")}, Priority: {Priority}, Parameters: {Parameters.Humanize()}";
        public static explicit operator DiscordApplicationCommandOption(CommandOverload overload)
        {
            if (overload.Parameters.Count > 25)
            {
                throw new InvalidOperationException($"A command overload can't have more than 25 parameters! If you're using {nameof(ParameterLimitAttribute)}, be sure to take it's {nameof(ParameterLimitAttribute.MaximumElementCount)} into count!");
            }

            IEnumerable<DiscordApplicationCommandOption> parameters = overload.Parameters.SelectMany(parameter => parameter.Flags.HasFlag(CommandParameterFlags.Params) ? parameter.SlashOptions! : new[] { (DiscordApplicationCommandOption)parameter });
            return new(
                overload.SlashName,
                overload.Command.Description,
                ApplicationCommandOptionType.SubCommand,
                null, null,
                parameters,
                null, null, null, null,
                overload.SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
                overload.SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value)
            );
        }
    }
}
