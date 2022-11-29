using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    public class BaseTestClass
    {
        public readonly CommandAllExtension Extension;

        public BaseTestClass() => Extension = (CommandAllExtension)typeof(CommandAllExtension).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new[] { new CommandAllConfiguration(new ServiceCollection().AddSingleton<ILoggerFactory, NullLoggerFactory>().AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))) {
            PrefixParser = new PrefixParser("!", ">>", "hey bot,")
        } });
    }
}
