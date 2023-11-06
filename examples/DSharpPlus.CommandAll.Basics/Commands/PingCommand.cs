using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.SlashCommands.Attributes;
using DSharpPlus.CommandAll.Processors.SlashCommands.Translation;
using DSharpPlus.CommandAll.Processors.TextCommands.Attributes;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class PingCommand
    {
        [Command("ping"), SlashLocalizer<PingTranslator>, TextAlias("pong")]
        public static async Task ExecuteAsync(CommandContext context) => await context.RespondAsync("Pong!");
    }

    public sealed class PingTranslator : ITranslator
    {
        public Task<IReadOnlyDictionary<Locales, string>> TranslateAsync(string fullSymbolName) => Task.FromResult<IReadOnlyDictionary<Locales, string>>(new Dictionary<Locales, string>
        {
            { Locales.en_US, "ping" },
            { Locales.ja, "ピング" }
        });
    }
}
