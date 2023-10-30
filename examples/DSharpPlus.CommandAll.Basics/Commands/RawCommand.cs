using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.SlashCommands.Attributes;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class RawCommand
    {
        [Command("raw"), SlashCommandTypes(ApplicationCommandType.SlashCommand, ApplicationCommandType.MessageContextMenu)]
        public static async Task ExecuteAsync(CommandContext context, DiscordMessage message) => await context.RespondAsync(new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder().WithDescription(Formatter.Sanitize(message.Content))));
    }
}
