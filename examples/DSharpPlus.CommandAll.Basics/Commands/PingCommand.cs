using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class PingCommand
    {
        [Command("ping")]
        public static async Task ExecuteAsync(CommandContext context) => await context.RespondAsync("Pong!");
    }
}
