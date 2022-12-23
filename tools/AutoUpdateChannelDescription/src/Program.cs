using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace DSharpPlus.CommandAll.Tools.AutoUpdateChannelDescription
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
            string nugetUrl = Environment.GetEnvironmentVariable("NUGET_URL") ?? throw new InvalidOperationException("NUGET_URL environment variable is not set.");
            string githubUrl = Environment.GetEnvironmentVariable("GITHUB_URL") ?? throw new InvalidOperationException("GITHUB_URL environment variable is not set.");

            DiscordClient client = new(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            client.GuildDownloadCompleted += (client, eventArgs) =>
            {
                DiscordGuild guild = client.Guilds[ulong.Parse(guildId, NumberStyles.Number, CultureInfo.InvariantCulture)];
                DiscordChannel channel = guild.Channels[ulong.Parse(channelId, NumberStyles.Number, CultureInfo.InvariantCulture)];

                // Task.Run in case ratelimit gets hit and event handler is cancelled.
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await channel.ModifyAsync(channel =>
                        {
                            string nightlyVersion = typeof(CommandAllExtension).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
                            channel.AuditLogReason = $"Updating channel topic to match stable version {latestStableVersion} and nightly version {nightlyVersion}.";
                            channel.Topic = @$"{channelTopic}
{Formatter.Bold("GitHub")}: {githubUrl}
{Formatter.Bold("Latest stable version")}: {nugetUrl}/{latestStableVersion}
{Formatter.Bold("Latest preview version")}: {nugetUrl}/{nightlyVersion}";
                        });
                    }
                    catch (DiscordException error)
                    {
                        Console.WriteLine($"Error: HTTP {error.WebResponse.ResponseCode}, {error.WebResponse.Response}");
                    }

                    await client.DisconnectAsync();
                    Environment.Exit(0);
                });

                return Task.CompletedTask;
            };

            await client.ConnectAsync();

            // The program should exit ASAP after the channel description is updated.
            // However it may get caught in a ratelimit, so we'll wait for a bit.
            // The program will exit after 10 seconds no matter what.
            // This includes the time it takes to connect to the Discord gateway.
            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }
}
