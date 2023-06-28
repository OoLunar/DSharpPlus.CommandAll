using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Contexts
{
    public class SlashInteractionContext : CommandContext
    {
        public required DiscordInteraction Interaction { get; init; }
        public InteractionStatus Status { get; private set; }
        private DiscordMessage? FollowUpMessage { get; set; }

        /// <inheritdoc />
        public override async Task CreateResponseAsync(IDiscordMessageBuilder response)
        {
            if (Status.HasFlag(InteractionStatus.Responded))
            {
                throw new InvalidOperationException("Cannot respond to an interaction that has already been responded to.");
            }

            await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(response));
            Status |= InteractionStatus.Responded;
        }

        /// <inheritdoc />
        public override Task EditResponseAsync(IDiscordMessageBuilder response) => EditResponseAsync(response);
        public async Task<DiscordMessage> EditResponseAsync(IDiscordMessageBuilder response, IEnumerable<DiscordAttachment>? attachments = null) => !Status.HasFlag(InteractionStatus.Responded)
            ? throw new InvalidOperationException("Cannot edit a response that has not been sent yet.")
            : await Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder(response), attachments);

        /// <inheritdoc />
        public override async Task DeleteResponseAsync()
        {
            if (!Status.HasFlag(InteractionStatus.Responded))
            {
                throw new InvalidOperationException("Cannot delete a response that has not been sent yet.");
            }

            await Interaction.DeleteOriginalResponseAsync();
        }

        /// <inheritdoc />
        public override async Task<DiscordMessage> GetResponseAsync() => !Status.HasFlag(InteractionStatus.Responded)
            ? throw new InvalidOperationException("Cannot get a response that has not been sent yet.")
            : await Interaction.GetOriginalResponseAsync();

        /// <inheritdoc />
        public override Task CreateFollowUpAsync(IDiscordMessageBuilder response) => CreateFollowUpAsync(response);
        public async Task<DiscordMessage> CreateFollowUpAsync(DiscordFollowupMessageBuilder response)
        {
            if (!Status.HasFlag(InteractionStatus.Responded))
            {
                throw new InvalidOperationException("Cannot create a follow-up message to a response that has not been sent yet.");
            }
            else if (Status.HasFlag(InteractionStatus.FollowedUp))
            {
                throw new InvalidOperationException("Cannot create a follow-up message to a response twice.");
            }

            FollowUpMessage = await Interaction.CreateFollowupMessageAsync(response);
            Status |= InteractionStatus.FollowedUp;
            return FollowUpMessage;
        }

        /// <inheritdoc />
        public override Task EditFollowUpAsync(IDiscordMessageBuilder response) => EditFollowUpAsync(response);
        public async Task EditFollowUpAsync(DiscordWebhookBuilder response, IEnumerable<DiscordAttachment>? attachments = null)
        {
            if (!Status.HasFlag(InteractionStatus.FollowedUp) || FollowUpMessage is null)
            {
                throw new InvalidOperationException("Cannot edit a follow-up message to a response that has not been sent yet.");
            }

            FollowUpMessage = await Interaction.EditFollowupMessageAsync(FollowUpMessage.Id, response, attachments);
        }

        public override async Task DeleteFollowUpAsync()
        {
            if (!Status.HasFlag(InteractionStatus.FollowedUp) || FollowUpMessage is null)
            {
                throw new InvalidOperationException("Cannot delete a follow-up message to a response that has not been sent yet.");
            }

            await Interaction.DeleteFollowupMessageAsync(FollowUpMessage.Id);
            FollowUpMessage = null;
        }

        /// <inheritdoc />
        public override Task<DiscordMessage> GetFollowUpAsync() => GetFollowUpAsync(false);
        public async Task<DiscordMessage> GetFollowUpAsync(bool cacheBust)
        {
            if (!Status.HasFlag(InteractionStatus.Responded) || FollowUpMessage is null)
            {
                throw new InvalidOperationException("Cannot get a follow-up message to a response that has not been sent yet.");
            }
            else if (!cacheBust)
            {
                return FollowUpMessage;
            }

            FollowUpMessage = await Interaction.GetFollowupMessageAsync(FollowUpMessage.Id);
            return FollowUpMessage;
        }

        /// <inheritdoc />
        public override Task DelayAsync() => DelayAsync(false);
        public async Task DelayAsync(bool ephemeral)
        {
            if (Status.HasFlag(InteractionStatus.Acknowledged))
            {
                throw new InvalidOperationException("Cannot acknowledge an interaction that has already been acknowledged.");
            }

            await Interaction.DeferAsync(ephemeral);
            Status |= InteractionStatus.Acknowledged;
        }

        /// <inheritdoc />
        public override Task<IReadOnlyList<string?>> PromptAsync(CancellationToken cancellationToken, params TextInputComponent[] questions) => throw new NotImplementedException();
    }

    [Flags]
    public enum InteractionStatus
    {
        None = 0,
        Acknowledged = 1 << 0,
        Responded = 1 << 1,
        FollowedUp = 1 << 2
    }
}
