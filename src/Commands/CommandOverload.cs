using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using Humanizer;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class CommandOverload
    {
        public readonly Command Command;
        public readonly MethodInfo Method;
        public readonly IReadOnlyList<CommandParameter> Parameters;
        public readonly CommandOverloadFlags Flags;
        public readonly int Priority;
        public readonly CommandSlashMetadata SlashMetadata;

        public CommandOverload(CommandOverloadBuilder builder, Command command)
        {
            builder.Verify();
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Method = builder.Method;
            Priority = builder.Priority;
            Flags = builder.Flags;
            Parameters = builder.Parameters.Select(parameterBuilder => new CommandParameter(parameterBuilder, this)).ToArray();
            SlashMetadata = new(builder.SlashMetadata);
        }

        public override string ToString() => $"{Command.FullName} {string.Join(" ", Parameters.Select(parameter => parameter.Type.Name))}{(Flags.HasFlag(CommandOverloadFlags.Disabled) ? " Disabled " : "")}";

        public static explicit operator DiscordApplicationCommandOption(CommandOverload overload) => new(
            overload.Command.Name.Underscore(),
            overload.Command.Description,
            ApplicationCommandOptionType.SubCommand,
            null, null,
            overload.Parameters.Select(parameter => (DiscordApplicationCommandOption)parameter),
            null, null, null, null,
            overload.SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
            overload.SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value));
    }
}
