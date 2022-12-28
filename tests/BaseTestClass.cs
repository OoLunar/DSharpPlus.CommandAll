using DSharpPlus.CommandAll.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DSharpPlus.CommandAll.Tests
{
    public class BaseTestClass
    {
        public readonly CommandAllExtension Extension;
        public readonly DiscordClient Client;

        public BaseTestClass()
        {
            DiscordConfiguration discordConfiguration = new()
            {
                // The command manager will register slash commands
                // by default when RegisterCommandsAsync is called.
                // We can prevent this by setting the shard id to 1
                // since RegisterCommandsAsync will only register
                // slash commands when the shard id is 0.
                ShardId = 1,
                Token = "invalid token that won't be used"
            };
            Client = new(discordConfiguration);
            Extension = Client.UseCommandAll(new(new ServiceCollection().AddSingleton<ILoggerFactory, NullLoggerFactory>().AddSingleton<ILogger, NullLogger>().AddSingleton(typeof(ILogger<>), typeof(NullLogger<>)))
            {
                PrefixParser = new PrefixParser("!", ">>", "hey bot,")
            });
        }
    }
}
