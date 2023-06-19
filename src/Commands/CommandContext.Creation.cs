using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.CommandAll.Converters;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DSharpPlus.CommandAll.Commands
{
    public sealed partial record CommandContext
    {
        /// <summary>
        /// Used to log complaints.
        /// </summary>
        private readonly ILogger<CommandContext> _logger;

        /// <summary>
        /// A new scope for the command context, created from the <see cref="Extension"/>'s <see cref="IServiceProvider"/>.
        /// </summary>
        public IServiceProvider ServiceProvider { get; init; }

        /// <summary>
        /// The <see cref="CommandAllExtension"/> instance that handled the command parsing and execution.
        /// </summary>
        public CommandAllExtension Extension { get; init; }

        /// <summary>
        /// The client attached to the <see cref="Extension"/>.
        /// </summary>
        public DiscordClient Client => Extension.Client;

        /// <summary>
        /// The <see cref="Command"/> that is being executed.
        /// </summary>
        public Command CurrentCommand => CurrentOverload.Command;

        /// <summary>
        /// The <see cref="CommandOverload"/> that is being executed.
        /// </summary>
        public CommandOverload CurrentOverload { get; init; }

        /// <summary>
        /// How the command was invoked.
        /// </summary>
        public CommandInvocationType InvocationType { get; init; }

        /// <summary>
        /// The arguments that were passed to the command.
        /// </summary>
        public IReadOnlyDictionary<CommandParameter, object?> NamedArguments => _namedArguments.AsReadOnly();
        private readonly Dictionary<CommandParameter, object?> _namedArguments;

        /// <summary>
        /// The channel the command was executed in.
        /// </summary>
        public DiscordChannel Channel { get; init; }

        /// <summary>
        /// Who executed the command.
        /// </summary>
        public DiscordUser User { get; init; }

        /// <summary>
        /// The guild that the command was executed in, if any.
        /// </summary>
        public DiscordGuild? Guild { get; init; }

        /// <summary>
        /// The <see cref="User"/> as a <see cref="DiscordMember"/>, if the command was executed in a guild.
        /// </summary>
        public DiscordMember? Member => User as DiscordMember;

        /// <summary>
        /// The message that triggered the command, if the command was executed via a text command.
        /// </summary>
        public DiscordMessage? Message { get; init; }

        /// <summary>
        /// The interaction that triggered the command, if the command was executed via slash command.
        /// </summary>
        public DiscordInteraction? Interaction { get; private set; }

        /// <summary>
        /// The original response to the message or interaction, if any.
        /// </summary>
        /// <remarks>
        /// <see langword="null"/> when <see cref="ResponseType"/> is <see cref="ContextResponseType.None"/>.
        /// </remarks>
        public DiscordMessage? Response { get; private set; }

        /// <summary>
        /// If the interaction has been responded to, the value will be what interaction response type was used. Will often be <see cref="InteractionResponseType.DeferredChannelMessageWithSource"/> or <see cref="InteractionResponseType.DeferredMessageUpdate"/>.
        /// </summary>
        public ContextResponseType ResponseType { get; private set; }

        /// <summary>
        /// Creates a new <see cref="CommandContext"/> from a slash command interaction.
        /// </summary>
        public CommandContext(CommandAllExtension extension, Command currentCommand, DiscordInteraction interaction, IEnumerable<DiscordInteractionDataOption> options) : this(interaction.Channel, interaction.User, interaction, null, interaction.Guild, extension, currentCommand.Overloads[0], CommandInvocationType.SlashCommand)
        {
            Dictionary<string, string?> parameters = new();
            foreach (CommandParameter parameter in CurrentOverload.Parameters)
            {
                foreach (string name in parameter.SlashNames)
                {
                    foreach (DiscordInteractionDataOption option in options)
                    {
                        if (option.Name == name)
                        {
                            parameters.Add(name, option.Value?.ToString());
                            break;
                        }
                    }

                    if (!parameters.ContainsKey(name) && !parameter.Flags.HasFlag(CommandParameterFlags.TrimExcess))
                    {
                        parameters.Add(name, null);
                    }
                }
            }

            _namedArguments = ConvertArgs(parameters.Values.ToArray()!);
            _logger.LogTrace("Successfully parsed arguments {Arguments}", _namedArguments);
        }

        /// <summary>
        /// Creates a new <see cref="CommandContext"/> from a text command message.
        /// </summary>
        public CommandContext(CommandAllExtension extension, Command currentCommand, DiscordMessage message, string rawArguments) : this(message.Channel, message.Author, null, message, message.Channel.Guild, extension, null!, CommandInvocationType.TextCommand)
        {
            if (!extension.TextArgumentParser.TryExtractArguments(extension, rawArguments, out IReadOnlyList<string> arguments))
            {
                throw new ArgumentException("Failed to parse arguments.", nameof(rawArguments));
            }
            else if (!extension.CommandOverloadParser.TryParseOverload(currentCommand, arguments, out CommandOverload? overload))
            {
                throw new ArgumentException("Failed to find a valid overload.", nameof(rawArguments));
            }
            else
            {
                CurrentOverload = overload;
                _namedArguments = ConvertArgs(arguments.ToArray());
                _logger.LogTrace("Successfully parsed arguments {Arguments}", _namedArguments);
            }
        }

        /// <summary>
        /// Attempts to create a <see cref="CommandContext"/>. This constructor can be used to create a fake context for testing purposes.
        /// </summary>
        public CommandContext(DiscordChannel channel, DiscordUser user, DiscordInteraction? interaction, DiscordMessage? message, DiscordGuild? guild, CommandAllExtension extension, CommandOverload currentOverload, CommandInvocationType invocationType = CommandInvocationType.VirtualCommand)
        {
            ServiceProvider = extension.ServiceProvider.CreateScope().ServiceProvider;
            Extension = extension;
            CurrentOverload = currentOverload;
            _namedArguments = new();
            InvocationType = invocationType;
            Channel = channel;
            User = user;
            Interaction = interaction;
            Message = message;
            Guild = guild;
            _logger = ServiceProvider.GetRequiredService<ILogger<CommandContext>>();
        }

        private Dictionary<CommandParameter, object?> ConvertArgs(object[] arguments)
        {
            Dictionary<CommandParameter, object?> result = new();
            IList? paramsList = null;
            CommandParameter? parameter = null;
            int i;
            for (i = 0; i < arguments.Length; i++)
            {
                IOptional optional;
                object? argument = arguments[i];

                // Get the parameter i is for, or the last parameter if there are more arguments than parameters.
                // We can assume that the last parameter has the params flag, because we already checked that in CommandOverloadParser.
                parameter = i < CurrentOverload.Parameters.Count ? CurrentOverload.Parameters[i] : CurrentOverload.Parameters[^1];

                // TODO: Hold onto the argument converters when all the services they ask for are singletons. This can
                //    probably be done in the ArgumentConverterManager class by calling a method during the READY event.
                // Attempt to convert the value to the parameter's type.
                if (ActivatorUtilities.CreateInstance(ServiceProvider, parameter.ArgumentConverterType!) is not IArgumentConverter converter)
                {
                    throw new InvalidOperationException($"Failed to create an instance of {parameter.ArgumentConverterType}. Does the argument converter have a public constructor? Were all the services able to be resolved?");
                }

                _logger.LogTrace("Converting argument {Argument} to {Type}", argument, parameter.ParameterInfo.ParameterType);
                Task<IOptional> optionalTask = converter.ConvertAsync(this, argument?.ToString() ?? string.Empty, parameter);
                optionalTask.Wait();
                optional = optionalTask.IsCompletedSuccessfully ? optionalTask.Result : throw new ArgumentException($"Failed to convert argument {i} to {parameter.ParameterInfo.ParameterType}.", nameof(arguments));

                if (!optional.HasValue)
                {
                    if (parameter.Flags.HasFlag(CommandParameterFlags.Optional))
                    {
                        _logger.LogDebug("Adding parameter {Parameter}'s default value because it failed conversion.", parameter);
                        result.Add(parameter, parameter.DefaultValue);
                    }
                    else
                    {
                        throw new ArgumentException($"Failed to convert argument {i} to {parameter.ParameterInfo.ParameterType}.", nameof(arguments));
                    }
                }
                else if (!parameter.Flags.HasFlag(CommandParameterFlags.Params) && !parameter.Flags.HasFlag(CommandParameterFlags.RemainingText))
                {
                    _logger.LogTrace("Successfully converted argument {Argument} to {Type}", argument, parameter!.ParameterInfo.ParameterType);
                    result.Add(parameter, optional.RawValue);
                }
                else
                {
                    paramsList ??= Array.CreateInstance(parameter.ParameterInfo.ParameterType.GetElementType() ?? parameter.ParameterInfo.ParameterType, arguments.Length - i);
                    paramsList[arguments.Length - i - 1] = optional.RawValue;
                }
            }

            if (paramsList is Array array)
            {
                Array.Reverse(array);

                if (parameter!.Flags.HasFlag(CommandParameterFlags.Params))
                {
                    result.Add(parameter, array);
                }
                else
                {
                    result.Add(parameter, string.Join(' ', arguments));
                }
            }

            while (i < CurrentOverload.Parameters.Count)
            {
                parameter = CurrentOverload.Parameters[i];
                if (parameter.Flags.HasFlag(CommandParameterFlags.Optional))
                {
                    _logger.LogDebug("Adding parameter {Parameter}'s default value because it was not provided.", parameter);
                    result.Add(parameter, parameter.DefaultValue);
                }
                else
                {
                    throw new ArgumentException($"Not enough arguments were provided. Expected {CurrentOverload.Parameters.Count}, got {arguments.Length}.", nameof(arguments));
                }
                i++;
            }

            return result;
        }

        public override string ToString() => $"CommandContext: {User} - {InvocationType} - {CurrentCommand} - {CurrentOverload}";
    }
}
