using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using System.Linq;

namespace DSharpPlus.CommandAll.Commands.Contexts
{
    public class MessageContext : CommandContext
    {
        public required DiscordMessage Message { get; init; }
        public DiscordMessage? Response { get; private set; }
        public DiscordMessage? FollowUp { get; private set; }

        /// <inheritdoc />
        public override async Task CreateResponseAsync(IDiscordMessageBuilder response)
        {
            DiscordMessageBuilder builder = new(response);

            // Reply to the message that invoked the command if no reply is set
            if (builder.ReplyId is null)
            {
                builder.WithReply(Message.Id);
            }

            // Don't ping anyone if no mentions are explicitly set
            if (builder.Mentions?.Count is null or 0)
            {
                builder.WithAllowedMentions(Mentions.None);
            }

            Response = await Channel.SendMessageAsync(builder);
        }

        /// <inheritdoc />
        public override async Task EditResponseAsync(IDiscordMessageBuilder response)
        {
            if (Response is null)
            {
                throw new InvalidOperationException("Cannot edit a response that has not been sent yet.");
            }

            Response = await Response.ModifyAsync(new DiscordMessageBuilder(response));
        }

        /// <inheritdoc />
        public override Task DeleteResponseAsync() => DeleteResponseAsync(null);
        public async Task DeleteResponseAsync(string? reason)
        {
            if (Response is null)
            {
                throw new InvalidOperationException("Cannot delete a response that has not been sent yet.");
            }

            await Response.DeleteAsync(reason);
            Response = null;
        }

        /// <inheritdoc />
        public override Task<DiscordMessage> GetResponseAsync() => Response is null
            ? throw new InvalidOperationException("Cannot get a response that has not been sent yet.")
            : Task.FromResult(Response);

        /// <inheritdoc />
        public override async Task CreateFollowUpAsync(IDiscordMessageBuilder response)
        {
            if (Response is null)
            {
                throw new InvalidOperationException("Cannot follow up to a response that has not been sent yet.");
            }
            else
            {
                DiscordMessageBuilder builder = new(response);

                // Reply to the message that invoked the command if no reply is set
                if (builder.ReplyId is null)
                {
                    builder.WithReply(Message.Id);
                }

                // Don't ping anyone if no mentions are explicitly set
                if (builder.Mentions?.Count is null or 0)
                {
                    builder.WithAllowedMentions(Mentions.None);
                }

                FollowUp = await Channel.SendMessageAsync(builder);
            }
        }

        /// <inheritdoc />
        public override async Task EditFollowUpAsync(IDiscordMessageBuilder response)
        {
            if (FollowUp is null)
            {
                throw new InvalidOperationException("Cannot edit a follow up that has not been sent yet.");
            }

            FollowUp = await FollowUp.ModifyAsync(new DiscordMessageBuilder(response));
        }

        public override Task DeleteFollowUpAsync() => DeleteFollowUpAsync(null);
        public async Task DeleteFollowUpAsync(string? reason)
        {
            if (FollowUp is null)
            {
                throw new InvalidOperationException("Cannot delete a follow up that has not been sent yet.");
            }

            await FollowUp.DeleteAsync(reason);
            FollowUp = null;
        }

        /// <inheritdoc />
        public override Task<DiscordMessage> GetFollowUpAsync() => FollowUp is null
            ? throw new InvalidOperationException("Cannot get a follow up that has not been sent yet.")
            : Task.FromResult(FollowUp);

        /// <inheritdoc />
        public override Task DelayAsync() => DelayAsync(TimeSpan.FromMinutes(15));
        public Task DelayAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationTokenSource.CancelAfter(timeout);
            _ = Task.Run(async () =>
            {
                PeriodicTimer timer = new(TimeSpan.FromSeconds(15));
                do
                {
                    await Channel.TriggerTypingAsync();
                } while (await timer.WaitForNextTickAsync() && Response is null);
            }, cancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task<IReadOnlyList<string?>> PromptAsync(CancellationToken cancellationToken, params TextInputComponent[] questions) => PromptAsync(cancellationToken, questions.Select(x => x.Placeholder).ToArray());
        public Task<IReadOnlyList<string?>> PromptAsync(params string[] questions) => PromptAsync(CancellationToken.None, questions);
        public Task<IReadOnlyList<string?>> PromptAsync(TimeSpan timeout, params string[] questions) => PromptAsync(new CancellationTokenSource(timeout).Token, questions);
        public Task<IReadOnlyList<string?>> PromptAsync(CancellationToken cancellationToken, params string[] questions) => throw new NotImplementedException();
    }
}
