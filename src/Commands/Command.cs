using System.Collections.Generic;
using System.Linq;
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
            builder.Aliases.Add(Name);
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
    }
}
