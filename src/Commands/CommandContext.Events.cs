using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DSharpPlus.CommandAll.Commands
{
    /// <summary>
    /// The context of a command.
    /// </summary>
    public sealed partial class CommandContext
    {
        /// <summary>
        /// A static dictionary of all TCS's awaiting a response.
        /// </summary>
        private static readonly Dictionary<ulong, CommandContext> _contextTcs = new();

        /// <summary>
        /// A list of prompts that the user has to respond to.
        /// </summary>
        private Dictionary<string, string>? _prompts;

        /// <summary>
        /// The TCS that will be set when the user responds to the prompt.
        /// </summary>
        private TaskCompletionSource<List<string>>? _userInputTcs { get; set; }

        /// <summary>
        /// The cancellation token source that will be used to cancel the TCS if the user doesn't respond in time.
        /// </summary>
        private CancellationTokenSource? _userInputCts { get; set; }

        /// <summary>
        /// The timeout for the prompt.
        /// </summary>
        public readonly TimeSpan PromptTimeout;

        /// <summary>
        /// Prompts the user for further information, either through a modal or a series of messages.
        /// </summary>
        /// <param name="messages">The messages to display to the user to prompt for input. There must be 1 to 5 text input components.</param>
        /// <returns>A list of strings representing the input from the user, or null if the timeout had occured.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if there are fewer than 1 or more than 5 text input components.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="InvocationType"/> is a <see cref="CommandInvocationType.SlashCommand"/> and a response has already been sent.</exception>
        public async Task<IReadOnlyList<string>?> PromptAsync(params TextInputComponent[] messages)
        {
            // Ensure there are 1 to 5 text input components
            if (messages.Length is < 1 or > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(messages), "There must be 1 to 5 text input components.");
            }

            // Initialize a TaskCompletionSource to store the user's input
            _userInputTcs = new TaskCompletionSource<List<string>>();

            // Check the InvocationType of the command
            if (InvocationType == CommandInvocationType.SlashCommand)
            {
                // Check if there was a response, or if the response WASN'T a deferred response
                if (ResponseType.HasFlag(ContextResponseType.Created))
                {
                    throw new InvalidOperationException("Cannot respond to a slash command more than once.");
                }
                else
                {
                    // Log a debug message and create a modal response
                    DiscordInteractionResponseBuilder responseBuilder = new()
                    {
                        Title = "Just a bit more information...",
                        CustomId = Guid.NewGuid().ToString()
                    };

                    // Add the text input components to the response
                    // We do this one by one because the AddComponents method will group them all into action rows
                    for (int i = 0; i < messages.Length; i++)
                    {
                        responseBuilder.AddComponents(messages[i]);
                    }

                    // Add a reference to this object to the context dictionary using the Interaction's Id as the key
                    _contextTcs.Add(Interaction!.Id, this);

                    // Send the modal response
                    await Interaction.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);
                }
            }
            else
            {
                // Reset the CancellationToken
                ResetCancellationToken();

                // Save the prompts in a dictionary
                _prompts = messages.ToDictionary(message => message.Label, message => string.Empty);

                // Send the first prompt message to the user
                await ReplyAsync(messages[0].Label);

                // Add a reference to this object to the context dictionary using the Response's Id as the key
                _contextTcs.Add(Response!.Id, this);
            }

            // Set the interaction response type to Modal
            ResponseType |= ContextResponseType.Modal;

            // Return the result of the Task from the TaskCompletionSource, or null if the timeout had occured
            try
            {
                return (await _userInputTcs.Task).AsReadOnly();
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        /// <summary>
        /// Handles the modal submission event for <see cref="PromptAsync(TextInputComponent[])"/>.
        /// </summary>
        /// <param name="client">(Unused) The Discord client instance.</param>
        /// <param name="eventArgs">The event arguments for the modal submission event.</param>
        /// <returns>A Task representing the asynchronous operation, though the method is completely synchronous.</returns>
        [SuppressMessage("Roslyn", "IDE0060", Justification = "The Discord event handler requires a DiscordClient parameter.")]
        internal static Task HandleModalAsync(DiscordClient client, ModalSubmitEventArgs eventArgs)
        {
            // Attempt to parse the CustomId of the Interaction as a ulong and get the corresponding CommandContext from the context dictionary
            if (!ulong.TryParse(eventArgs.Interaction.Data.CustomId, out ulong id) || !_contextTcs.TryGetValue(id, out CommandContext? context))
            {
                // If the CustomId could not be parsed or the context could not be found
                // return a completed Task as this modal is not related to this context
                return Task.CompletedTask;
            }


            // Set the Interaction property of the CommandContext and set the result of the TaskCompletionSource
            context.Interaction = eventArgs.Interaction;
            context._userInputTcs!.SetResult(eventArgs.Values.Values.Select(value => value).ToList());

            // Return a completed Task
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the message created event for <see cref="PromptAsync(TextInputComponent[])"/>, specifically when the intended user replies to our message.
        /// </summary>
        /// <param name="client">(Unused) The Discord client instance.</param>
        /// <param name="eventArgs">The event arguments for the message event.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [SuppressMessage("Roslyn", "IDE0060", Justification = "The Discord event handler requires a DiscordClient parameter.")]
        internal static async Task HandleMessageAsync(DiscordClient client, MessageCreateEventArgs eventArgs)
        {
            // Ensure the user replied to our message and the user belongs to the context
            if (eventArgs.Message.ReferencedMessage is null
                || !_contextTcs.TryGetValue(eventArgs.Message.ReferencedMessage.Id, out CommandContext? context)
                || eventArgs.Author.Id != context.User.Id)
            {
                return;
            }

            // Iterate through the prompts in the context
            int i = 0;
            foreach ((string prompt, string response) in context._prompts!)
            {
                // If the prompt matches the referenced message, save the user's response and send the next prompt if there is one
                // Additionally ensure the response is empty for duplicate prompts
                if (prompt == eventArgs.Message.ReferencedMessage.Content && string.IsNullOrEmpty(response))
                {
                    context._prompts[prompt] = eventArgs.Message.Content;
                    if (i++ < context._prompts.Count)
                    {
                        // Reply with the next prompt
                        await context.ReplyAsync(context._prompts.ElementAt(i).Key);
                        context.ResetCancellationToken();
                    }
                    else
                    {
                        // If there are no more prompts, set the result of the TaskCompletionSource
                        context._userInputTcs!.SetResult(context._prompts.Values.ToList());
                    }
                }
            }
        }

        /// <summary>
        /// Resets the <see cref="_userInputCts"/> CancellationTokenSource and registers a cancellation callback to the CancellationToken.
        /// </summary>
        private void ResetCancellationToken()
        {
            // If the CancellationTokenSource is null, create a new one
            _userInputCts ??= new CancellationTokenSource();

            // Reset the CancellationTokenSource
            _userInputCts.TryReset();

            // Add a timeout to the CancellationTokenSource
            _userInputCts.CancelAfter(PromptTimeout);

            // Cancel the TaskCompletionSource if the CancellationToken is cancelled
            _userInputCts.Token.Register(_userInputTcs!.SetCanceled);
        }
    }
}
