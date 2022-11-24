using System;
using System.ComponentModel;
using System.Threading.Tasks;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples.HelloWorld
{
    public sealed class PingCommand : BaseCommand
    {
        [Command("ping")]
        [Description("Replies with pong and the delay between the gateway.")]
        public static async Task PingAsync(CommandContext context)
        {
            await context.DelayAsync();
            await Task.Delay(TimeSpan.FromSeconds(5));
            await context.ReplyAsync(new()
            {
                Content = $"Pong! Delay is {context.Client.Ping}ms"
            });
        }
    }
}
