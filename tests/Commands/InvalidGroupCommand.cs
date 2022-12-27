using System.ComponentModel;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Tests.Commands
{
    [Command("invalid_group_command"), Description("An empty and invalid group command.")]
    public sealed class InvalidGroupCommand : BaseCommand { }
}
