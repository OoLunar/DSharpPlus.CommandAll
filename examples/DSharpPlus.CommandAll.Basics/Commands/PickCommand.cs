using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.SlashCommands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class PickCommand
    {
        [Command("pick")]
        public static async Task ExecuteAsync(CommandContext context, [SlashChoiceProvider<PickProvider>] string favorite_animal) => await context.RespondAsync($"Was your favorite animal a... {favorite_animal}?");
    }

    public sealed class PickProvider : IChoiceProvider
    {
        public Task<Dictionary<string, object>> ProvideAsync(CommandParameter parameter) => Task.FromResult(new Dictionary<string, object>
        {
            { "Dog", "dog" },
            { "Cat", "cat" },
            { "Bird", "bird" }
        });
    }
}
