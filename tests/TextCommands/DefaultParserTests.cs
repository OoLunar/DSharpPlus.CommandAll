using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.CommandAll.Processors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DSharpPlus.CommandAll.Tests.Processors.TextCommands
{
    [TestClass]
    public class DefaultParserTests
    {
        public TextCommandProcessor Processor { get; init; } = new();

        public List<string> HelloWorlds { get; init; } =
        [
            "Hello World!",
            "Hello 'World!'",
            "Hello \"World!\"",
            "Hello «World!»",
            "Hello ‘World!’",
            "Hello “World!”",
            "Hello „World!‟",
            "Hello ‟World!‟",
            "Hello World!",
            "'Hello World!'",
            "«Hello World!»",
            "‘Hello World!’",
            "“Hello World!”",
            "„Hello World!‟"
        ];

        [TestMethod]
        public void TestHelloWorlds()
        {
            List<string> parsedTexts = [];
            foreach (string helloWorld in HelloWorlds)
            {
                StringBuilder stringBuilder = new();
                int index = 0;
                while (index != -1)
                {
                    index = Processor.Configuration.TextArgumentSplicer(null!, helloWorld, index, out ReadOnlySpan<char> argument);
                    stringBuilder.Append(argument);
                }

                parsedTexts.Add(stringBuilder.ToString());
            }

            CollectionAssert.AreEquivalent(Enumerable.Repeat("Hello World!", parsedTexts.Count).ToArray(), parsedTexts);
        }
    }
}
