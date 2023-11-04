using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.TextCommands.Attributes;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Tests.Cases
{
    public class TestTopLevelCommands
    {
        [Command("oops")]
        public static Task OopsAsync() => Task.CompletedTask;

        [Command("ping")]
        public static Task PingAsync(CommandContext context) => Task.CompletedTask;

        [Command("echo")]
        public static Task EchoAsync(CommandContext context, [RemainingText] string message) => Task.CompletedTask;

        [Command("user_info")]
        public static Task UserInfoAsync(CommandContext context, DiscordUser? user = null) => Task.CompletedTask;
    }
}
