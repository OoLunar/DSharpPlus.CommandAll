using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class HelloWorldCommand
    {
        [Command("hello_world")]
        public static async Task ExecuteAsync(CommandContext context, DiscordAttachment file) => await context.RespondAsync($"Echo: {file.ProxyUrl}");
    }
}
