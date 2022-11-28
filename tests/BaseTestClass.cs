using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    public class BaseTestClass
    {
        public readonly CommandAllExtension Extension;

        public BaseTestClass() => Extension = (CommandAllExtension)typeof(CommandAllExtension).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new[] { new CommandAllConfiguration() {
            PrefixParser = new PrefixParser("!", ">>", "hey bot,")
        } });
    }
}
