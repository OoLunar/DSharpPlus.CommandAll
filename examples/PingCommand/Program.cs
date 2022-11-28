using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace OoLunar.DSharpPlus.CommandAll.Examples.PingCommand
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

            DiscordClient client = new(new DiscordConfiguration()
            {
                Token = args[0],
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            CommandAllExtension extension = client.UseCommandAll(new() // Register the extension
            {
                DebugGuildId = debugGuildId // Which guild to register the debug slash commands to.
            });
            extension.AddCommands(typeof(Program).Assembly); // Add all commands in this assembly

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
