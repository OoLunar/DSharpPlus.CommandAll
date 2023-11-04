using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.TextCommands;
using DSharpPlus.CommandAll.Processors.TextCommands.Attributes;

namespace DSharpPlus.CommandAll.Tests.Cases
{
    public class TestSingleLevelSubCommands
    {
        [Command("tag")]
        public class TagCommand
        {
            [Command("add")]
            public static Task AddAsync(TextContext context, string name, [RemainingText] string content) => Task.CompletedTask;

            [Command("get")]
            public static Task GetAsync(CommandContext context, string name) => Task.CompletedTask;
        }

        [Command("empty")]
        public class EmptyCommand { }
    }
}
