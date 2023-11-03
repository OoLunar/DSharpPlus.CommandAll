using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.TextCommands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class EchoCommand
    {
        [Command("echo")]
        public static async Task ExecuteAsync(CommandContext context, string text1, [RemainingText] string? text2 = null) => await context.RespondAsync($"{text1} {text2}".Trim());
    }
}
