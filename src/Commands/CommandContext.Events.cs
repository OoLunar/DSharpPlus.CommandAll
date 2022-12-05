using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private string? _expectedId;
        private Dictionary<string, string> _prompts;
        private TaskCompletionSource<IEnumerable<string>> _userInputTcs { get; set; } = new();

        public async Task<IEnumerable<string>> PromptAsync(params string[] messages)
        {
            if (messages.Length is < 1 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(messages), "Must have between 1 and 5 messages.");
            }

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

                    for (int i = 0; i < messages.Length; i++)
                    {
                        string message = messages[i];
                        responseBuilder.AddComponents(new TextInputComponent(message, i.ToString(CultureInfo.InvariantCulture), style: TextInputStyle.Paragraph, min_length: 1));
                    }

                    Client.ModalSubmitted += HandleModalAsync;
                    await Interaction.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);
                }
            }
            else
            {
                // Sets Response in logic
                _prompts = messages.ToDictionary(message => message, message => string.Empty);
                Client.MessageCreated += HandleMessageAsync;
                await ReplyAsync(messages[0]);
            }

            return await _userInputTcs.Task;
        }

        private Task HandleModalAsync(DiscordClient client, ModalSubmitEventArgs eventArgs)
        {
            if (eventArgs.Interaction.Data.CustomId != _expectedId)
            {
                return Task.CompletedTask;
            }

            Client.ModalSubmitted -= HandleModalAsync;
            Interaction = eventArgs.Interaction;
            _userInputTcs.SetResult(eventArgs.Values.OrderBy(kvp => kvp.Key).Select(value => value.Value));
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            if (eventArgs.Author.Id != User.Id || eventArgs.Message.ReferencedMessage is null || eventArgs.Message.ReferencedMessage.Id != Response!.Id)
            {
                return;
            }

            _logger.LogDebug("Handling message {Id}.", eventArgs.Message.Id);
            for (int i = 0; i < _prompts.Count; i++)
            {
                if (_prompts.ElementAt(i).Key == eventArgs.Message.ReferencedMessage.Content)
                {
                    _prompts[_prompts.ElementAt(i).Key] = eventArgs.Message.Content;
                    if (i + 1 < _prompts.Count)
                    {
                        await ReplyAsync(_prompts.ElementAt(i + 1).Key);
                    }
                    else
                    {
                        Client.MessageCreated -= HandleMessageAsync;
                        _userInputTcs.SetResult(_prompts.Values);
                    }
                }
            }
        }
    }
}
