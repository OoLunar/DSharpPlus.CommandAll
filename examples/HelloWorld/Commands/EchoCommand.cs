using System.ComponentModel;
using System.Threading.Tasks;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples.HelloWorld
{
    public sealed class EchoCommand : BaseCommand
    {
        [Command("echo")]
        [Description("Repeats what the user said.")]
        public static Task EchoAsync(CommandContext context, string text) => context.ReplyAsync(new()
        {
            Content = text
        });
    }
}
