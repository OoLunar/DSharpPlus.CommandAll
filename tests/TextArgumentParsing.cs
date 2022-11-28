using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class TextArgumentParsing : BaseTestClass
    {
        [TestMethod]
        public void Input()
        {
            Assert.IsTrue(Extension.TextArgumentParser.TryExtractArguments(Extension, "Hello World", out IReadOnlyList<string> arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(2, arguments.Count);
            Assert.AreEqual("Hello", arguments[0]);
            Assert.AreEqual("World", arguments[1]);
        }

        [TestMethod]
        public void SingleQuotes()
        {
            Assert.IsTrue(Extension.TextArgumentParser.TryExtractArguments(Extension, "'Hello world'", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("Hello world", arguments[0]);
        }

        [TestMethod]
        public void SingleInlineCode()
        {
            Assert.IsTrue(Extension.TextArgumentParser.TryExtractArguments(Extension, "`Hello world`", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("`Hello world`", arguments[0]);
        }

        [TestMethod]
        public void SingleBlockCode()
        {
            Assert.IsTrue(Extension.TextArgumentParser.TryExtractArguments(Extension, "```Hello world```", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("```Hello world```", arguments[0]);
        }

        [TestMethod]
        public void SingleBlockCodeWithNewLines()
        {
            Assert.IsTrue(Extension.TextArgumentParser.TryExtractArguments(Extension, "```\nHello world\n```", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("```\nHello world\n```", arguments[0]);
        }

        [TestMethod]
        public void DoubleNestedQuotes()
        {
            Assert.IsTrue(Extension.TextArgumentParser.TryExtractArguments(Extension, "\"Hello 'world'\"", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("Hello 'world'", arguments[0]);
        }

        [TestMethod]
        public void DoubleNestedQuotesWithSpaces()
        {
            Assert.IsTrue(Extension.TextArgumentParser.TryExtractArguments(Extension, "\"'Hello world'\"", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("'Hello world'", arguments[0]);
        }
    }
}
