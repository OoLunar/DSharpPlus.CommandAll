using System.ComponentModel;
using System.Threading.Tasks;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples.ArgumentConverter.Commands
{
    public sealed class AddCommand : BaseCommand
    {
        [Command("add"), Description("Adds two numbers together.")]
        public static async Task ExecuteAsync(CommandContext context, ulong number1, ulong number2) => await context.ReplyAsync(new()
        {
            Content = $"{number1} + {number2} = {number1 + number2}"
        });
    }
}
