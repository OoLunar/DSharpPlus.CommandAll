using System;
using System.Threading.Tasks;
using DSharpPlus;

namespace OoLunar.DSharpPlus.CommandAll.Examples.PingCommand
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please provide a bot token.");
                return;
            }

            DiscordClient client = new(new DiscordConfiguration()
            {
                Token = args[0],
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            CommandAllExtension extension = client.UseCommandAll(); // Register the extension
            extension.CommandManager.AddCommands(typeof(Program).Assembly); // Add all commands in this assembly

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
