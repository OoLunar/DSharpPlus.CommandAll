using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class CommandBuilder
    {
        private static readonly Type _baseCommandType = typeof(BaseCommand);

        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<CommandOverloadBuilder> Overloads { get; set; } = new List<CommandOverloadBuilder>();
        public List<CommandBuilder> Subcommands { get; set; } = new List<CommandBuilder>();
        public List<string> Aliases { get; set; } = new();
        public CommandFlags Flags { get; set; }

        public void NormalizeOverloadPriorities()
        {
            int priority = 0;
            List<CommandOverloadBuilder> overloads = new();

            // Order the priority by descending order (highest number to lowest number).
            // The overload priority can be defined manually by the user.
            // When it isn't, the priority is 0. We want to prefer the method with the highest amount of parameters.
            foreach (CommandOverloadBuilder overload in Overloads.OrderByDescending(overload => overload.Priority == 0 ? overload.Parameters.Count : overload.Priority))
            {
                // If the overload is preferred by slash commands, it must have an index of 0.
                if (overload.IsSlashPreferred)
                {
                    overloads.Insert(0, overload);
                    continue;
                }

                overload.Priority = priority++;
                overloads.Insert(overload.Priority, overload);
            }

            // Set only the first element to be slash preferred.
            for (int i = 0; i < overloads.Count; i++)
            {
                overloads[i].IsSlashPreferred = i == 0;
            }
            Overloads = overloads;
        }

        [MemberNotNull(nameof(Name), nameof(Description))]
        public void Verify()
        {
            if (!TryVerify(out Exception? error))
            {
                throw error;
            }
        }

        [MemberNotNullWhen(true, nameof(Name), nameof(Description))]
        public bool TryVerify() => TryVerify(out _);

        [MemberNotNullWhen(true, nameof(Name), nameof(Description))]
        public bool TryVerify([NotNullWhen(false)] out Exception? error)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                error = new PropertyNullException(nameof(Name));
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Description))
            {
                error = new PropertyNullException(nameof(Description));
                return false;
            }
            // Determine if we're at a level that allows subcommands
            else if (Subcommands.Count != 0 && Subcommands[0].Subcommands.Count != 0 && Subcommands[0].Subcommands[0].Subcommands.Count != 0)
            {
                error = new InvalidPropertyStateException(nameof(Subcommands), "Commands may not group commands that contain more groups!");
                return false;
            }
            else if (Subcommands.Count > 25)
            {
                error = new PropertyOutOfRangeException(nameof(Subcommands.Count), 0, 25, Subcommands.Count);
                return false;
            }

            // Verify subcommands
            foreach (CommandBuilder subcommand in Subcommands)
            {
                if (!subcommand.TryVerify(out error))
                {
                    return false;
                }
            }

            // Verify overloads
            foreach (CommandOverloadBuilder overload in Overloads)
            {
                if (!overload.TryVerify(out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        public static IEnumerable<CommandBuilder> Parse(Type type) => TryParse(type, 0, out IEnumerable<CommandBuilder>? builders, out Exception? error) ? builders : throw error;
        public static bool TryParse(Type type, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders) => TryParse(type, 0, out builders, out _);
        public static bool TryParse(Type type, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders, [NotNullWhen(false)] out Exception? error) => TryParse(type, 0, out builders, out error);
        private static bool TryParse(Type type, int? recursionCount, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders, [NotNullWhen(false)] out Exception? error)
        {
            if (type is null)
            {
                error = new ArgumentNullException(nameof(type));
                builders = null;
                return false;
            }
            else if (_baseCommandType.IsAssignableTo(type))
            {
                error = new InvalidCastException($"The type {type.FullName} must be assignable from {nameof(BaseCommand)}!");
                builders = null;
                return false;
            }
            else if (recursionCount == 2)
            {
                error = new InvalidOperationException("Groups must not have subgroups! The maximum amount of nested classes is 1.");
                builders = null;
                return false;
            }

            recursionCount ??= 0;

            // Parse overloads
            List<KeyValuePair<string, CommandOverloadBuilder>> overloads = new();
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                if (method.GetCustomAttribute<CommandAttribute>() is CommandAttribute commandAttribute && CommandOverloadBuilder.TryParse(method, out CommandOverloadBuilder? overload))
                {
                    overloads.Add(new KeyValuePair<string, CommandOverloadBuilder>(commandAttribute.Name, overload));
                }
            }

            List<CommandBuilder> commandBuilders = new();
            foreach (IGrouping<string, KeyValuePair<string, CommandOverloadBuilder>> value in overloads.GroupBy(x => x.Key))
            {
                CommandBuilder builder = new() { Overloads = new(value.Select(x => x.Value)) };
                builder.NormalizeOverloadPriorities();
                builder.Name = value.Key;

                // Take the description from the first overload that has it.
                foreach (CommandOverloadBuilder overload in builder.Overloads)
                {
                    if (overload.Method.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute descriptionAttribute)
                    {
                        builder.Description = descriptionAttribute.Description;
                    }
                }

                commandBuilders.Add(builder);
            }

            // Parse subcommands
            foreach (Type subType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                if (!TryParse(subType, recursionCount + 1, out IEnumerable<CommandBuilder>? subBuilders, out error))
                {
                    builders = null;
                    return false;
                }
                commandBuilders.AddRange(subBuilders);
            }

            if (!commandBuilders.Any())
            {
                error = new ArgumentNullException(nameof(type), "No commands were found.");
                builders = null;
                return false;
            }
            // If the whole thing is a group command and this is the top level, then we need to mark this as a group command. If it isn't, then the subcommand parsing done above will automatically mark the results as a group command for us.
            else if (type.GetCustomAttribute<CommandAttribute>() is CommandAttribute commandAttribute)
            {
                CommandBuilder builder = new()
                {
                    Name = commandAttribute.Name,
                    Description = type.GetCustomAttribute<DescriptionAttribute>()?.Description,
                    Subcommands = new(commandBuilders),
                };

                builders = new[] { builder };
                error = null;
                return true;
            }
            // If there are many commands
            else
            {
                builders = commandBuilders;
                error = null;
                return true;
            }
        }

        public override string? ToString() => $"Command Builder, {Name} {(Flags.HasFlag(CommandFlags.Disabled) ? "Disabled " : "")}({Overloads.Count} overloads, {Subcommands.Count} subcommands)";
    }
}
