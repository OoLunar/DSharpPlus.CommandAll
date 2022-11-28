using Microsoft.VisualStudio.TestTools.UnitTesting;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class PrefixParsing : BaseTestClass
    {
        private readonly IPrefixParser _parser = new PrefixParser(">>", "!", "hey bot,");

        [TestMethod]
        public void SingleChar()
        {
            Assert.IsTrue(_parser.TryRemovePrefix(Extension, "!hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }

        [TestMethod]
        public void DoubleChar()
        {
            Assert.IsTrue(_parser.TryRemovePrefix(Extension, ">>hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }

        [TestMethod]
        public void MultiWord()
        {
            Assert.IsTrue(_parser.TryRemovePrefix(Extension, "hey bot, hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }
    }
}
