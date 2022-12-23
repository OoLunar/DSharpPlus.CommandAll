using System;
using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Examples.ParamsCommand.Commands
{
    public sealed class RandomCommand : BaseCommand
    {
        [Command("random"), Description("Chooses a random option from the given options.")]
        public static Task RandomAsync(CommandContext context, [Description("Multiple choices to be singled out by random.")] params string[] randomChoices) => context.ReplyAsync($"You chose {randomChoices[Random.Shared.Next(randomChoices.Length)]}!");
    }
}
