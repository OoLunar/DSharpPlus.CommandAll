using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class PrefixParsing : BaseTestClass
    {
        [TestMethod]
        public void SingleChar()
        {
            Assert.IsTrue(Extension.PrefixParser.TryRemovePrefix(Extension, "!hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }

        [TestMethod]
        public void DoubleChar()
        {
            Assert.IsTrue(Extension.PrefixParser.TryRemovePrefix(Extension, ">>hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }

        [TestMethod]
        public void MultiWord()
        {
            Assert.IsTrue(Extension.PrefixParser.TryRemovePrefix(Extension, "hey bot, hello world", out string? content));
            Assert.IsNotNull(content);
            Assert.AreEqual("hello world", content);
        }
    }
}
