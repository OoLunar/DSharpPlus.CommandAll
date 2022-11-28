using System.Reflection;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    public class BaseTestClass
    {
        public readonly CommandAllExtension Extension;

        public BaseTestClass() => Extension = (CommandAllExtension)typeof(CommandAllExtension).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new[] { new CommandAllConfiguration() });
    }
}
