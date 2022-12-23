using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Tests.Commands
{
    [Command("command"), Description("A command for testing.")]
    public sealed class MultiLevelCommand : BaseCommand
    {
        [Command("subcommand"), Description("A subcommand for testing.")]
        public static Task ExecuteAsync(CommandContext context) => Task.CompletedTask;

        [Command("group"), Description("Group description.")]
        public sealed class GroupCommand : BaseCommand
        {
            [Command("subcommand"), Description("Subcommand description.")]
            public static Task ExecuteAsync(CommandContext context) => Task.CompletedTask;
        }
    }
}
