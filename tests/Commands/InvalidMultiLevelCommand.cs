using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Tests.Commands
{
    [Command("invalid_command"), Description("This command is valid.")]
    public sealed class InvalidMultiLevelCommand : BaseCommand
    {
        [Command("group_command"), Description("This group command is valid.")]
        public sealed class GroupCommand : BaseCommand
        {
            [Command("sub_group_command"), Description("This sub group command is invalid.")]
            public sealed class SubGroupCommand : BaseCommand
            {
                [Command("sub_sub_command"), Description("This sub sub command is invalid.")]
                public static Task ExecuteAsync(CommandContext context) => Task.CompletedTask;
            }
        }
    }
}
