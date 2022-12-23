using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Tests.Commands
{
    public sealed class PingCommand : BaseCommand
    {
        [Command("ping"), Description("Sends a ping to the bot.")]
        public static Task PingAsync(CommandContext context) => context.ReplyAsync(new DiscordMessageBuilder().WithContent("Pong!"));
    }
}
