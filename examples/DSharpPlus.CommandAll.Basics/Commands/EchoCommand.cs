using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class EchoCommand
    {
        [Command("echo")]
        public static async Task ExecuteAsync(CommandContext context, params string[] text) => await context.RespondAsync(string.Join(' ', text));
    }
}
