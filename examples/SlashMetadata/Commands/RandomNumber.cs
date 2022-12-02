using System;
using System.ComponentModel;
using System.Threading.Tasks;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples.SlashMetadata.Commands
{
    public sealed class RandomNumberCommand : BaseCommand
    {
        [Command("random_number"), Description("Generates a random number between 1 and 20.")]
        public static Task RandomNumberAsync(CommandContext context, [Description("The minimum number."), MinMax(MinValue = 1)] int min, [Description("The maximum number"), MinMax(MaxValue = 20)] int max) => context.ReplyAsync($"Your random number is {Random.Shared.Next(min, max)}");
    }
}
