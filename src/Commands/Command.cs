using System.Collections.Generic;
using System.Linq;
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
            Name = builder.Name!;
            Description = builder.Description!;
            Parent = parent;
            Overloads = builder.Overloads.Select(overloadBuilder => new CommandOverload(overloadBuilder, this)).ToList().AsReadOnly();
            Subcommands = builder.Subcommands.Select(subcommandBuilder => new Command(subcommandBuilder, this)).ToList().AsReadOnly();
            Aliases = builder.Aliases.AsReadOnly();
            Flags = builder.Flags;
        }

        public override string? ToString() => $"{FullName} {(Flags.HasFlag(CommandFlags.Disabled) ? "Disabled " : "")}({Overloads.Count} overloads, {Subcommands.Count} subcommands)";
    }
}
