using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands
{
    public sealed partial class CommandContext
    {
        /// <inheritdoc cref="ReplyAsync(IDiscordMessageBuilder)"/>
        /// <param name="content">The content to send.</param>
        public Task ReplyAsync(string content) => ReplyAsync(new DiscordMessageBuilder().WithContent(content));

        /// <inheritdoc cref="ReplyAsync(IDiscordMessageBuilder)"/>
        /// <param name="embed">The embed to send.</param>
        public Task ReplyAsync(params DiscordEmbedBuilder[] embedBuilders) => ReplyAsync(new DiscordMessageBuilder().AddEmbeds(embedBuilders.Select(embedBuilder => embedBuilder.Build())));

        /// <summary>
        /// Responds to the command with a message.
        /// </summary>
        /// <remarks>
        /// If the command was invoked via a text command and <paramref name="messageBuilder"/> does not have a reply message set, the message that invoked the command will be set as the reply.
        /// Additionally if <see cref="DiscordMessageBuilder.Mentions"/> is not set, it will be set to <see cref="Mentions.None"/>.
        /// </remarks>
        /// <param name="messageBuilder">The message to send.</param>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="InvocationType"/> is a <see cref="CommandInvocationType.SlashCommand"/> and a response has already been sent (excluding <see cref="PromptAsync(TextInputComponent[])"/>).</exception>
        public async Task ReplyAsync(IDiscordMessageBuilder messageBuilder)
        {
            if (InvocationType == CommandInvocationType.SlashCommand)
            {
                // Slash commands can only respond once, though we make an exception for modals since
                // PromptAsync will replace the orignal interaction.
                if (ResponseType.HasFlag(ContextResponseType.Created))
                {
                    throw new InvalidOperationException("Cannot respond to a slash command more than once.");
                }

                ResponseType |= ContextResponseType.Created;
                await Interaction!.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(messageBuilder));
            }
            else if (InvocationType == CommandInvocationType.TextCommand)
            {
                DiscordMessageBuilder builder = new(messageBuilder);

                // Reply to the message that invoked the command if no reply is set
                if (builder.ReplyId is null)
                {
                    builder.WithReply(Message!.Id);
                }

                // Don't ping anyone if no mentions are explicitly set
                if (builder.Mentions?.Count is null or 0)
                {
                    builder.WithAllowedMentions(Mentions.None);
                }

                Response = await Channel.SendMessageAsync(builder);
            }
        }

        /// <summary>
        /// Responds to the command by letting the user know that the command is still being processed.
        /// </summary>
        /// <remarks>
        /// If <see cref="InvocationType"/> is a <see cref="CommandInvocationType.TextCommand"/>, the bot will instead start "typing" in the channel for roughly 15 seconds.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="InvocationType"/> is a <see cref="CommandInvocationType.SlashCommand"/> and <see cref="ResponseType"/> is not null.</exception>
        [SuppressMessage("Roslyn", "IDE0046", Justification = "No nested conditional expressions.")]
        public Task DelayAsync()
        {
            if (InvocationType == CommandInvocationType.SlashCommand)
            {
                // Ensure that the command has not already responded
                if (ResponseType != ContextResponseType.None)
                {
                    throw new InvalidOperationException("Cannot delay a command that has already responded.");
                }

                ResponseType |= ContextResponseType.Delayed;
                return Interaction!.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            }
            else if (InvocationType == CommandInvocationType.TextCommand)
            {
                return Message!.Channel.TriggerTypingAsync();
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Edits the original response to the command.
        /// </summary>
        /// <param name="messageBuilder">The new message content.</param>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="InvocationType"/> is a <see cref="CommandInvocationType.SlashCommand"/> and a response has not been sent.</exception>
        public async Task EditAsync(IDiscordMessageBuilder messageBuilder)
        {
            if (InvocationType == CommandInvocationType.SlashCommand)
            {
                // Ensure that the command has responded with a deferred response
                ResponseType |= ContextResponseType.Updated;
                await Interaction!.EditOriginalResponseAsync(new DiscordWebhookBuilder(messageBuilder));
            }
            else if (InvocationType == CommandInvocationType.TextCommand)
            {
                // Ensure that the command has responded
                if (Response is null)
                {
                    throw new InvalidOperationException("You must first send a response before you can edit it.");
                }

                Response = await Response.ModifyAsync(new DiscordMessageBuilder(messageBuilder));
            }
        }

        /// <inheritdoc cref="EditAsync(IDiscordMessageBuilder)"/>
        /// <param name="content">The new message content.</param>
        public Task EditAsync(string content) => EditAsync(new DiscordMessageBuilder().WithContent(content));

        /// <inheritdoc cref="EditAsync(IDiscordMessageBuilder)"/>
        /// <param name="embed">The new message content.</param>
        public Task EditAsync(params DiscordEmbedBuilder[] embedBuilders) => EditAsync(new DiscordMessageBuilder().AddEmbeds(embedBuilders.Select(embedBuilder => embedBuilder.Build())));

        /// <summary>
        /// Deletes the original response to the command.
        /// </summary>
        public Task DeleteAsync() => InvocationType switch
        {
            CommandInvocationType.SlashCommand => Interaction!.DeleteOriginalResponseAsync(),
            CommandInvocationType.TextCommand => Response!.DeleteAsync(),
            CommandInvocationType.VirtualCommand => Task.CompletedTask,
            _ => throw new NotImplementedException("Unknown invocation type.")
        };

        /// <summary>
        /// Returns the original response to the command.
        /// </summary>
        /// <returns>The original response to the command. Null is returned if no response has been made.</returns>
        public Task<DiscordMessage?> GetOriginalResponse()
        {
            if (InvocationType == CommandInvocationType.SlashCommand)
            {
                // Ensure the slash command has responded before making the rest requests
                return ResponseType is ContextResponseType.None or ContextResponseType.Delayed
                    ? Task.FromResult<DiscordMessage?>(null)
                    : Interaction!.GetOriginalResponseAsync();
            }
            else if (InvocationType == CommandInvocationType.TextCommand)
            {
                // Return the cached response
                return Task.FromResult(Response);
            }
            else
            {
                return Task.FromResult<DiscordMessage?>(null);
            }
        }
    }
}
