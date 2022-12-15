using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed partial class CommandContext
    {
        /// <summary>
        /// The channel the command was executed in.
        /// </summary>
        public readonly DiscordChannel Channel;

        /// <summary>
        /// Who executed the command.
        /// </summary>
        public readonly DiscordUser User;

        /// <summary>
        /// The interaction that triggered the command, if the command was executed via slash command.
        /// </summary>
        public DiscordInteraction? Interaction { get; private set; }

        /// <summary>
        /// The message that triggered the command, if the command was executed via a text command.
        /// </summary>
        public readonly DiscordMessage? Message;

        /// <summary>
        /// The guild that the command was executed in, if any.
        /// </summary>
        public readonly DiscordGuild? Guild;

        /// <summary>
        /// The <see cref="User"/> as a <see cref="DiscordMember"/>, if the command was executed in a guild.
        /// </summary>
        public readonly DiscordMember? Member;

        /// <summary>
        /// The <see cref="CommandAllExtension"/> instance that handled the command parsing and execution.
        /// </summary>
        public readonly CommandAllExtension Extension;

        /// <summary>
        /// The <see cref="Command"/> that is being executed.
        /// </summary>
        public readonly Command CurrentCommand;

        /// <summary>
        /// The <see cref="CommandOverload"/> that is being executed.
        /// </summary>
        public readonly CommandOverload CurrentOverload;

        /// <summary>
        /// The arguments that were passed to the command.
        /// </summary>
        public readonly IDictionary<CommandParameter, object?> NamedArguments;

        /// <summary>
        /// The arguments that were passed to the command, in a string form.
        /// </summary>
        public readonly string RawArguments;

        /// <summary>
        /// If the interaction has been responded to, the value will be what interaction response type was used. Will often be <see cref="InteractionResponseType.DeferredChannelMessageWithSource"/> or <see cref="InteractionResponseType.DeferredMessageUpdate"/>.
        /// </summary>
        public InteractionResponseType? LastInteractionResponseType { get; private set; }

        /// <summary>
        /// The client attached to the <see cref="Extension"/>.
        /// </summary>
        public DiscordClient Client => Extension.Client;

        /// <summary>
        /// A new scope for the command context, created from the <see cref="Extension"/>'s <see cref="IServiceProvider"/>.
        /// </summary>
        public readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// Whether the command was executed via slash command or not.
        /// </summary>
        public bool IsSlashCommand => Interaction is not null;

        /// <summary>
        /// The original response to the message or interaction, if any.
        /// </summary>
        /// <remarks>
        /// Not null when <see cref="IsSlashCommand"/> is false and <see cref="ReplyAsync(DiscordMessageBuilder)"/> has been called.
        /// </remarks>
        public DiscordMessage? Response { get; private set; }

        /// <summary>
        /// Used to log complaints.
        /// </summary>
        private readonly ILogger<CommandContext> _logger;

        /// <summary>
        /// Creates a new <see cref="CommandContext"/> from a slash command interaction.
        /// </summary>
        public CommandContext(CommandAllExtension extension, Command currentCommand, DiscordInteraction interaction, IEnumerable<DiscordInteractionDataOption> options) : this(interaction.Channel, interaction.User, interaction, null, interaction.Guild, interaction.User as DiscordMember, extension, currentCommand, string.Empty)
        {
            CurrentOverload = currentCommand.Overloads[0];
            NamedArguments = ConvertArgs(options.Select(option => option.Value?.ToString() ?? throw new NotImplementedException($"Option {option} has a null value!")).ToArray());
            _logger.LogTrace("Successfully parsed arguments {Arguments}", NamedArguments);
        }

        /// <summary>
        /// Creates a new <see cref="CommandContext"/> from a text command message.
        /// </summary>
        public CommandContext(CommandAllExtension extension, Command currentCommand, DiscordMessage message, string rawArguments) : this(message.Channel, message.Author, null, message, message.Channel.Guild, message.Author as DiscordMember, extension, currentCommand, rawArguments)
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
                NamedArguments = ConvertArgs(arguments.ToArray());
                _logger.LogTrace("Successfully parsed arguments {Arguments}", NamedArguments);
            }
        }

        /// <summary>
        /// Attempts to create a <see cref="CommandContext"/>. This constructor can be used to create a fake context for testing purposes.
        /// </summary>
        public CommandContext(DiscordChannel channel, DiscordUser user, DiscordInteraction? interaction, DiscordMessage? message, DiscordGuild? guild, DiscordMember? member, CommandAllExtension extension, Command currentCommand, string rawArguments)
        {
            Channel = channel;
            User = user;
            Interaction = interaction;
            Message = message;
            Guild = guild;
            Member = member;
            Extension = extension;
            CurrentCommand = currentCommand;
            CurrentOverload = null!; // This will be overriden by both constructors that call this one.
            RawArguments = rawArguments;
            NamedArguments = new Dictionary<CommandParameter, object?>();
            PromptTimeout = extension.PromptTimeout;
            ServiceProvider = extension.ServiceProvider.CreateScope().ServiceProvider;
            _logger = extension.ServiceProvider.GetRequiredService<ILogger<CommandContext>>();
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
                Task<IOptional> optionalTask = converter.ConvertAsync(this, parameter, argument.ToString() ?? string.Empty);
                optionalTask.Wait();
                optional = optionalTask.IsCompletedSuccessfully ? optionalTask.Result : throw new ArgumentException($"Failed to convert argument {i} to {parameter.ParameterInfo.ParameterType}.", nameof(arguments));

                if (!optional.HasValue)
                {
                    if (parameter.Flags.HasFlag(CommandParameterFlags.Optional))
                    {
                        _logger.LogDebug("Adding parameter {Parameter}'s default value because it failed conversion.", parameter);
                        result.Add(parameter, parameter.DefaultValue.Value);
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
                    result.Add(parameter, parameter.DefaultValue.Value);
                }
                else
                {
                    throw new ArgumentException($"Not enough arguments were provided. Expected {CurrentOverload.Parameters.Count}, got {arguments.Length}.", nameof(arguments));
                }
                i++;
            }

            return result;
        }
    }
}