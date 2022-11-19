using System.Collections.Generic;
using System.Linq;

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

        public Command(string name, IEnumerable<string> aliases, string description, IEnumerable<CommandOverload> overloads, IEnumerable<Command> subcommands, Command? parent = null)
        {
            Name = name;
            Description = description;
            List<CommandOverload> overloadsList = overloads.ToList();
            overloadsList.Sort((overloadA, overloadB) => overloadA.Priority is not null && overloadB.Priority is not null ? overloadA.Priority.Value.CompareTo(overloadB.Priority.Value) : (overloadA.Priority is null ? 1 : -1));
            Overloads = overloadsList.AsReadOnly();
            Subcommands = subcommands.ToList().AsReadOnly();
            Aliases = aliases.ToList().AsReadOnly();
            Parent = parent;
        }
    }
}
