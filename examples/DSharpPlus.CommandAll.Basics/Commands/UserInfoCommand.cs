using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.CommandAll.Processors.SlashCommands.Attributes;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Examples.Basics.Commands
{
    public sealed class UserInfoCommand
    {
        [Command("user_info"), DisplayName("User Info"), SlashCommandTypes(ApplicationCommandType.SlashCommand, ApplicationCommandType.UserContextMenu)]
        public static async Task ExecuteAsync(CommandContext context, DiscordUser user) => await context.RespondAsync(new DiscordMessageBuilder().WithEmbed(new DiscordEmbedBuilder().WithDescription($"This person's account was created on {user.CreationTimestamp:MMMM dd, yyyy}")));
    }
}
