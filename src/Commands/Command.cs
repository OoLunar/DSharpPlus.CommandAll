using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;
using Humanizer;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class Command
    {
        public readonly string Name;
        public readonly string Description;
        public readonly Command? Parent;
        public readonly IReadOnlyList<CommandOverload> Overloads;
        public readonly IReadOnlyList<Command> Subcommands;
        public readonly IReadOnlyList<string> Aliases;
        public readonly CommandFlags Flags;
        public string FullName => Parent is null ? Name : $"{Parent.FullName} {Name}";

        public Command(CommandBuilder builder, Command? parent = null)
        {
            builder.Verify();
            foreach (string alias in builder.Aliases)
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    continue;
                }

                string trimmed = alias.Trim();
                builder.Aliases.Add(trimmed.Pascalize());
                builder.Aliases.Add(trimmed.Kebaberize());
                builder.Aliases.Add(trimmed.Camelize());
                builder.Aliases.Add(trimmed.Underscore());
            };

            Name = builder.Name!.Trim().Pascalize();
            builder.Aliases.Add(Name.Kebaberize());
            builder.Aliases.Add(Name.Camelize());
            builder.Aliases.Add(Name.Underscore());

            Description = builder.Description!;
            Parent = parent;
            Overloads = builder.Overloads.Select(overloadBuilder => new CommandOverload(overloadBuilder, this)).ToList().AsReadOnly();
            Subcommands = builder.Subcommands.Select(subcommandBuilder => new Command(subcommandBuilder, this)).ToList().AsReadOnly();
            Aliases = builder.Aliases.Distinct().ToList().AsReadOnly();
            Flags = builder.Flags;
        }

        public override string? ToString() => $"{FullName} {(Flags.HasFlag(CommandFlags.Disabled) ? "Disabled " : "")}({Overloads.Count} overloads, {Subcommands.Count} subcommands)";

        public static explicit operator DiscordApplicationCommand(Command command)
        {
            List<DiscordApplicationCommandOption> subCommandAndGroups = new();

            CommandOverload? overload = command.Overloads.FirstOrDefault(overload => overload.Flags.HasFlag(CommandOverloadFlags.SlashPreferred));
            if (overload is not null)
            {
                subCommandAndGroups.Add((DiscordApplicationCommandOption)overload);
            }

            foreach (Command subcommand in command.Subcommands)
            {
                overload = subcommand.Overloads.FirstOrDefault(overload => overload.Flags.HasFlag(CommandOverloadFlags.SlashPreferred));
                if (overload is not null)
                {
                    subCommandAndGroups.Add((DiscordApplicationCommandOption)overload);
                }
            }

            return new DiscordApplicationCommand(command.Name.Underscore(), command.Description, subCommandAndGroups, defaultMemberPermissions: command.Flags.HasFlag(CommandFlags.Disabled) ? Permissions.Administrator : null);
        }

        // This means we're a subcommand group
        public static explicit operator DiscordApplicationCommandOption(Command command)
        {
            List<DiscordApplicationCommandOption> subCommandAndGroups = new();

            CommandOverload? overload = command.Overloads.FirstOrDefault(overload => overload.Flags.HasFlag(CommandOverloadFlags.SlashPreferred));
            if (overload is not null)
            {
                subCommandAndGroups.Add((DiscordApplicationCommandOption)overload);
            }

            foreach (Command subcommand in command.Subcommands)
            {
                overload = subcommand.Overloads.FirstOrDefault(overload => overload.Flags.HasFlag(CommandOverloadFlags.SlashPreferred));
                if (overload is not null)
                {
                    subCommandAndGroups.Add((DiscordApplicationCommandOption)overload);
                }
            }

            return new DiscordApplicationCommandOption(command.Name.Underscore(), command.Description, ApplicationCommandOptionType.SubCommandGroup, null, null, subCommandAndGroups);

        }
    }
}
