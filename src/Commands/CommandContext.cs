using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed class CommandContext
    {
        public readonly DiscordChannel Channel;
        public readonly DiscordUser User;

        public readonly DiscordInteraction? Interaction;
        public readonly DiscordMessage? Message;

        public readonly DiscordGuild? Guild;
        public readonly DiscordMember? Member;

        public readonly CommandAllExtension Extension;
        public readonly Command CurrentCommand;
        public readonly CommandOverload CurrentOverload;
        public readonly IDictionary<string, object?> NamedArguments;
        public readonly string RawArguments;

        public bool IsSlashCommand => Interaction != null;
        public InteractionResponseType? LastInteractionResponseType { get; private set; }

        public CommandContext(CommandAllExtension extension, Command currentCommand, DiscordInteraction interaction) : this(interaction.Channel, interaction.User, interaction, null, interaction.Guild, interaction.User as DiscordMember, extension, currentCommand, string.Empty)
        {

        }

        public CommandContext(CommandAllExtension extension, Command currentCommand, DiscordMessage message, string rawArguments) : this(message.Channel, message.Author, null, message, message.Channel.Guild, message.Author as DiscordMember, extension, currentCommand, rawArguments)
        {
            if (!extension.TextArgumentParser.TryExtractArguments(rawArguments, out IReadOnlyList<string> arguments))
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
                IEnumerable<(CommandParameter, string)> namedArguments = overload.Parameters.Skip(1).Zip(arguments.Union(new string[overload.Parameters.Count - arguments.Count - 1]), (param, arg) => (param, arg));
                foreach ((CommandParameter param, string? arg) in namedArguments)
                {
                    if (arg is null)
                    {
                        if (param.Flags.HasFlag(CommandParameterFlags.Optional))
                        {
                            NamedArguments.Add(param.Name, param.DefaultValue);
                            continue;
                        }
                        else
                        {
                            throw new ArgumentException("Not enough arguments were provided.", nameof(rawArguments));
                        }
                    }

                    if (ActivatorUtilities.CreateInstance(extension.ServiceProvider, param.ArgumentConverterType!) is not IArgumentConverter converter)
                    {
                        throw new InvalidOperationException($"Failed to create an instance of {param.ArgumentConverterType}. Does the argument converter have a public constructor? Were all the services able to be resolved?");
                    }
                    Task<IOptional> optionalTask = converter.ConvertAsync(this, param, arg);
                    optionalTask.Wait();
                    if (!optionalTask.IsCompletedSuccessfully)
                    {
                        throw new InvalidOperationException($"Failed to convert argument {arg} to type {param.Type}.", optionalTask.Exception);
                    }
                    else if (optionalTask.Result.HasValue)
                    {
                        NamedArguments.Add(param.Name, optionalTask.Result.RawValue);
                    }
                    else if (param.Flags.HasFlag(CommandParameterFlags.Optional))
                    {
                        NamedArguments.Add(param.Name, param.DefaultValue);
                    }
                }
            }
        }

        private CommandContext(DiscordChannel channel, DiscordUser user, DiscordInteraction? interaction, DiscordMessage? message, DiscordGuild? guild, DiscordMember? member, CommandAllExtension extension, Command currentCommand, string rawArguments)
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
            NamedArguments = new Dictionary<string, object?>();
        }

        public Task ReplyAsync(DiscordMessageBuilder messageBuilder)
        {
            if (IsSlashCommand)
            {
                LastInteractionResponseType = InteractionResponseType.ChannelMessageWithSource;
                return Interaction!.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(messageBuilder));
            }
            else
            {
                return Channel.SendMessageAsync(messageBuilder);
            }
        }

        public Task DelayAsync()
        {
            if (IsSlashCommand)
            {
                LastInteractionResponseType = InteractionResponseType.DeferredChannelMessageWithSource;
                return Interaction!.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            }
            else
            {
                // Does not last as long as slash command
                return Message!.Channel.TriggerTypingAsync();
            }
        }

        public Task EditAsync(DiscordMessageBuilder messageBuilder)
        {
            if (IsSlashCommand)
            {
                LastInteractionResponseType = LastInteractionResponseType is InteractionResponseType.DeferredChannelMessageWithSource
                    ? InteractionResponseType.DeferredMessageUpdate
                    : InteractionResponseType.UpdateMessage;

                return Interaction!.CreateResponseAsync(LastInteractionResponseType.Value, new DiscordInteractionResponseBuilder(messageBuilder));
            }
            else
            {
                return Message!.ModifyAsync(messageBuilder);
            }
        }
    }
}
