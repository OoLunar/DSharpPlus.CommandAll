using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Tests.Commands
{
    public sealed class PingCommand : BaseCommand
    {
        [Command("ping"), Description("Sends a ping to the bot.")]
        public static Task PingAsync(CommandContext context) => context.ReplyAsync(new DiscordMessageBuilder().WithContent("Pong!"));
    }
}
