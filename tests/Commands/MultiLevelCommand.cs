using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Tests.Commands
{
    [Command("command"), Description("A valid command.")]
    public sealed class MultiLevelCommand : BaseCommand
    {
        [Command("subcommand"), Description("A valid subcommand.")]
        public static Task ExecuteAsync(CommandContext context) => Task.CompletedTask;

        [Command("group"), Description("An invalid group command.")]
        public sealed class GroupCommand : BaseCommand
        {
            [Command("subcommand"), Description("An invalid sub group command.")]
            public static Task ExecuteAsync(CommandContext context) => Task.CompletedTask;
        }
    }
}
