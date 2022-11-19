using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Attributes;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    [DebuggerDisplay("{" + nameof(DebugDisplay) + ",nq}")]
    public sealed class CommandOverload
    {
        public readonly MethodInfo Method;
        public readonly IReadOnlyList<CommandParameter> Parameters;
        public readonly CommandAttribute CommandAttribute;
        public readonly int? Priority;
        public CommandFlags Flags { get; internal set; }

        public CommandOverload(MethodInfo method, IEnumerable<CommandParameter>? parameters = null, CommandAttribute? commandAttribute = null, OverloadPriorityAttribute? priorityAttribute = null)
        {
            Method = method;
            Parameters = new List<CommandParameter>(parameters ?? method.GetParameters().Select(parameter => new CommandParameter(parameter))).AsReadOnly();
            CommandAttribute = commandAttribute ?? method.GetCustomAttribute<CommandAttribute>() ?? throw new ArgumentException("Command overload must have a CommandAttribute.", nameof(commandAttribute));
            Priority = (priorityAttribute ?? method.GetCustomAttribute<OverloadPriorityAttribute>())?.Priority;

            if (Parameters.Count == 0 || Parameters[0].ParameterInfo.ParameterType != typeof(CommandContext))
            {
                throw new ArgumentException($"First parameter must be of type {nameof(CommandContext)}.", nameof(parameters));
            }
        }

        // [Command("help")]
        // public async Task HelpCommand(CommandContext ctx, [Description("The command to get help for.")] string? command = null)
        // ->
        // help, OoLunar.DSharpPlus.Examples.HelloWorld.Commands.HelpCommand(ctx, command)
        private string DebugDisplay() => $"{CommandAttribute.Name}, {ReflectionUtilities.GetFullname(Method)}({Parameters.Select(param => param.Name).Aggregate((a, b) => $"{a}, {b}")})";
    }
}
