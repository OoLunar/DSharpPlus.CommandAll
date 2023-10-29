using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class EchoCommand
    {
        [Command("echo")]
        public static async Task ExecuteAsync(CommandContext context, string text1, string text2, string text3, string text4, string text5) => await context.RespondAsync($"{text1} {text2} {text3} {text4} {text5}");
    }
}
