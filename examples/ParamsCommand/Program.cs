using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSharpPlus.CommandAll.Examples.ParamsCommand
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

            DiscordShardedClient client = new(new DiscordConfiguration()
            {
                Token = args[0],
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            IReadOnlyDictionary<int, CommandAllExtension> extensions = await client.UseCommandAllAsync(new() // Register the extension
            {
                DebugGuildId = debugGuildId // Which guild to register the debug slash commands to.
            });

            foreach (CommandAllExtension extension in extensions.Values)
            {
                extension.AddCommands(typeof(Program).Assembly); // Add all commands in this assembly
            }

            await client.StartAsync();
            await Task.Delay(-1);
        }
    }
}
