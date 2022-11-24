using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Exceptions;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Builders
{
    public sealed class CommandOverloadBuilder
    {
        private static readonly Type _commandContextType = typeof(CommandContext);

        public MethodInfo Method { get; set; } = null!;
        public List<CommandParameterBuilder> Parameters { get; set; } = new();
        public CommandOverloadFlags Flags { get; set; }
        public int Priority { get; set; }
        public bool IsSlashPreferred { get; set; }

        [MemberNotNull(nameof(Method), nameof(Parameters))]
        public void Verify()
        {
            if (!TryVerify(out Exception? error))
            {
                throw error;
            }
        }

        [MemberNotNullWhen(true, nameof(Method), nameof(Parameters))]
        public bool TryVerify() => TryVerify(out _);

        [MemberNotNullWhen(true, nameof(Method), nameof(Parameters))]
        public bool TryVerify([NotNullWhen(false)] out Exception? error)
        {
            if (Method is null)
            {
                error = new PropertyNullException(nameof(Method));
                return false;
            }
            else if (Parameters is null)
            {
                error = new PropertyNullException(nameof(Parameters));
                return false;
            }
            else if (Parameters.Count is 0 or > 25)
            {
                error = new PropertyOutOfRangeException(nameof(Parameters), 1, 25, Parameters.Count);
                return false;
            }
            else if (Parameters[0].ParameterInfo!.ParameterType.IsAssignableFrom(_commandContextType))
            {
                error = new InvalidPropertyStateException(nameof(Parameters), "The command context parameter must not be included in the parameter list!");
                return false;
            }

            // Verify all of the parameters don't have any issues.
            for (int i = 0; i < Parameters.Count; i++)
            {
                CommandParameterBuilder parameterBuilder = Parameters[i];
                if (!parameterBuilder.TryVerify(out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        public static CommandOverloadBuilder Parse(MethodInfo methodInfo) => TryParse(methodInfo, out CommandOverloadBuilder? overload, out Exception? error) ? overload : throw error;
        public static bool TryParse(MethodInfo methodInfo, [NotNullWhen(true)] out CommandOverloadBuilder? builder) => TryParse(methodInfo, out builder, out _);
        public static bool TryParse(MethodInfo methodInfo, [NotNullWhen(true)] out CommandOverloadBuilder? builder, [NotNullWhen(false)] out Exception? error)
        {
            if (methodInfo is null)
            {
                error = new ArgumentNullException(nameof(methodInfo));
                builder = null;
                return false;
            }
            else if (methodInfo.GetCustomAttribute<CommandAttribute>() is null)
            {
                error = new InvalidOperationException("The method must have a CommandAttribute!");
                builder = null;
                return false;
            }

            builder = new() { Method = methodInfo };
            List<CommandParameterBuilder> parameterBuilders = new();
            ParameterInfo[] parameters = methodInfo.GetParameters();
            for (int i = 1; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                if (!CommandParameterBuilder.TryParse(parameter, out CommandParameterBuilder? parameterBuilder, out error))
                {
                    builder = null;
                    return false;
                }

                parameterBuilders.Add(parameterBuilder);
            }

            builder.Parameters = parameterBuilders;
            if (methodInfo.GetCustomAttribute<CommandOverloadPriorityAttribute>() is CommandOverloadPriorityAttribute priorityAttribute)
            {
                builder.Priority = priorityAttribute.Priority;
                builder.IsSlashPreferred = priorityAttribute.IsSlashPreferred;
            }
            else
            {
                builder.Priority = 0;
            }

            error = null;
            return true;
        }
    }
}
