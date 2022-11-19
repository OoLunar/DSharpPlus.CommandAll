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

        public CommandAllConfiguration()
        {
            ServiceCollection = new ServiceCollection().AddSingleton<ILoggerFactory, NullLoggerFactory>().AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            QuoteCharacters = new[] { '"', '«', '»', '‘', '“', '„', '‟' };
            ArgumentConverterManager = new ArgumentConverterManager();
            CommandOverloadParser = new CommandOverloadParser();
            PrefixParser = new PrefixParser();
            TextArgumentParser = new CommandsNextStyleTextArgumentParser(this);
            CommandExecutor = new CommandExecutor();
            CommandManager = new CommandManager(ArgumentConverterManager, CommandOverloadParser);
        }
    }
}
