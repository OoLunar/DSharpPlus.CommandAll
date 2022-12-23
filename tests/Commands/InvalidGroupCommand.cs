using System.ComponentModel;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Tests.Commands
{
    [Command("invalid_group_command"), Description("A command for testing.")]
    public sealed class InvalidGroupCommand : BaseCommand
    {

    }
}
