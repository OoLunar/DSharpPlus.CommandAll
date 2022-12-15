using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Humanizer;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands
{
    /// <summary>
    /// Used to build a new top level command, subcommand or subcommand group.
    /// </summary>
    [DebuggerDisplay("{ToString()},nq")]
    public sealed class CommandBuilder : Builder
    {
        /// <inheritdoc cref="Command.Name"/>
        public string? Name { get; set; }

        /// <inheritdoc cref="Command.Description"/>
        public string? Description { get; set; }

        /// <inheritdoc cref="Command.Overloads"/>
        public List<CommandOverloadBuilder> Overloads { get; set; } = new List<CommandOverloadBuilder>();

        /// <inheritdoc cref="Command.Subcommands"/>
        public List<CommandBuilder> Subcommands { get; set; } = new List<CommandBuilder>();

        /// <inheritdoc cref="Command.Aliases"/>
        public List<string> Aliases { get; set; } = new();

        /// <inheritdoc cref="Command.Flags"/>
        public CommandFlags Flags { get; set; }

        /// <inheritdoc cref="Command.SlashMetadata"/>
        public CommandSlashMetadataBuilder SlashMetadata { get; set; }

        /// <inheritdoc cref="Command.Parent"/>
        public CommandBuilder? Parent { get; set; }

        /// <inheritdoc cref="Command.FullName"/>
        public string? FullName => Parent is null ? Name : $"{Parent.FullName} {Name}";

        /// <inheritdoc/>
        public CommandBuilder(CommandAllExtension commandAllExtension) : base(commandAllExtension) => SlashMetadata = new(commandAllExtension);

        /// <summary>
        /// Organizes the command's overloads, sorting it from the overload priority, then by the number of parameters.
        /// </summary>
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
                if (overload.Flags.HasFlag(CommandOverloadFlags.SlashPreferred))
                {
                    overloads.Insert(0, overload);
                    continue;
                }

                overload.Priority = priority++;
                overloads.Insert(overload.Priority, overload);
            }

            for (int i = 0; i < overloads.Count; i++)
            {
                // If this is the first overload
                if (i == 0)
                {
                    // Set the SlashPreferred flag
                    overloads[i].Flags |= CommandOverloadFlags.SlashPreferred;
                }
                else
                {
                    // Remove the SlashPreferred flag
                    overloads[i].Flags &= ~CommandOverloadFlags.SlashPreferred;
                }
            }
            Overloads = overloads;
        }

        public IReadOnlyList<CommandParameterBuilder> GetAllParameters()
        {
            List<CommandParameterBuilder> parameters = new();
            foreach (CommandOverloadBuilder overload in Overloads)
            {
                parameters.AddRange(overload.Parameters);
            }

            foreach (CommandBuilder subcommand in Subcommands)
            {
                parameters.AddRange(subcommand.GetAllParameters());
            }

            return parameters.AsReadOnly();
        }

        /// <inheritdoc/>
        [MemberNotNull(nameof(Name), nameof(Description))]
        public override void Verify()
        {
            if (!TryVerify(out Exception? error))
            {
                throw error;
            }
        }

        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(Name), nameof(Description))]
        public override bool TryVerify() => TryVerify(out _);

        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(Name), nameof(Description))]
        public override bool TryVerify([NotNullWhen(false)] out Exception? error)
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

            if (!SlashMetadata.TryVerify(out error))
            {
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Parses a type into a command builder through reflection and recursion.
        /// </summary>
        /// <param name="type">The type to parse.</param>
        /// <param name="commandAllExtension">The command all extension.</param>
        public static IEnumerable<CommandBuilder> Parse(CommandAllExtension commandAllExtension, Type type) => TryParse(commandAllExtension, type, 0, out IEnumerable<CommandBuilder>? builders, out Exception? error) ? builders : throw error;

        /// <inheritdoc cref="Parse(Type)"/>
        /// <param name="builders">The command builders that were parsed.</param>
        /// <returns>Whether or not the type was parsed successfully.</returns>
        public static bool TryParse(CommandAllExtension commandAllExtension, Type type, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders) => TryParse(commandAllExtension, type, 0, out builders, out _);

        /// <inheritdoc cref="TryParse(Type, out IEnumerable{CommandBuilder}?)"/>
        /// <param name="error">The error that occurred, if any.</param>
        public static bool TryParse(CommandAllExtension commandAllExtension, Type type, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders, [NotNullWhen(false)] out Exception? error) => TryParse(commandAllExtension, type, 0, out builders, out error);

        /// <inheritdoc cref="TryParse(Type, out IEnumerable{CommandBuilder}?, out Exception?)"/>
        /// <remarks>
        /// If you cannot comprehend the magic of recursion, you should NOT attempt to modify it. You have been warned.
        /// </remarks>
        /// <param name="recursionLevel">The current level of recursion. Should never exceed 2. Remember this is zero based.</param>
        private static bool TryParse(CommandAllExtension commandAllExtension, Type type, int? recursionLevel, [NotNullWhen(true)] out IEnumerable<CommandBuilder>? builders, [NotNullWhen(false)] out Exception? error)
        {
            if (type is null)
            {
                error = new ArgumentNullException(nameof(type));
                builders = null;
                return false;
            }
            else if (!typeof(BaseCommand).IsAssignableFrom(type))
            {
                error = new InvalidCastException($"The type {type.FullName} must be assignable from {nameof(BaseCommand)}!");
                builders = null;
                return false;
            }
            else if (recursionLevel == 2)
            {
                error = new InvalidOperationException("Groups must not have subgroups! The maximum amount of nested classes is 1.");
                builders = null;
                return false;
            }

            recursionLevel ??= 0;

            // Parse overloads
            List<(string, IEnumerable<string>, CommandOverloadBuilder)> overloads = new();
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                if (method.GetCustomAttribute<CommandAttribute>() is CommandAttribute commandAttribute)
                {
                    // The method was marked with the command attribute which means there is a user error if it fails.
                    if (!CommandOverloadBuilder.TryParse(commandAllExtension, method, out CommandOverloadBuilder? overload, out error))
                    {
                        builders = null;
                        return false;
                    }

                    overloads.Add(new(commandAttribute.Name, commandAttribute.Aliases, overload));
                }
            }

            List<CommandBuilder> commandBuilders = new();
            foreach (IGrouping<string, (string, IEnumerable<string>, CommandOverloadBuilder)> value in overloads.GroupBy(x => x.Item1))
            {
                CommandBuilder builder = new(commandAllExtension)
                {
                    Name = value.Key,
                    Aliases = new(value.SelectMany(x => x.Item2)),
                    Overloads = new(value.Select(x => x.Item3))
                };
                builder.NormalizeOverloadPriorities();

                // Take the description from the first overload that has it.
                foreach (CommandOverloadBuilder overload in builder.Overloads)
                {
                    overload.Command = builder;
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
                if (!TryParse(commandAllExtension, subType, recursionLevel + 1, out IEnumerable<CommandBuilder>? subBuilders, out error))
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
                CommandBuilder builder = new(commandAllExtension)
                {
                    Name = commandAttribute.Name,
                    Aliases = commandAttribute.Aliases.ToList(),
                    Description = type.GetCustomAttribute<DescriptionAttribute>()?.Description,
                    Subcommands = new(commandBuilders),
                };

                foreach (CommandBuilder subcommand in builder.Subcommands)
                {
                    subcommand.Parent = builder;
                }

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

        public override string ToString() => $"{Name}{(Flags == 0 ? string.Empty : $" ({Flags.Humanize()})")} - {Description}";
        public override bool Equals(object? obj) => obj is CommandBuilder builder && EqualityComparer<CommandAllExtension>.Default.Equals(CommandAllExtension, builder.CommandAllExtension) && Name == builder.Name && Description == builder.Description && EqualityComparer<List<CommandOverloadBuilder>>.Default.Equals(Overloads, builder.Overloads) && EqualityComparer<List<CommandBuilder>>.Default.Equals(Subcommands, builder.Subcommands) && EqualityComparer<List<string>>.Default.Equals(Aliases, builder.Aliases) && Flags == builder.Flags && EqualityComparer<CommandSlashMetadataBuilder>.Default.Equals(SlashMetadata, builder.SlashMetadata);
        public override int GetHashCode()
        {
            HashCode hashCode = new();
            hashCode.Add(CommandAllExtension);
            if (Name is not null)
            {
                hashCode.Add(Name);
            }

            if (Description is not null)
            {
                hashCode.Add(Description);
            }

            hashCode.Add(Overloads);
            hashCode.Add(Subcommands);
            hashCode.Add(Aliases);
            hashCode.Add(Flags);
            hashCode.Add(SlashMetadata);
            return hashCode.ToHashCode();
        }
    }
}
