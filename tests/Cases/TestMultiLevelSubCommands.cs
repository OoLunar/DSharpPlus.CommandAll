using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Attributes;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Tests.Cases
{
    public class TestMultiLevelSubCommands
    {
        [Command("info")]
        public class InfoCommand
        {
            [Command("user")]
            public class UserCommand
            {
                [Command("avatar")]
                public static Task AvatarAsync(CommandContext context, DiscordUser user) => Task.CompletedTask;

                [Command("roles")]
                public static Task RolesAsync(CommandContext context, DiscordUser user) => Task.CompletedTask;

                [Command("permissions")]
                public static Task PermissionsAsync(CommandContext context, DiscordUser user, DiscordChannel? channel = null) => Task.CompletedTask;
            }

            [Command("channel")]
            public class ChannelCommand
            {
                [Command("created")]
                public static Task PermissionsAsync(CommandContext context, DiscordChannel channel) => Task.CompletedTask;

                [Command("members")]
                public static Task MembersAsync(CommandContext context, DiscordChannel channel) => Task.CompletedTask;
            }
        }
    }
}
