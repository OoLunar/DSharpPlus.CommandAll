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
    public sealed class CommandOverload
    {
        /// <summary>
        /// The command this overload belongs to.
        /// </summary>
        public readonly Command Command;

        /// <summary>
        /// The method this overload calls.
        /// </summary>
        public readonly MethodInfo Method;

        /// <summary>
        /// The parameters of this overload, NOT including the command context parameter.
        /// </summary>
        public readonly IReadOnlyList<CommandParameter> Parameters;

        /// <summary>
        /// The flags of this overload.
        /// </summary>
        public readonly CommandOverloadFlags Flags;

        /// <summary>
        /// The priority of this overload. If this overload is <see cref="CommandOverloadFlags.SlashPreferred"/>, this will be set to 0.
        /// </summary>
        public readonly int Priority;

        /// <summary>
        /// The slash metadata of this overload.
        /// </summary>
        public readonly CommandOverloadSlashMetadata SlashMetadata;

        /// <summary>
        /// The name that's used when registering this parameter with Discord.
        /// </summary>
        public readonly string SlashName;

        /// <summary>
        /// A list of checks that the command should pass before it can be executed.
        /// </summary>
        public readonly IReadOnlyList<CommandCheckAttribute> Checks;

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
        public override bool Equals(object? obj) => obj is CommandOverload overload && EqualityComparer<Command>.Default.Equals(Command, overload.Command) && EqualityComparer<MethodInfo>.Default.Equals(Method, overload.Method) && EqualityComparer<IReadOnlyList<CommandParameter>>.Default.Equals(Parameters, overload.Parameters) && Flags == overload.Flags && Priority == overload.Priority && EqualityComparer<CommandOverloadSlashMetadata>.Default.Equals(SlashMetadata, overload.SlashMetadata) && SlashName == overload.SlashName;
        public override int GetHashCode() => HashCode.Combine(Command, Method, Parameters, Flags, Priority, SlashMetadata, SlashName);

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
