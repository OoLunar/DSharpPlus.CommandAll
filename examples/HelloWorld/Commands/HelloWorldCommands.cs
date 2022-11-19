using System.ComponentModel;
using System.Threading.Tasks;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples
{
    public sealed class HelloWorldCommand : BaseCommand
    {
        [Command("hello")]
        [Description("Says hello to the user")]
        public static Task HelloWorldAsync(CommandContext context, string text, string? test = null) => context.ReplyAsync(new()
        {
            Content = $"Hello World! {text} {test ?? "I was null!"}"
        });
    }
}
