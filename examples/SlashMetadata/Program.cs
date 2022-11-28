using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands;
using OoLunar.DSharpPlus.CommandAll.EventArgs;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace OoLunar.DSharpPlus.CommandAll.Examples.SlashMetadata
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please provide a bot token and debug guild id, in that order.");
                return;
            }

            if (!ulong.TryParse(args[1], out ulong debugGuildId))
            {
                Console.WriteLine("Please provide a valid debug guild id as the second argument.");
                return;
            }

            ServiceCollection services = new();
            services.AddLogging(logger =>
            {
                string loggingFormat = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u4}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

                // Log both to console and the file
                LoggerConfiguration loggerConfiguration = new LoggerConfiguration().MinimumLevel.Is(LogEventLevel.Information)
                .WriteTo.Console(outputTemplate: loggingFormat, theme: new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
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
                    $"logs/{DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd' 'HH'_'mm'_'ss", CultureInfo.InvariantCulture)}.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: loggingFormat
                );

                // Set Log.Logger for a static reference to the logger
                logger.AddSerilog(loggerConfiguration.CreateLogger());
            });

            DiscordClient client = new(new DiscordConfiguration()
            {
                Token = args[0],
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
                LoggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>()
            });

            CommandAllExtension extension = client.UseCommandAll(new(services) // Register the extension
            {
                DebugGuildId = debugGuildId // Which guild to register the debug slash commands to.
            });
            extension.ArgumentConverterManager.AddArgumentConverters(typeof(Program).Assembly); // Register all argument converters in the assembly
            extension.AddCommands(typeof(Program).Assembly); // Add all commands in this assembly
            extension.ConfigureCommands += TranslateCommands;

            await client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task TranslateCommands(CommandAllExtension extension, ConfigureCommandsEventArgs eventArgs)
        {
            CommandBuilder? pingBuilder = eventArgs.CommandManager.CommandBuilders.Values.FirstOrDefault(x => x.Name == "ping");
            if (pingBuilder is not null)
            {
                pingBuilder.SlashMetadata.LocalizedNames.Add(CultureInfo.GetCultureInfo("ru-RU"), "пинг");
                pingBuilder.SlashMetadata.LocalizedDescriptions.Add(CultureInfo.GetCultureInfo("ru-RU"), "Проверяет, жив ли бот.");

                pingBuilder.Overloads[0].SlashMetadata.LocalizedNames.Add(CultureInfo.GetCultureInfo("ru-RU"), "пинг");
                pingBuilder.Overloads[0].SlashMetadata.LocalizedDescriptions.Add(CultureInfo.GetCultureInfo("ru-RU"), "Проверяет, жив ли бот.");
            }

            CommandBuilder? pinnedMessageCountBuilder = eventArgs.CommandManager.CommandBuilders.Values.FirstOrDefault(x => x.Name == "pinned_message_count");
            if (pinnedMessageCountBuilder is not null)
            {
                pinnedMessageCountBuilder.Overloads[0].Parameters[0].SlashMetadata.ChannelTypes = new List<ChannelType> { ChannelType.Text, ChannelType.News };
            }
            return Task.CompletedTask;
        }
    }
}
