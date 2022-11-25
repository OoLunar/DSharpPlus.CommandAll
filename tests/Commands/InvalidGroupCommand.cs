using System.ComponentModel;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Tests.Commands
{
    [Command("invalid_group_command"), Description("A command for testing.")]
    public sealed class InvalidGroupCommand : BaseCommand
    {

    }
}
