using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.SlashCommands;
using DSharpPlus.CommandAll.Processors.SlashCommands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class TagCommand
    {
        [Command("tag")]
        public static async Task ExecuteAsync(CommandContext context, [SlashAutoCompleteProvider<TagAutoCompleteProvider>] string tag_name) => await context.RespondAsync($"There is no tag system. Tag {tag_name} does not exist. It has never existed.");
    }

    public sealed class TagAutoCompleteProvider : IAutoCompleteProvider
    {
        private static readonly Dictionary<string, string> _prefilled = new()
        {
            ["Predict"] = "predict",
            ["Prepare"] = "prepare",
            ["Prefix"] = "prefix",
            ["Presume"] = "presume",
            ["Precede"] = "precede",
            ["Tricycle"] = "tricycle",
            ["Triangle"] = "triangle",
            ["Trickster"] = "trickster",
            ["Trimester"] = "trimester",
            ["Trillion"] = "trillion",
            ["Strong"] = "strong",
            ["Stress"] = "stress",
            ["Strike"] = "strike",
            ["Street"] = "street",
            ["Stretch"] = "stretch"
        };

        public Task<Dictionary<string, object>> AutoCompleteAsync(AutoCompleteContext context)
        {
            string? userInput = context.UserInput.ToString();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return Task.FromResult(Unsafe.As<Dictionary<string, object>>(_prefilled));
            }

            Dictionary<string, object> results = [];
            foreach (KeyValuePair<string, string> pair in _prefilled)
            {
                if (pair.Key.StartsWith(context.UserInput.ToString() ?? string.Empty))
                {
                    results.Add(pair.Key, pair.Value);
                }
            }

            return Task.FromResult(results);
        }
    }
}
