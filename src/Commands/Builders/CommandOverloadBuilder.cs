using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands.Builders.SlashMetadata;
using DSharpPlus.CommandAll.Commands.Checks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.CommandAll.Exceptions;
using Humanizer;

namespace DSharpPlus.CommandAll.Commands.Builders
{
    /// <summary>
    /// A builder for command overloads.
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
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

        /// <inheritdoc cref="CommandOverload.Checks"/>
        public List<CommandCheckAttribute> Checks { get; set; } = new();


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

        /// <inheritdoc cref="TryParse(CommandAllExtension, MethodInfo, out CommandOverloadBuilder?, out Exception?)"/>
        /// <summary>
        /// Parses a <see cref="CommandOverloadBuilder"/> from a <see cref="MethodInfo"/>.
        /// </summary>
        /// <returns>The <see cref="CommandOverloadBuilder"/> that was parsed.</returns>
        public static CommandOverloadBuilder Parse(CommandAllExtension commandAllExtension, MethodInfo methodInfo) => TryParse(commandAllExtension, methodInfo, out CommandOverloadBuilder? overload, out Exception? error) ? overload : throw error;

        /// <inheritdoc cref="TryParse(CommandAllExtension, MethodInfo, out CommandOverloadBuilder?, out Exception?)"/>
        public static bool TryParse(CommandAllExtension commandAllExtension, MethodInfo methodInfo, [NotNullWhen(true)] out CommandOverloadBuilder? builder) => TryParse(commandAllExtension, methodInfo, out builder, out _);

        /// <summary>
        /// Attempts to parse a <see cref="CommandOverloadBuilder"/> from a <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="commandAllExtension">The <see cref="CommandAllExtension"/> to use when grabbing configuration values.</param>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> to parse from.</param>
        /// <param name="builder">The <see cref="CommandOverloadBuilder"/> that was parsed.</param>
        /// <param name="error">The <see cref="Exception"/> that was found when parsing the <see cref="CommandOverloadBuilder"/>. Not thrown.</param>
        /// <returns>Whether or not the <see cref="CommandOverloadBuilder"/> was parsed successfully.</returns>
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
            foreach (Attribute attribute in methodInfo.GetCustomAttributes().Cast<Attribute>())
            {
                switch (attribute)
                {
                    case CommandOverloadPriorityAttribute overloadPriorityAttribute:
                        builder.Priority = overloadPriorityAttribute.Priority;
                        if (overloadPriorityAttribute.IsSlashPreferred)
                        {
                            builder.Flags |= CommandOverloadFlags.SlashPreferred;
                        }
                        break;
                    case CommandCheckAttribute:
                        builder.Checks.Add((CommandCheckAttribute)attribute);
                        break;
                }
            }

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
            error = null;
            return true;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new(Method.Name);
            stringBuilder.AppendFormat("{0}", Method.Name);

            if (Flags != 0)
            {
                stringBuilder.AppendFormat(" ({0})", Flags.Humanize());
            }

            stringBuilder.AppendFormat(", Priority: {0}, Parameters: {1}, {2}", Priority, Parameters.Count.ToString("N0", CultureInfo.InvariantCulture), base.ToString());
            return stringBuilder.ToString();
        }

        public override bool Equals(object? obj) => obj is CommandOverloadBuilder builder && EqualityComparer<CommandAllExtension>.Default.Equals(CommandAllExtension, builder.CommandAllExtension) && EqualityComparer<MethodInfo>.Default.Equals(Method, builder.Method) && EqualityComparer<List<CommandParameterBuilder>>.Default.Equals(Parameters, builder.Parameters) && Flags == builder.Flags && Priority == builder.Priority && EqualityComparer<CommandOverloadSlashMetadataBuilder>.Default.Equals(SlashMetadata, builder.SlashMetadata);
        public override int GetHashCode() => HashCode.Combine(CommandAllExtension, Method, Parameters, Flags, Priority, SlashMetadata);
    }
}
