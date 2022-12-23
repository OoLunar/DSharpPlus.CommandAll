using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Attributes;
using DSharpPlus.CommandAll.Commands;

namespace DSharpPlus.CommandAll.Examples.HelpCommand.Commands
{
    public sealed class EchoCommand : BaseCommand
    {
        [Command("echo"), Description("Echoes the given text.")]
        public static Task EchoAsync(CommandContext context, [Description("The text to echo.")] string text) => context.ReplyAsync(text);
    }
}
