using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class TextArgumentParsing
    {
        private readonly ITextArgumentParser _parser = new CommandsNextStyleTextArgumentParser(new CommandAllConfiguration());
        private readonly CommandAllExtension _extension = (CommandAllExtension)typeof(CommandAllExtension).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new[] { new CommandAllConfiguration() });

        [TestMethod]
        public void Input()
        {
            Assert.IsTrue(_parser.TryExtractArguments(_extension, "Hello World", out IReadOnlyList<string> arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(2, arguments.Count);
            Assert.AreEqual("Hello", arguments[0]);
            Assert.AreEqual("World", arguments[1]);
        }

        [TestMethod]
        public void SingleQuotes()
        {
            Assert.IsTrue(_parser.TryExtractArguments(_extension, "'Hello world'", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("Hello world", arguments[0]);
        }

        [TestMethod]
        public void SingleInlineCode()
        {
            Assert.IsTrue(_parser.TryExtractArguments(_extension, "`Hello world`", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("`Hello world`", arguments[0]);
        }

        [TestMethod]
        public void SingleBlockCode()
        {
            Assert.IsTrue(_parser.TryExtractArguments(_extension, "```Hello world```", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("```Hello world```", arguments[0]);
        }

        [TestMethod]
        public void SingleBlockCodeWithNewLines()
        {
            Assert.IsTrue(_parser.TryExtractArguments(_extension, "```\nHello world\n```", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("```\nHello world\n```", arguments[0]);
        }

        [TestMethod]
        public void DoubleNestedQuotes()
        {
            Assert.IsTrue(_parser.TryExtractArguments(_extension, "\"Hello 'world'\"", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("Hello 'world'", arguments[0]);
        }

        [TestMethod]
        public void DoubleNestedQuotesWithSpaces()
        {
            Assert.IsTrue(_parser.TryExtractArguments(_extension, "\"'Hello world'\"", out IReadOnlyList<string>? arguments));
            Assert.IsNotNull(arguments);
            Assert.AreEqual(1, arguments.Count);
            Assert.AreEqual("'Hello world'", arguments[0]);
        }
    }
}
