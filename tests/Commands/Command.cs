using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Tests.Commands
{
    [Command("command"), Description("A command for testing.")]
    public sealed class Command : BaseCommand
    {
        [Command("group"), Description("Group description.")]
        public sealed class GroupCommand
        {
            [Command("subcommand"), Description("Subcommand description.")]
            public static Task ExecuteAsync(CommandContext context) => context.ReplyAsync(new DiscordMessageBuilder().WithContent(context.CurrentCommand.FullName));
        }
    }
}
