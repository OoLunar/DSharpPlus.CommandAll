using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class HelloWorldCommand
    {
        private static readonly HttpClient _httpClient = new();

        [Command("hello_world")]
        public static async Task ExecuteAsync(CommandContext context, DiscordAttachment file) => await context.RespondAsync(new DiscordMessageBuilder().WithContent(file.FileName).AddFile(file.FileName, await _httpClient.GetStreamAsync(file.Url)));
    }
}
