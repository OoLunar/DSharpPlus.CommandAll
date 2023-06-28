using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Contexts
{
    public abstract class CommandContext
    {
        // Extension Properties
        public required CommandAllExtension Extension { get; init; }
        public DiscordClient Client => Extension.Client;

        // Command Properties
        public Command Command => CurrentOverload.Command;
        public required CommandOverload CurrentOverload { get; init; }
        public required IReadOnlyDictionary<CommandParameter, object?> Arguments { get; init; }

        // Context Properties
        public required DiscordChannel Channel { get; init; }
        public required DiscordUser User { get; init; }
        public DiscordGuild? Guild { get; init; }
        public DiscordMember? Member => User as DiscordMember;

        // Methods
        public abstract Task DelayAsync();
        public abstract Task<IReadOnlyList<string?>> PromptAsync(CancellationToken cancellationToken, params TextInputComponent[] questions);

        // Response Methods
        public abstract Task CreateResponseAsync(IDiscordMessageBuilder response);
        public abstract Task EditResponseAsync(IDiscordMessageBuilder response);
        public abstract Task DeleteResponseAsync();
        public abstract Task<DiscordMessage> GetResponseAsync();

        // Follow Up Methods
        public abstract Task CreateFollowUpAsync(IDiscordMessageBuilder response);
        public abstract Task EditFollowUpAsync(IDiscordMessageBuilder response);
        public abstract Task DeleteFollowUpAsync();
        public abstract Task<DiscordMessage> GetFollowUpAsync();

        // Helper Methods
        public virtual Task CreateResponseAsync(string messageContent) => CreateResponseAsync(new DiscordMessageBuilder().WithContent(messageContent));
        public virtual Task CreateResponseAsync(params DiscordEmbedBuilder[] messageEmbeds) => CreateResponseAsync(new DiscordMessageBuilder().AddEmbeds(messageEmbeds.Select(embedBuilder => embedBuilder.Build())));
        public virtual Task EditResponseAsync(string messageContent) => EditResponseAsync(new DiscordMessageBuilder().WithContent(messageContent));
        public virtual Task EditResponseAsync(params DiscordEmbedBuilder[] messageEmbeds) => EditResponseAsync(new DiscordMessageBuilder().AddEmbeds(messageEmbeds.Select(embedBuilder => embedBuilder.Build())));

        public virtual Task CreateFollowUpAsync(string messageContent) => CreateFollowUpAsync(new DiscordMessageBuilder().WithContent(messageContent));
        public virtual Task CreateFollowUpAsync(params DiscordEmbedBuilder[] messageEmbeds) => CreateFollowUpAsync(new DiscordMessageBuilder().AddEmbeds(messageEmbeds.Select(embedBuilder => embedBuilder.Build())));
        public virtual Task EditFollowUpAsync(string messageContent) => EditFollowUpAsync(new DiscordMessageBuilder().WithContent(messageContent));
        public virtual Task EditFollowUpAsync(params DiscordEmbedBuilder[] messageEmbeds) => EditFollowUpAsync(new DiscordMessageBuilder().AddEmbeds(messageEmbeds.Select(embedBuilder => embedBuilder.Build())));

        public virtual Task<IReadOnlyList<string?>> PromptAsync(params TextInputComponent[] questions) => PromptAsync(CancellationToken.None, questions);
        public virtual Task<IReadOnlyList<string?>> PromptAsync(TimeSpan timeout, params TextInputComponent[] questions) => PromptAsync(new CancellationTokenSource(timeout).Token, questions);
    }
}
