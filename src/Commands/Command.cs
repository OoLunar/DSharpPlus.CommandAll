using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DSharpPlus.CommandAll.Commands.Builders;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;
using Humanizer;

namespace DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// A command. This can be a top level command, subcommand and/or group command.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class Command
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The description of the command.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// The command's parent, if any.
        /// </summary>
        public readonly Command? Parent;

        /// <summary>
        /// The command's overloads, if any.
        /// </summary>
        /// <remarks>
        /// The overload registered as the slash command will always be the first overload in this list.
        /// </remarks>
        public readonly IReadOnlyList<CommandOverload> Overloads;

        /// <summary>
        /// The command's subcommands or subcommand groups, if any.
        /// </summary>
        public readonly IReadOnlyList<Command> Subcommands;

        /// <summary>
        /// The command's aliases.
        /// </summary>
        public readonly IReadOnlyList<string> Aliases;

        /// <summary>
        /// The command's flags.
        /// </summary>
        public readonly CommandFlags Flags;

        /// <summary>
        /// The command's slash metadata.
        /// </summary>
        public readonly CommandSlashMetadata SlashMetadata;

        /// <summary>
        /// The name that's used when registering this parameter with Discord.
        /// </summary>
        public readonly string SlashName;

        /// <summary>
        /// The command's name concatenated with its parents.
        /// </summary>
        public string FullName => Parent is null ? Name : $"{Parent.FullName} {Name}";

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="builder">The command builder.</param>
        /// <param name="parent">The command's parent, if any.</param>
        public Command(CommandBuilder builder, Command? parent = null)
        {
            builder.NormalizeOverloadPriorities();
            builder.Verify();

            Name = builder.Name!.Trim().Pascalize();
            List<string> aliases = new()
            {
                Name.ToLowerInvariant(),
                Name.Kebaberize(),
                Name.Camelize(),
                Name.Underscore()
            };

            SlashName = builder.CommandAllExtension.ParameterNamingStrategy switch
            {
                CommandParameterNamingStrategy.SnakeCase => Name.Underscore(),
                CommandParameterNamingStrategy.KebabCase => Name.Kebaberize(),
                CommandParameterNamingStrategy.LowerCase => Name.ToLowerInvariant(),
                _ => throw new NotImplementedException("Unknown command parameter naming strategy.")
            };

            foreach (string alias in builder.Aliases)
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    continue;
                }

                string trimmed = alias.Trim();
                aliases.Add(trimmed.Pascalize());
                aliases.Add(trimmed.Pascalize().ToLowerInvariant());
                aliases.Add(trimmed.Kebaberize());
                aliases.Add(trimmed.Camelize());
                aliases.Add(trimmed.Underscore());
            };

            Description = builder.Description.Truncate(100, "…");
            Parent = parent;
            Overloads = builder.Overloads.Select(overloadBuilder => new CommandOverload(overloadBuilder, this)).ToList().AsReadOnly();
            Subcommands = builder.Subcommands.Select(subcommandBuilder => new Command(subcommandBuilder, this)).ToList().AsReadOnly();
            Aliases = aliases.Distinct().ToList().AsReadOnly();
            Flags = builder.Flags;
            SlashMetadata = new(builder.SlashMetadata);
        }

        public override string ToString() => $"{FullName}{(Flags == 0 ? string.Empty : $" ({Flags.Humanize()})")} - {Description}";
        public override bool Equals(object? obj) => obj is Command command && Name == command.Name && Description == command.Description && EqualityComparer<Command?>.Default.Equals(Parent, command.Parent) && EqualityComparer<IReadOnlyList<CommandOverload>>.Default.Equals(Overloads, command.Overloads) && EqualityComparer<IReadOnlyList<Command>>.Default.Equals(Subcommands, command.Subcommands) && EqualityComparer<IReadOnlyList<string>>.Default.Equals(Aliases, command.Aliases) && Flags == command.Flags && EqualityComparer<CommandSlashMetadata>.Default.Equals(SlashMetadata, command.SlashMetadata) && SlashName == command.SlashName && FullName == command.FullName;
        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(Name);
            hash.Add(Description);

            if (Parent is not null)
            {
                hash.Add(Parent);
            }

            hash.Add(Overloads);
            hash.Add(Subcommands);
            hash.Add(Aliases);
            hash.Add(Flags);
            hash.Add(SlashMetadata);
            hash.Add(SlashName);
            hash.Add(FullName);
            return hash.ToHashCode();
        }

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
                subCommandAndGroups.Add((DiscordApplicationCommandOption)subcommand);
            }

            return new DiscordApplicationCommand(
                command.SlashName,
                command.Description,
                // Check if this is the only subcommand. If it is, promote it to a top level command.
                // This prevents commands like /ping ping from being registered.
                subCommandAndGroups.Count == 1 ? subCommandAndGroups[0].Options : subCommandAndGroups,
                null,
                ApplicationCommandType.SlashCommand,
                command.SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
                command.SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
                command.Flags.HasFlag(CommandFlags.AllowDirectMessages),
                command.SlashMetadata.RequiredPermissions
            );
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

            return new DiscordApplicationCommandOption(
                command.SlashName,
                command.Description,
                subCommandAndGroups.Count == 1 ? ApplicationCommandOptionType.SubCommand : ApplicationCommandOptionType.SubCommandGroup,
                null, null,
                subCommandAndGroups.Count == 1 ? subCommandAndGroups[0].Options : subCommandAndGroups,
                null, null, null, null,
                command.SlashMetadata.LocalizedNames.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value),
                command.SlashMetadata.LocalizedDescriptions.ToDictionary(x => x.Key.Parent.TwoLetterISOLanguageName == x.Key.TwoLetterISOLanguageName ? x.Key.Parent.TwoLetterISOLanguageName : $"{x.Key.Parent.TwoLetterISOLanguageName}-{x.Key.TwoLetterISOLanguageName}", x => x.Value));
        }
    }
}
