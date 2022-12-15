using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace OoLunar.DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// The context of a command.
    /// </summary>
    public sealed partial class CommandContext
    {
        public readonly TimeSpan PromptTimeout;
        private static readonly Dictionary<object, CommandContext> _currentTcs = new();
        private Dictionary<string, string>? _prompts;
        private TaskCompletionSource<List<string>>? _userInputTcs { get; set; }
        private CancellationTokenSource? _userInputCts { get; set; }

        public async Task<IReadOnlyList<string>?> PromptAsync(params TextInputComponent[] messages)
        {
            if (messages.Length is < 1 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(messages), "Must have between 1 and 5 messages.");
            }

            _userInputTcs = new TaskCompletionSource<List<string>>();
            if (IsSlashCommand)
            {
                if (LastInteractionResponseType is not null)
                {
                    throw new InvalidOperationException("Cannot respond to a slash command more than once.");
                }
                else
                {
                    _logger.LogDebug("Prompting slash command {Id}.", Interaction!.Id);
                    LastInteractionResponseType = InteractionResponseType.Modal;
                    DiscordInteractionResponseBuilder responseBuilder = new()
                    {
                        Title = "Just a bit more information...",
                        CustomId = Guid.NewGuid().ToString()
                    };

                    // Can't add them all at once because it'll be shoved into an action row and break.
                    for (int i = 0; i < messages.Length; i++)
                    {
                        responseBuilder.AddComponents(messages[i]);
                    }

                    _currentTcs.Add(Interaction!.Id, this);
                    await Interaction.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);
                }
            }
            else
            {
                ResetCancellationToken();
                _prompts = messages.ToDictionary(message => message.Label, message => string.Empty);

                // Sets Response in logic
                await ReplyAsync(messages[0].Label);
                _currentTcs.Add(Response!.Id, this);
            }

            try
            {
                return (await _userInputTcs.Task).ToList().AsReadOnly();
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        internal static Task HandleModalAsync(DiscordClient client, ModalSubmitEventArgs eventArgs)
        {
            if (!_currentTcs.TryGetValue(eventArgs.Interaction.Data.CustomId, out CommandContext? context))
            {
                return Task.CompletedTask;
            }

            context.Interaction = eventArgs.Interaction;
            context._userInputTcs!.SetResult(eventArgs.Values.Values.Select(value => value).ToList());
            return Task.CompletedTask;
        }

        internal static async Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            if (eventArgs.Message.ReferencedMessage is null || !_currentTcs.TryGetValue(eventArgs.Message.ReferencedMessage.Id, out CommandContext? context) || eventArgs.Author.Id != context.User.Id)
            {
                return;
            }

            context._logger.LogDebug("Handling message {Id}.", eventArgs.Message.Id);
            for (int i = 0; i < context._prompts!.Count; i++)
            {
                if (context._prompts.ElementAt(i).Key == eventArgs.Message.ReferencedMessage.Content)
                {
                    context._prompts[context._prompts.ElementAt(i).Key] = eventArgs.Message.Content;
                    if (i + 1 < context._prompts.Count)
                    {
                        await context.ReplyAsync(context._prompts.ElementAt(i + 1).Key);
                        context.ResetCancellationToken();
                    }
                    else
                    {
                        context._userInputTcs!.SetResult(context._prompts.Values.ToList());
                    }
                }
            }
        }

        private void ResetCancellationToken()
        {
            _userInputCts ??= new CancellationTokenSource();
            _userInputCts.TryReset();
            _userInputCts.CancelAfter(PromptTimeout);
            _userInputCts.Token.Register(_userInputTcs!.SetCanceled);
        }
    }
}
