using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Humanizer;
using OoLunar.DSharpPlus.CommandAll.Attributes;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class CommandBuilder
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public CommandBuilder? Parent { get; set; }
        public List<CommandOverload> Overloads { get; set; } = new List<CommandOverload>();
        public List<CommandBuilder> Subcommands { get; set; } = new List<CommandBuilder>();
        public List<string> Aliases { get; set; } = new List<string>();

        public static IEnumerable<CommandBuilder> Parse(Type type) => TryParse(type, 0, out IEnumerable<CommandBuilder>? builders, out string? error) ? builders : throw new InvalidOperationException(error);

        public static bool TryParse(Type type, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders) => TryParse(type, 0, out builders, out _);

        public static bool TryParse(Type type, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders, [NotNullWhen(false)] out string? error) => TryParse(type, 0, out builders, out error);

        private static bool TryParse(Type type, int? recursionCount, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders, [NotNullWhen(false)] out string? error)
        {
            if (type is null)
            {
                builders = null;
                error = $"{nameof(type)} cannot be null.";
                return false;
            }
            else if (recursionCount > 3) // 0 based, so 2 is the third level
            {
                builders = null;
                error = $"Recursion limit exceeded. {nameof(type)}: {type.FullName}";
                return false;
            }

            CommandBuilder commandBuilder = new();
            if (type.GetCustomAttribute<CommandAttribute>() is CommandAttribute commandAttribute)
            {
                commandBuilder.Name = commandAttribute.Name;
                commandBuilder.Aliases = commandAttribute.Aliases.ToList();
                commandBuilder.Description = type.GetCustomAttribute<DescriptionAttribute>()?.Description;
            }

            foreach (Type nestedType in type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                if (recursionCount == 2)
                {
                    builders = null;
                    error = $"Recursion limit exceeded. Subcommand groups may not have subcommand groups. {nameof(type)}: {type.FullName}";
                    return false;
                }
                else if (TryParse(nestedType, recursionCount + 1, out IEnumerable<CommandBuilder>? nestedBuilders, out error))
                {
                    foreach (CommandBuilder nestedBuilder in nestedBuilders)
                    {
                        nestedBuilder.Parent = commandBuilder;
                        commandBuilder.Subcommands.Add(nestedBuilder);
                    }
                }
                else
                {
                    builders = null;
                    return false;
                }
            }

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                if (method.GetCustomAttribute<CommandAttribute>() is not CommandAttribute methodCommandAttribute)
                {
                    continue;
                }

                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(CommandContext))
                {
                    builders = null;
                    error = $"The first parameter of method {method.Name} must be of type {nameof(CommandContext)}.";
                    return false;
                }

                commandBuilder.Overloads.Add(new CommandOverload(method, parameters.Select(parameter => new CommandParameter(parameter)), methodCommandAttribute));
            }

            if (commandBuilder.Subcommands.Count + commandBuilder.Overloads.Count == 0)
            {
                builders = null;
                error = $"Type {type.FullName} does not have any subcommands or overloads.";
                return false;
            }
            else if (commandBuilder.Subcommands.Count + commandBuilder.Overloads.Count > 25)
            {
                builders = null;
                error = $"Type {type.FullName} has too many subcommands and overloads. The maximum is 25.";
                return false;
            }
            // If no CommandAttribute was found on the class, then it must have at least one CommandAttribute on a method.
            else if (commandBuilder.Name is null)
            {
                error = null;
                // Group the overloads by name. This should allow for multiple overloads of different commands to be defined in the same class.
                builders = commandBuilder.Overloads.GroupBy(overload => overload.CommandAttribute!.Name).Select(overloads => new CommandBuilder()
                {
                    Name = overloads.First().CommandAttribute?.Name,
                    Aliases = overloads.SelectMany(overload => overload.CommandAttribute!.Aliases).ToList() ?? new List<string>(),
                    Description = overloads.First().Method.GetCustomAttribute<DescriptionAttribute>()?.Description,
                    Overloads = new List<CommandOverload>(overloads)
                });
                return true;
            }
            else
            {
                builders = new List<CommandBuilder>() { commandBuilder };
                error = null;
                return true;
            }
        }

        public Command Build() => TryBuild(out Command? command, out string? error) ? command! : throw new InvalidDataException(error);

        public bool TryBuild([NotNullWhen(true)] out Command? command) => TryBuild(out command, out _);

        public bool TryBuild([NotNullWhen(true)] out Command? command, [NotNullWhen(false)] out string? error)
        {
            command = null;
            error = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                error = "Name cannot be null or whitespace.";
                return false;
            }
            else if (Overloads.Count == 0 && Subcommands.Count == 0)
            {
                error = "Command must have at least one overload or subcommand.";
                return false;
            }

            Name = Name.Trim().Pascalize();
            Description = Description?.Trim() ?? "No description provided.";
            List<string> aliases = new()
            {
                Name,
                Name.Kebaberize(),
                Name.Camelize(),
                Name.Underscore()
            };

            foreach (string alias in Aliases)
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    continue;
                }

                string trimmed = alias.Trim();
                aliases.Add(trimmed.Pascalize());
                aliases.Add(trimmed.Kebaberize());
                aliases.Add(trimmed.Camelize());
                aliases.Add(trimmed.Underscore());
            };

            command = new Command(Name, aliases.Distinct(), Description!, Overloads, Subcommands.Select(subcommand => subcommand.Build()));
            return true;
        }
    }
}
