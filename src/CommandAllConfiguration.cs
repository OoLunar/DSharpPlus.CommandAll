using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Commands.Executors;
using OoLunar.DSharpPlus.CommandAll.Managers;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll
{
    public sealed class CommandAllConfiguration
    {
        public IServiceCollection ServiceCollection { get; set; }
        public ICommandExecutor CommandExecutor { get; set; }
        public ICommandManager CommandManager { get; set; }
        public IArgumentConverterManager ArgumentConverterManager { get; set; }
        public ICommandOverloadParser CommandOverloadParser { get; set; }
        public IPrefixParser PrefixParser { get; set; }
        public ITextArgumentParser TextArgumentParser { get; set; }
        public char[] QuoteCharacters { get; set; }
        public ulong DebugGuildId { get; set; }

        public CommandAllConfiguration(IServiceCollection? serviceDescriptors = null)
        {
            ServiceCollection = serviceDescriptors ?? new ServiceCollection().AddSingleton<ILoggerFactory, NullLoggerFactory>().AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            QuoteCharacters = new[] { '"', '\'', '«', '»', '‘', '“', '„', '‟' };

            IServiceProvider serviceProvider = ServiceCollection.BuildServiceProvider();
            ArgumentConverterManager = new ArgumentConverterManager(serviceProvider.GetService<ILogger<ArgumentConverterManager>>());
            CommandOverloadParser = new CommandOverloadParser(serviceProvider.GetService<ILogger<CommandOverloadParser>>());
            PrefixParser = new PrefixParser();
            CommandExecutor = new CommandExecutor(serviceProvider.GetService<ILogger<CommandExecutor>>());
            CommandManager = new CommandManager(serviceProvider.GetService<ILogger<CommandManager>>());
            TextArgumentParser = new CommandsNextStyleTextArgumentParser(this);
        }
    }
}
