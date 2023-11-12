using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.TextCommands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class EchoCommand
    {
        [Command("echo")]
        public static async Task ExecuteAsync(CommandContext context, [RemainingText] string text) => await context.RespondAsync(text);
    }
}
