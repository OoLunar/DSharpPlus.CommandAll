using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Managers;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class CommandOverloadParsing
    {
        private readonly ICommandOverloadParser _parser = new CommandOverloadParser();
        private readonly Command EchoCommand;

        public CommandOverloadParsing()
        {
            CommandBuilder command = CommandBuilder.Parse(typeof(Commands.EchoCommand)).First();
            ArgumentConverterManager argumentConverterManager = new();
            argumentConverterManager.AddArgumentConverters(typeof(CommandAllExtension).Assembly.DefinedTypes.Where(type => type.Namespace == "OoLunar.DSharpPlus.CommandAll.Converters"));
            argumentConverterManager.TryAddParameters(command.Overloads.SelectMany(x => x.Parameters), out _);
            EchoCommand = new Command(command);
        }

        [TestMethod]
        public void SingleOverload()
        {
            Assert.IsTrue(_parser.TryParseOverload(EchoCommand, new[] { "hello world" }, out CommandOverload? overload));
            Assert.IsNotNull(overload);
            Assert.AreEqual(2, overload.Parameters.Count);
            Assert.AreEqual(EchoCommand.Overloads[0], overload);
        }
    }
}
