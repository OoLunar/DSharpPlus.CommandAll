using System;
using System.ComponentModel;
using System.Threading.Tasks;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples.ParamsCommand.Commands
{
    public sealed class RandomCommand : BaseCommand
    {
        [Command("random"), Description("Chooses a random option from the given options.")]
        public static Task RandomAsync(CommandContext context, [Description("Multiple choices to be singled out by random.")] params string[] randomChoices) => context.ReplyAsync(new() { Content = $"You chose {randomChoices[Random.Shared.Next(randomChoices.Length)]}!" });
    }
}
