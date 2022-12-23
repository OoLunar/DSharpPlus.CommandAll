using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Examples.SlashMetadata.Commands
{
    public sealed class PingCommand : BaseCommand
    {
        [Command("ping"), Description("Pings the bot, returning the latency between the Discord gateway and the bot.")]
        public static Task PingAsync(CommandContext context) => context.ReplyAsync($"Pong! Ping is {context.Extension.Client.Ping}ms.");
    }
}
