using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using OoLunar.DSharpPlus.CommandAll.Attributes;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Examples.SlashMetadata.Commands
{
    public sealed class PinnedMessageCountCommand : BaseCommand
    {
        [Command("pinned_message_count"), Description("Gets the number of pinned messages in the channel.")]
        public static async Task GetPinnedMessageCountAsync(CommandContext context, [Description("The channel to grab.")] DiscordChannel channel)
        {
            IReadOnlyList<DiscordMessage> messages = await channel.GetPinnedMessagesAsync();
            await context.ReplyAsync($"There are {messages.Count} pinned messages in {channel.Mention}.");
        }
    }
}
