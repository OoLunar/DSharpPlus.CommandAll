using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;

namespace OoLunar.DSharpPlus.CommandAll
{
    public static class ExtensionMethods
    {
        public static CommandAllExtension UseCommandsAll(this DiscordClient client, CommandAllConfiguration? configuration = null)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            else if (client.GetExtension<CommandAllExtension>() != null)
            {
                throw new InvalidOperationException("CommandAll Extension is already initialized.");
            }

            configuration ??= new();
            CommandAllExtension extension = new(configuration);
            client.AddExtension(extension);
            return extension;
        }

        public static Task<IReadOnlyDictionary<int, CommandAllExtension>> UseCommandsAllAsync(this DiscordShardedClient shardedClient, CommandAllConfiguration? configuration = null)
        {
            if (shardedClient == null)
            {
                throw new ArgumentNullException(nameof(shardedClient));
            }

            configuration ??= new();
            Dictionary<int, CommandAllExtension> extensions = new();
            foreach (DiscordClient shard in shardedClient.ShardClients.Values)
            {
                extensions[shard.ShardId] = shard.GetExtension<CommandAllExtension>() ?? shard.UseCommandsAll(configuration);
            }

            return Task.FromResult((IReadOnlyDictionary<int, CommandAllExtension>)extensions.AsReadOnly());
        }

        public static CommandAllExtension? GetCommandsAllExtension(this DiscordClient client) => client == null
            ? throw new ArgumentNullException(nameof(client))
            : client.GetExtension<CommandAllExtension>();

        public static IReadOnlyDictionary<int, CommandAllExtension> GetCommandsAllExtensions(this DiscordShardedClient shardedClient)
        {
            if (shardedClient == null)
            {
                throw new ArgumentNullException(nameof(shardedClient));
            }

            Dictionary<int, CommandAllExtension> extensions = new();
            foreach (DiscordClient shard in shardedClient.ShardClients.Values)
            {
                CommandAllExtension? extension = shard.GetExtension<CommandAllExtension>();
                if (extension != null)
                {
                    extensions[shard.ShardId] = extension;
                }
            }

            return extensions.AsReadOnly();
        }
    }
}
