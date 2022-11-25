using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class PrefixParsing
    {
        private readonly IPrefixParser _parser = new PrefixParser(">>", "!", "hey bot,");
        private readonly CommandAllExtension _extension;

        public PrefixParsing() => _extension = (CommandAllExtension)typeof(CommandAllExtension).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new[] { new CommandAllConfiguration() });

        [TestMethod]
        public void SingleChar()
        {
            Assert.IsTrue(_parser.TryRemovePrefix(_extension, "!hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }

        [TestMethod]
        public void DoubleChar()
        {
            Assert.IsTrue(_parser.TryRemovePrefix(_extension, ">>hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }

        [TestMethod]
        public void MultiWord()
        {
            Assert.IsTrue(_parser.TryRemovePrefix(_extension, "hey bot, hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }
    }
}
