using System.Linq;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class CommandOverloadParsing
    {
        private readonly ICommandOverloadParser _parser = new CommandOverloadParser();
        private readonly Command EchoCommand = new(CommandBuilder.Parse(typeof(Commands.EchoCommand)).First());

        [TestMethod]
        public void SingleOverload()
        {
            Assert.IsTrue(_parser.TryParseOverload(EchoCommand, new[] { "hello world" }, out CommandOverload? overload));
            Assert.IsNotNull(overload);
            Assert.AreEqual(2, overload.Parameters.Count);
            Assert.AreEqual(EchoCommand.Overloads[1], overload);
        }
    }
}
