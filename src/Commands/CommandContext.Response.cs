using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    public sealed partial class CommandContext
    {
        /// <summary>
        /// Responds to the command with a message.
        /// </summary>
        /// <remarks>
        /// If the command was invoked via a text command and <paramref name="messageBuilder"/> does not have a reply message set, the message that invoked the command will be set as the reply.
        /// Additionally if <see cref="DiscordMessageBuilder.Mentions"/> is not set, it will be set to <see cref="Mentions.None"/>.
        /// </remarks>
        /// <param name="messageBuilder">The message to send.</param>
        public async Task ReplyAsync(IDiscordMessageBuilder messageBuilder)
        {
            if (IsSlashCommand)
            {
                if (LastInteractionResponseType is not null and not InteractionResponseType.Modal)
                {
                    throw new InvalidOperationException("Cannot respond to a slash command more than once.");
                }

                _logger.LogDebug("Replying to slash command {Id}.", Interaction!.Id);
                LastInteractionResponseType = InteractionResponseType.ChannelMessageWithSource;
                await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(messageBuilder));
            }
            else
            {
                DiscordMessageBuilder builder = new(messageBuilder);
                _logger.LogDebug("Replying to message {Id}.", Message!.Id);
                if (builder.ReplyId is null)
                {
                    _logger.LogTrace("No reply set, setting reply to text command message id {Id}.", Message.Id);
                    builder.WithReply(Message.Id);
                }

                if (builder.Mentions is null || builder.Mentions.Count == 0)
                {
                    _logger.LogTrace("No mentions explicitly set when replying to text command message id {Id}, automatically preventing any accidental mentions.", Message.Id);
                    builder.WithAllowedMentions(Mentions.None);
                }

                Response = await Channel.SendMessageAsync(builder);
            }
        }

        /// <inheritdoc cref="ReplyAsync(DiscordMessageBuilder)"/>
        /// <param name="content">The content to send.</param>
        public Task ReplyAsync(string content) => ReplyAsync(new DiscordMessageBuilder().WithContent(content));

        /// <inheritdoc cref="ReplyAsync(DiscordMessageBuilder)"/>
        /// <param name="embed">The embed to send.</param>
        public Task ReplyAsync(params DiscordEmbedBuilder[] embedBuilders) => ReplyAsync(new DiscordMessageBuilder().AddEmbeds(embedBuilders.Select(embedBuilder => embedBuilder.Build())));

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
                if (LastInteractionResponseType is not null)
                {
                    throw new InvalidOperationException("Cannot delay a command that has already responded.");
                }

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
        public async Task EditAsync(IDiscordMessageBuilder messageBuilder)
        {
            if (IsSlashCommand)
            {
                if (LastInteractionResponseType is InteractionResponseType.DeferredChannelMessageWithSource)
                {
                    _logger.LogDebug("Editing deferred slash command {Id}.", Interaction!.Id);
                    LastInteractionResponseType = InteractionResponseType.UpdateMessage;
                }
                else
                {
                    _logger.LogDebug("Editing slash command {Id}.", Interaction!.Id);
                    LastInteractionResponseType = InteractionResponseType.UpdateMessage;
                }

                await Interaction!.EditOriginalResponseAsync(new DiscordWebhookBuilder(messageBuilder));
            }
            else
            {
                _logger.LogDebug("Editing text command response {Id}.", Message!.Id);
                if (Response is null)
                {
                    DiscordMessageBuilder builder = new(messageBuilder);
                    if (builder.ReplyId is null)
                    {
                        _logger.LogTrace("No reply set, setting reply to text command message id {Id}.", Message.Id);
                        builder.WithReply(Message.Id);
                    }

                    if (builder.Mentions is null || builder.Mentions.Count == 0)
                    {
                        _logger.LogTrace("No mentions explicitly set when replying to text command message id {Id}, automatically preventing any accidental mentions.", Message.Id);
                        builder.WithAllowedMentions(Mentions.None);
                    }
                    Response = await Message.RespondAsync(builder);
                }
                else
                {
                    await Response.ModifyAsync(new DiscordMessageBuilder(messageBuilder));
                }
            }
        }

        /// <inheritdoc cref="EditAsync(DiscordMessageBuilder)"/>
        /// <param name="content">The new message content.</param>
        public Task EditAsync(string content) => EditAsync(new DiscordMessageBuilder().WithContent(content));

        /// <inheritdoc cref="EditAsync(DiscordMessageBuilder)"/>
        /// <param name="embed">The new message content.</param>
        public Task EditAsync(params DiscordEmbedBuilder[] embedBuilders) => EditAsync(new DiscordMessageBuilder().AddEmbeds(embedBuilders.Select(embedBuilder => embedBuilder.Build())));

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
                return Response!.DeleteAsync();
            }
        }

        public Task<DiscordMessage> GetOriginalResponse() => IsSlashCommand ? Interaction!.GetOriginalResponseAsync() : Task.FromResult(Response!);

        public override string? ToString() => $"{CurrentCommand.FullName} {RawArguments} ({(IsSlashCommand ? Interaction!.Id : Message!.Id)})";
    }
}