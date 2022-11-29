using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OoLunar.DSharpPlus.CommandAll
{
    /// <summary>
    /// Extension methods used by the <see cref="CommandAllExtension"/> for the <see cref="DiscordClient"/>.
    /// </summary>
    public static class ExtensionMethods
    {
        private static readonly Type _shardedLoggerFactoryType = typeof(DiscordClient).Assembly.GetType("DSharpPlus.ShardedLoggerFactory", true)!;

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

            configuration ??= new(new ServiceCollection());
            ServiceDescriptor? currentLoggingImplementation = configuration.ServiceCollection.FirstOrDefault(service => service.ServiceType == typeof(ILoggerFactory));

            // There are 4 scenarios here:
            // - The user does not provide a logging implementation.
            // - The user provides a logging implementation only to the DiscordClient.
            // - The user provides a the default ShardedLoggerFactory implementation through the services
            // - The user provides a custom logging implementation through the services

            // No implementation provided
            if (currentLoggingImplementation is null)
            {
                // Check if the client has a valid logging implementation
                Type clientType = client.Logger.GetType();
                if (clientType != _shardedLoggerFactoryType)
                {
                    Type[] clientInterfaces = clientType.GetInterfaces();
                    if (clientInterfaces.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ILogger<>)))
                    {
                        configuration.ServiceCollection.AddSingleton(typeof(ILogger<>), client.Logger);
                    }

                    if (clientInterfaces.Contains(typeof(ILoggerFactory)))
                    {
                        configuration.ServiceCollection.AddSingleton(typeof(ILoggerFactory), client.Logger);
                    }
                }
                else
                {
                    Console.WriteLine($"No logging system set, using a {nameof(NullLoggerFactory)}. This is not recommended, please provide a logging system so you can see errors.");
                    configuration.ServiceCollection.AddSingleton<ILoggerFactory, NullLoggerFactory>().AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
                }
            }
            // Check if they provided the ShardedLoggerFactory explicitly to the services
            else if (currentLoggingImplementation.ServiceType == _shardedLoggerFactoryType)
            {
                Console.WriteLine($"ShardedLoggerFactory detected, using {nameof(NullLoggerFactory)} instead. CommandAll is NOT compatible with the default logging system that DSharpPlus provides!");
                configuration.ServiceCollection
                    .RemoveAll<ILoggerFactory>().RemoveAll(typeof(ILogger<>)) // Remove the default logging implementation, if set
                    .AddSingleton<ILoggerFactory, NullLoggerFactory>().AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            }

            CommandAllExtension extension = new(configuration);
            client.AddExtension(extension);
            return extension;
        }

        /// <summary>
        /// Registers the extension with all the shards on the <see cref="DiscordShardedClient"/>.
        /// </summary>
        /// <param name="shardedClient">The client to register the extension with.</param>
        /// <param name="configuration">The configuration to use for the extension.</param>
        public static async Task<IReadOnlyDictionary<int, CommandAllExtension>> UseCommandAllAsync(this DiscordShardedClient shardedClient, CommandAllConfiguration? configuration = null)
        {
            if (shardedClient == null)
            {
                throw new ArgumentNullException(nameof(shardedClient));
            }

            await shardedClient.InitializeShardsAsync();
            configuration ??= new(new ServiceCollection());

            ServiceDescriptor? currentLoggingImplementation = configuration.ServiceCollection.FirstOrDefault(service => service.ServiceType == typeof(ILoggerFactory));
            if (currentLoggingImplementation is null)
            {
                Console.WriteLine($"No logging system set, using a {nameof(NullLoggerFactory)}. This is not recommended, please provide a logging system so you can see errors.");
                configuration.ServiceCollection.AddSingleton<ILoggerFactory, NullLoggerFactory>().AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            }
            else if (currentLoggingImplementation.ServiceType == _shardedLoggerFactoryType && shardedClient.Logger.GetType() == _shardedLoggerFactoryType)
            {
                Console.WriteLine($"ShardedLoggerFactory detected, using {nameof(NullLoggerFactory)} instead. CommandAll is NOT compatible with the default logging system that DSharpPlus provides!");
                configuration.ServiceCollection
                    .RemoveAll<ILoggerFactory>().RemoveAll(typeof(ILogger<>)) // Remove the default logging implementation, if set
                    .AddSingleton<ILoggerFactory, NullLoggerFactory>().AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            }

            Dictionary<int, CommandAllExtension> extensions = new();
            foreach (DiscordClient shard in shardedClient.ShardClients.Values)
            {
                extensions[shard.ShardId] = shard.GetExtension<CommandAllExtension>() ?? shard.UseCommandAll(configuration);
            }

            return extensions.AsReadOnly();
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
