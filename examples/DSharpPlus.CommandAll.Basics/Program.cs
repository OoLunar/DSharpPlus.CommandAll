using System;
using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Processors.SlashCommands;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.CommandAll.Examples.Basics
{
    public static class Program
    {
        public static async Task Main()
        {
            DiscordClient client = new(new DiscordConfiguration()
            {
                Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new InvalidOperationException("No Discord bot token found."),
            });

            CommandAllExtension extension = client.UseCommandAll(new()
            {
                DebugGuildId = Environment.GetEnvironmentVariable("DEBUG_GUILD_ID") is string debugGuildId ? ulong.Parse(debugGuildId, CultureInfo.InvariantCulture) : null,
                ServiceProvider = new ServiceCollection().BuildServiceProvider()
            });
            extension.AddProcessor(new SlashCommandProcessor());
            extension.AddCommands(typeof(Program).Assembly);

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
