using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class CommandOverloadParsing : BaseTestClass
    {
        private readonly Command EchoCommand;

        public CommandOverloadParsing()
        {
            CommandBuilder command = CommandBuilder.Parse(Extension, typeof(Commands.EchoCommand)).First();
            Extension.ArgumentConverterManager.TrySaturateParameters(command.Overloads.SelectMany(x => x.Parameters), out _);
            EchoCommand = new Command(command);
        }

        [TestMethod]
        public void SingleOverload()
        {
            Assert.IsTrue(Extension.CommandOverloadParser.TryParseOverload(EchoCommand, new[] { "hello world" }, out CommandOverload? overload));
            Assert.IsNotNull(overload);
            Assert.AreEqual(1, overload.Parameters.Count);
            Assert.AreEqual(EchoCommand.Overloads[0], overload);
        }
    }
}
