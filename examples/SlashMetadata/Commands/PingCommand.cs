using System.ComponentModel;
using System.Threading.Tasks;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples.SlashMetadata.Commands
{
    public sealed class PingCommand : BaseCommand
    {
        [Command("ping"), Description("Pings the bot, returning the latency between the Discord gateway and the bot.")]
        public static Task PingAsync(CommandContext context) => context.ReplyAsync(new() { Content = $"Pong! Ping is {context.Extension.Client.Ping}ms." });
    }
}
