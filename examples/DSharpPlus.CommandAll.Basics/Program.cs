using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Processors.MessageCommands;
using DSharpPlus.CommandAll.Processors.SlashCommands;
using DSharpPlus.CommandAll.Processors.UserCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

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
                ServiceProvider = new ServiceCollection().AddLogging(loggerBuilder =>
                {
                    const string loggingFormat = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u4}] {SourceContext}: {Message:lj}{NewLine}{Exception}";
                    IConfiguration configuration = new ConfigurationBuilder().Build();
                    LoggerConfiguration loggingConfiguration = new LoggerConfiguration()
                        .MinimumLevel.Is(configuration.GetValue("logging:level", LogEventLevel.Debug))
                        .WriteTo.Console(outputTemplate: loggingFormat, formatProvider: CultureInfo.InvariantCulture, theme: new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
                        {
                            [ConsoleThemeStyle.Text] = "\x1b[0m",
                            [ConsoleThemeStyle.SecondaryText] = "\x1b[90m",
                            [ConsoleThemeStyle.TertiaryText] = "\x1b[90m",
                            [ConsoleThemeStyle.Invalid] = "\x1b[31m",
                            [ConsoleThemeStyle.Null] = "\x1b[95m",
                            [ConsoleThemeStyle.Name] = "\x1b[93m",
                            [ConsoleThemeStyle.String] = "\x1b[96m",
                            [ConsoleThemeStyle.Number] = "\x1b[95m",
                            [ConsoleThemeStyle.Boolean] = "\x1b[95m",
                            [ConsoleThemeStyle.Scalar] = "\x1b[95m",
                            [ConsoleThemeStyle.LevelVerbose] = "\x1b[34m",
                            [ConsoleThemeStyle.LevelDebug] = "\x1b[90m",
                            [ConsoleThemeStyle.LevelInformation] = "\x1b[36m",
                            [ConsoleThemeStyle.LevelWarning] = "\x1b[33m",
                            [ConsoleThemeStyle.LevelError] = "\x1b[31m",
                            [ConsoleThemeStyle.LevelFatal] = "\x1b[97;91m"
                        }))
                        .WriteTo.File(
                            $"logs/{DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd' 'HH'.'mm'.'ss", CultureInfo.InvariantCulture)}.log",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: loggingFormat,
                            formatProvider: CultureInfo.InvariantCulture
                        );

                    // Allow specific namespace log level overrides, which allows us to hush output from things like the database basic SELECT queries on the Information level.
                    foreach (IConfigurationSection logOverride in configuration.GetSection("logging:overrides").GetChildren())
                    {
                        if (logOverride.Value is null || !Enum.TryParse(logOverride.Value, out LogEventLevel logEventLevel))
                        {
                            continue;
                        }

                        loggingConfiguration.MinimumLevel.Override(logOverride.Key, logEventLevel);
                    }

                    loggerBuilder.AddSerilog(loggingConfiguration.CreateLogger());
                }).BuildServiceProvider()
            });
            extension.AddProcessor(new UserCommandProcessor());
            extension.AddProcessor(new MessageCommandProcessor());
            extension.AddProcessor(new SlashCommandProcessor());
            extension.AddCommands(typeof(Program).Assembly);

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
