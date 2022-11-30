using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace OoLunar.DSharpPlus.CommandAll.Tools
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            string token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new InvalidOperationException("DISCORD_TOKEN environment variable is not set.");
            string guildId = Environment.GetEnvironmentVariable("DISCORD_GUILD_ID") ?? throw new InvalidOperationException("DISCORD_GUILD_ID environment variable is not set.");
            string channelId = Environment.GetEnvironmentVariable("DISCORD_CHANNEL_ID") ?? throw new InvalidOperationException("DISCORD_CHANNEL_ID environment variable is not set.");
            string channelTopic = Environment.GetEnvironmentVariable("DISCORD_CHANNEL_TOPIC") ?? throw new InvalidOperationException("DISCORD_DESCRIPTION environment variable is not set.");
            string latestStableVersion = args.Length == 1 ? args[0] : throw new InvalidOperationException("LATEST_STABLE_VERSION should be the first argument passed.");
            string latestPreviewVersion = Environment.GetEnvironmentVariable("LATEST_PREVIEW_VERSION") ?? throw new InvalidOperationException("LATEST_PREVIEW_VERSION environment variable is not set.");
            string nugetUrl = Environment.GetEnvironmentVariable("NUGET_URL") ?? throw new InvalidOperationException("NUGET_URL environment variable is not set.");
            string githubUrl = Environment.GetEnvironmentVariable("GITHUB_URL") ?? throw new InvalidOperationException("GITHUB_URL environment variable is not set.");

            DiscordClient client = new(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            client.GuildDownloadCompleted += async (client, eventArgs) =>
            {
                DiscordGuild guild = client.Guilds[ulong.Parse(guildId)];
                DiscordChannel channel = guild.Channels[ulong.Parse(channelId)];
                await channel.ModifyAsync(channel => channel.Topic = $"{channelTopic}\n{Formatter.Bold("Github")}: {githubUrl}\nNuGet: {nugetUrl}\nLatest stable version: {new Uri(new Uri(nugetUrl), latestStableVersion)}\nLatest preview version: {new Uri(new Uri(nugetUrl), latestStableVersion)}");
                Environment.Exit(0);
            };

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
