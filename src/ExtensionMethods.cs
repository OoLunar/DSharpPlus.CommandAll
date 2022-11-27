using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;

namespace OoLunar.DSharpPlus.CommandAll
{
    /// <summary>
    /// Extension methods used by the <see cref="CommandAllExtension"/> for the <see cref="DiscordClient"/>.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Registers the extension with the <see cref="DiscordClient"/>.
        /// </summary>
        /// <param name="client">The client to register the extension with.</param>
        /// <param name="configuration">The configuration to use for the extension.</param>
        public static CommandAllExtension UseCommandAll(this DiscordClient client, CommandAllConfiguration? configuration = null)
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

        /// <summary>
        /// Registers the extension with all the shards on the <see cref="DiscordShardedClient"/>.
        /// </summary>
        /// <param name="shardedClient">The client to register the extension with.</param>
        /// <param name="configuration">The configuration to use for the extension.</param>
        public static Task<IReadOnlyDictionary<int, CommandAllExtension>> UseCommandAllAsync(this DiscordShardedClient shardedClient, CommandAllConfiguration? configuration = null)
        {
            if (shardedClient == null)
            {
                throw new ArgumentNullException(nameof(shardedClient));
            }

            configuration ??= new();
            Dictionary<int, CommandAllExtension> extensions = new();
            foreach (DiscordClient shard in shardedClient.ShardClients.Values)
            {
                extensions[shard.ShardId] = shard.GetExtension<CommandAllExtension>() ?? shard.UseCommandAll(configuration);
            }

            return Task.FromResult((IReadOnlyDictionary<int, CommandAllExtension>)extensions.AsReadOnly());
        }

        /// <summary>
        /// Retrieves the <see cref="CommandAllExtension"/> from the <see cref="DiscordClient"/>.
        /// </summary>
        /// <param name="client">The client to retrieve the extension from.</param>
        public static CommandAllExtension? GetCommandAllExtension(this DiscordClient client) => client == null
            ? throw new ArgumentNullException(nameof(client))
            : client.GetExtension<CommandAllExtension>();

        /// <summary>
        /// Retrieves the <see cref="CommandAllExtension"/> from all of the shards on <see cref="DiscordShardedClient"/>.
        /// </summary>
        /// <param name="shardedClient">The client to retrieve the extension from.</param>
        public static IReadOnlyDictionary<int, CommandAllExtension> GetCommandAllExtensions(this DiscordShardedClient shardedClient)
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
