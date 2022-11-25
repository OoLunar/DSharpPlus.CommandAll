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

        [Command("avatar"), Description("Echoes the given user's avatar.")]
        public static Task ExecuteAsync(CommandContext context, [Description("The user to echo.")] DiscordUser user) => context.ReplyAsync(new DiscordMessageBuilder().WithContent(user.AvatarUrl));
    }
}
