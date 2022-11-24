using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public CommandOverload(CommandOverloadBuilder builder, Command command)
        {
            builder.Verify();
            Command = command ?? throw new ArgumentNullException(nameof(command));
            Method = builder.Method;
            Priority = builder.Priority;
            Flags = builder.Flags;
            Parameters = builder.Parameters.Select(parameterBuilder => new CommandParameter(parameterBuilder, this)).ToArray();
        }
    }
}
