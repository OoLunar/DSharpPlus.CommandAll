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
        private string? _expectedId;
        private Dictionary<string, string>? _prompts;
        private TaskCompletionSource<IEnumerable<string>>? _userInputTcs { get; set; }
        private CancellationTokenSource? _userInputCts { get; set; }

        public async Task<IReadOnlyList<string>?> PromptAsync(params TextInputComponent[] messages)
        {
            if (messages.Length is < 1 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(messages), "Must have between 1 and 5 messages.");
            }

            _userInputTcs = new TaskCompletionSource<IEnumerable<string>>();
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
                        CustomId = _expectedId = Guid.NewGuid().ToString()
                    };

                    // Can't add them all at once because it'll be shoved into an action row and break.
                    for (int i = 0; i < messages.Length; i++)
                    {
                        responseBuilder.AddComponents(messages[i]);
                    }
                    Client.ModalSubmitted += HandleModalAsync;
                    await Interaction.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);
                }
            }
            else
            {
                ResetCancellationToken();
                _prompts = messages.ToDictionary(message => message.Label, message => string.Empty);
                Client.MessageCreated += HandleMessageAsync;

                // Sets Response in logic
                await ReplyAsync(messages[0].Label);
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

        private Task HandleModalAsync(DiscordClient client, ModalSubmitEventArgs eventArgs)
        {
            if (eventArgs.Interaction.Data.CustomId != _expectedId)
            {
                return Task.CompletedTask;
            }

            Client.ModalSubmitted -= HandleModalAsync;
            Interaction = eventArgs.Interaction;
            _userInputTcs!.SetResult(eventArgs.Values.Values.Select(value => value));
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            if (eventArgs.Author.Id != User.Id || eventArgs.Message.ReferencedMessage is null || eventArgs.Message.ReferencedMessage.Id != Response!.Id)
            {
                return;
            }

            _logger.LogDebug("Handling message {Id}.", eventArgs.Message.Id);
            for (int i = 0; i < _prompts!.Count; i++)
            {
                if (_prompts.ElementAt(i).Key == eventArgs.Message.ReferencedMessage.Content)
                {
                    _prompts[_prompts.ElementAt(i).Key] = eventArgs.Message.Content;
                    if (i + 1 < _prompts.Count)
                    {
                        await ReplyAsync(_prompts.ElementAt(i + 1).Key);
                        ResetCancellationToken();
                    }
                    else
                    {
                        Client.MessageCreated -= HandleMessageAsync;
                        _userInputTcs!.SetResult(_prompts.Values);
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
