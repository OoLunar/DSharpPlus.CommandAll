using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Enums;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// The context of a command.
    /// </summary>
    public sealed class CommandContext
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
        public readonly DiscordInteraction? Interaction;

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
        /// Whether the command was executed via slash command or not.
        /// </summary>
        public bool IsSlashCommand => Interaction != null;

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
            NamedArguments = new Dictionary<CommandParameter, object?>();

            DiscordInteractionDataOption[] arguments = options.ToArray();
            for (int i = 0; i < arguments.Length; i++)
            {
                CommandParameter parameter = i < CurrentOverload.Parameters.Count ? CurrentOverload.Parameters[i] : CurrentOverload.Parameters[^1];
                IOptional optional = Convert(parameter!, arguments[i]);
                object? value;
                if (optional.HasValue)
                {
                    _logger.LogTrace("Successfully converted argument {Argument} to {Type}", arguments[i], parameter!.ParameterInfo.ParameterType);
                    value = optional.RawValue;
                }
                else if (parameter!.Flags.HasFlag(CommandParameterFlags.Optional) && parameter.DefaultValue.HasValue)
                {
                    _logger.LogDebug("Adding parameter {Parameter}'s default value because it failed conversion.", parameter);
                    value = parameter.DefaultValue.Value;
                }
                else
                {
                    throw new ArgumentException($"Failed to convert argument {i} to {parameter.ParameterInfo.ParameterType}.", nameof(options));
                }

                if (parameter.Flags.HasFlag(CommandParameterFlags.Params))
                {
                    if (NamedArguments.TryGetValue(parameter, out object? existing))
                    {
                        if (existing is IList list)
                        {
                            list[arguments.Length - i - 1] = value;
                        }
                    }
                    else
                    {
                        IList list = Array.CreateInstance(parameter.ParameterInfo.ParameterType.GetElementType()!, arguments.Length - i);
                        list[arguments.Length - i - 1] = value;
                        NamedArguments.Add(parameter, list);
                    }
                }
                else
                {
                    NamedArguments.Add(parameter, value);
                }
            }

            if (NamedArguments.Values.Last() is Array array)
            {
                Array.Reverse(array);
            }

        }

        /// <summary>
        /// Creates a new <see cref="CommandContext"/> from a text command message.
        /// </summary>
        [SuppressMessage("Roslyn", "IDE0045", Justification = "Clarity on an already confusing assignment in if statement.")]
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
                for (int i = 0; i < arguments.Count; i++)
                {
                    CommandParameter parameter = i < CurrentOverload.Parameters.Count ? CurrentOverload.Parameters[i] : CurrentOverload.Parameters[^1];
                    IOptional optional = Convert(parameter!, arguments[i]);
                    object? value;
                    if (optional.HasValue)
                    {
                        _logger.LogTrace("Successfully converted argument {Argument} to {Type}", arguments[i], parameter!.ParameterInfo.ParameterType);
                        value = optional.RawValue;
                    }
                    else if (parameter!.Flags.HasFlag(CommandParameterFlags.Optional) && parameter.DefaultValue.HasValue)
                    {
                        _logger.LogDebug("Adding parameter {Parameter}'s default value because it failed conversion.", parameter);
                        value = parameter.DefaultValue.Value;
                    }
                    else
                    {
                        throw new ArgumentException($"Failed to convert argument {i} to {parameter.ParameterInfo.ParameterType}.", nameof(rawArguments));
                    }

                    if (parameter.Flags.HasFlag(CommandParameterFlags.Params))
                    {
                        if (NamedArguments.TryGetValue(parameter, out object? existing))
                        {
                            if (existing is IList list)
                            {
                                list[arguments.Count - i - 1] = value;
                            }
                        }
                        else
                        {
                            IList list = Array.CreateInstance(parameter.ParameterInfo.ParameterType.GetElementType()!, arguments.Count - i);
                            list[arguments.Count - i - 1] = value;
                            NamedArguments.Add(parameter, list);
                        }
                    }
                    else
                    {
                        NamedArguments.Add(parameter, value);
                    }
                }

                if (NamedArguments.Values.Last() is Array array)
                {
                    Array.Reverse(array);
                }

                _logger.LogTrace("Successfully parsed arguments {Arguments}", NamedArguments);
            }
        }

        /// <summary>
        /// Creates a new instance of the parameter's argument converter and attempts to convert the argument.
        /// </summary>
        private IOptional Convert(CommandParameter param, object? arg)
        {
            if (ActivatorUtilities.CreateInstance(Extension.ServiceProvider, param.ArgumentConverterType!) is not IArgumentConverter converter)
            {
                throw new InvalidOperationException($"Failed to create an instance of {param.ArgumentConverterType}. Does the argument converter have a public constructor? Were all the services able to be resolved?");
            }

            _logger.LogTrace("Converting argument {Argument} to {Type}", arg, param.ParameterInfo.ParameterType);
            Task<IOptional> optionalTask = converter.ConvertAsync(this, param, arg?.ToString() ?? string.Empty);
            optionalTask.Wait();
            return !optionalTask.IsCompletedSuccessfully
                ? Optional.FromNoValue<object?>()
                : optionalTask.Result;
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
            _logger = extension.ServiceProvider.GetService<ILogger<CommandContext>>() ?? NullLogger<CommandContext>.Instance;
        }

        /// <summary>
        /// Responds to the command with a message.
        /// </summary>
        /// <remarks>
        /// If the command was invoked via a text command and <paramref name="messageBuilder"/> does not have a reply message set, the message that invoked the command will be set as the reply.
        /// </remarks>
        /// <param name="messageBuilder">The message to send.</param>
        public Task ReplyAsync(DiscordMessageBuilder messageBuilder)
        {
            if (IsSlashCommand)
            {
                _logger.LogDebug("Replying to slash command {Id}.", Interaction!.Id);
                LastInteractionResponseType = InteractionResponseType.ChannelMessageWithSource;
                return Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(messageBuilder));
            }
            else
            {
                _logger.LogDebug("Replying to message {Id}.", Message!.Id);
                if (messageBuilder.ReplyId is null)
                {
                    _logger.LogTrace("No reply set, setting reply to text command message id {Id}.", Message.Id);
                    messageBuilder.WithReply(Message.Id);
                }

                if (messageBuilder.Mentions is null || messageBuilder.Mentions.Count == 0)
                {
                    _logger.LogTrace("No mentions explicitly set when replying to text command message id {Id}, automatically preventing any accidental mentions.", Message.Id);
                    messageBuilder.WithAllowedMentions(Mentions.None);
                }

                return Channel.SendMessageAsync(messageBuilder);
            }
        }

        /// <summary>
        /// Responds to the command by letting the user know that the command is still being processed.
        /// </summary>
        /// <remarks>
        /// If the command was invoked via a text command, the bot will instead start "typing" in the channel.
        /// </remarks>
        public Task DelayAsync()
        {
            if (IsSlashCommand)
            {
                _logger.LogDebug("Delaying slash command {Id}.", Interaction!.Id);
                LastInteractionResponseType = InteractionResponseType.DeferredChannelMessageWithSource;
                return Interaction!.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            }
            else
            {
                // Does not last as long as slash command
                _logger.LogDebug("Delaying text command {Id}.", Message!.Id);
                return Message!.Channel.TriggerTypingAsync();
            }
        }

        /// <summary>
        /// Edits the original response to the command.
        /// </summary>
        /// <param name="messageBuilder">The new message content.</param>
        public Task EditAsync(DiscordMessageBuilder messageBuilder)
        {
            if (IsSlashCommand)
            {
                if (LastInteractionResponseType is InteractionResponseType.DeferredChannelMessageWithSource)
                {
                    _logger.LogDebug("Editing deferred slash command {Id}.", Interaction!.Id);
                    LastInteractionResponseType = InteractionResponseType.DeferredMessageUpdate;
                }
                else
                {
                    _logger.LogDebug("Editing slash command {Id}.", Interaction!.Id);
                    LastInteractionResponseType = InteractionResponseType.UpdateMessage;
                }
                return Interaction!.CreateResponseAsync(LastInteractionResponseType.Value, new DiscordInteractionResponseBuilder(messageBuilder));
            }
            else
            {
                _logger.LogDebug("Editing text command response {Id}.", Message!.Id);
                return Message!.ModifyAsync(messageBuilder);
            }
        }

        /// <summary>
        /// Deletes the original response to the command.
        /// </summary>
        public Task DeleteAsync()
        {
            if (IsSlashCommand)
            {
                _logger.LogDebug("Deleting slash command response to interaction {Id}.", Interaction!.Id);
                return Interaction!.DeleteOriginalResponseAsync();
            }
            else
            {
                _logger.LogDebug("Deleting text command response to message id {Id}.", Message!.Id);
                return Message!.DeleteAsync();
            }
        }

        public override string? ToString() => $"{CurrentCommand.FullName} {RawArguments} ({(IsSlashCommand ? Interaction!.Id : Message!.Id)})";
    }
}
