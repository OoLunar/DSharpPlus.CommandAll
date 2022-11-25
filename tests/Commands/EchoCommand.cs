using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Tests.Commands
{
    public sealed class EchoCommand : BaseCommand
    {
        [Command("echo"), Description("Echoes the given message.")]
        public static Task ExecuteAsync(CommandContext context, [Description("The message to echo.")] string message) => context.ReplyAsync(new DiscordMessageBuilder().WithContent(message));

        [Command("echo")]
        public static Task ExecuteAsync(CommandContext context, [Description("The message to echo.")] string message, [Description("The number of times to echo the message.")] int count)
        {
            List<string> messages = new();
            for (int i = 0; i < count; i++)
            {
                messages.Add(message);
            }
            return context.ReplyAsync(new DiscordMessageBuilder().WithContent(string.Join("\n", messages)));
        }
    }
}
