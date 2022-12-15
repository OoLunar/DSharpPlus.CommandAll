using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    /// A builder for command overloads.
    /// </summary>
    [DebuggerDisplay("ToString(),nq")]
    public sealed class CommandOverloadBuilder : Builder
    {
        /// <inheritdoc cref="CommandOverload.Method"/>
        public MethodInfo Method { get; set; } = null!;

        /// <inheritdoc cref="CommandOverload.Parameters"/>
        public List<CommandParameterBuilder> Parameters { get; set; } = new();

        /// <inheritdoc cref="CommandOverload.Flags"/>
        public CommandOverloadFlags Flags { get; set; }

        /// <inheritdoc cref="CommandOverload.Priority"/>
        public int Priority { get; set; }

        /// <inheritdoc cref="CommandOverload.SlashMetadata"/>
        public CommandOverloadSlashMetadataBuilder SlashMetadata { get; set; }

        /// <inheritdoc cref="CommandOverload.Command"/>
        public CommandBuilder? Command { get; set; }

        /// <inheritdoc/>
        public CommandOverloadBuilder(CommandAllExtension commandAllExtension) : base(commandAllExtension) => SlashMetadata = new(commandAllExtension);

        /// <inheritdoc/>
        [MemberNotNull(nameof(Method), nameof(Parameters))]
        public override void Verify()
        {
            if (!TryVerify(out Exception? error))
            {
                throw error;
            }
        }

        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(Method), nameof(Parameters))]
        public override bool TryVerify() => TryVerify(out _);

        /// <inheritdoc/>
        [MemberNotNullWhen(true, nameof(Method), nameof(Parameters))]
        public override bool TryVerify([NotNullWhen(false)] out Exception? error)
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
            else if (Parameters.Count > 25)
            {
                error = new PropertyOutOfRangeException(nameof(Parameters), 0, 25, Parameters.Count);
                return false;
            }
            else if (!SlashMetadata.TryVerify(out error))
            {
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

        /// <summary>
        /// Attempts to parse a command overload from a method.
        /// </summary>
        /// <param name="method">The method to parse.</param>
        public static CommandOverloadBuilder Parse(CommandAllExtension commandAllExtension, MethodInfo methodInfo) => TryParse(commandAllExtension, methodInfo, out CommandOverloadBuilder? overload, out Exception? error) ? overload : throw error;

        /// <inheritdoc cref="Parse(MethodInfo)"/>
        /// <param name="builder">The parsed command overload.</param>
        /// <returns>Whether the overload was parsed successfully.</returns>
        public static bool TryParse(CommandAllExtension commandAllExtension, MethodInfo methodInfo, [NotNullWhen(true)] out CommandOverloadBuilder? builder) => TryParse(commandAllExtension, methodInfo, out builder, out _);

        /// <inheritdoc cref="TryParse(MethodInfo, out CommandOverloadBuilder?)"/>
        /// <param name="error">The error that occurred while parsing the overload.</param>
        public static bool TryParse(CommandAllExtension commandAllExtension, MethodInfo methodInfo, [NotNullWhen(true)] out CommandOverloadBuilder? builder, [NotNullWhen(false)] out Exception? error)
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

            builder = new(commandAllExtension) { Method = methodInfo };
            List<CommandParameterBuilder> parameterBuilders = new();
            ParameterInfo[] parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                if (i == 0)
                {
                    if (!typeof(CommandContext).IsAssignableTo(parameter.ParameterType))
                    {
                        error = new InvalidPropertyStateException(nameof(Parameters), "The command context parameter must not be included in the parameter list!");
                        builder = null;
                        return false;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!CommandParameterBuilder.TryParse(commandAllExtension, parameter, out CommandParameterBuilder? parameterBuilder, out error))
                {
                    builder = null;
                    return false;
                }

                parameterBuilder.OverloadBuilder = builder;
                parameterBuilders.Add(parameterBuilder);
            }

            builder.Parameters = parameterBuilders;
            if (methodInfo.GetCustomAttribute<CommandOverloadPriorityAttribute>() is CommandOverloadPriorityAttribute priorityAttribute)
            {
                builder.Priority = priorityAttribute.Priority;
                if (priorityAttribute.IsSlashPreferred)
                {
                    builder.Flags |= CommandOverloadFlags.SlashPreferred;
                }
            }
            else
            {
                builder.Priority = 0;
            }

            error = null;
            return true;
        }

        public override string ToString() => $"{Method.Name}{(Flags == 0 ? string.Empty : $" ({Flags.Humanize()})")}, Priority: {Priority}, Parameters: {Parameters.Humanize()}";
        public override bool Equals(object? obj) => obj is CommandOverloadBuilder builder && EqualityComparer<CommandAllExtension>.Default.Equals(CommandAllExtension, builder.CommandAllExtension) && EqualityComparer<MethodInfo>.Default.Equals(Method, builder.Method) && EqualityComparer<List<CommandParameterBuilder>>.Default.Equals(Parameters, builder.Parameters) && Flags == builder.Flags && Priority == builder.Priority && EqualityComparer<CommandOverloadSlashMetadataBuilder>.Default.Equals(SlashMetadata, builder.SlashMetadata);
        public override int GetHashCode() => HashCode.Combine(CommandAllExtension, Method, Parameters, Flags, Priority, SlashMetadata);
    }
}
