using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Managers;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class CommandParsing
    {
        private readonly CommandManager _commandManager = new();
        private readonly ArgumentConverterManager _argumentConverterManager = new();

        public CommandParsing()
        {
            _commandManager.AddCommands(typeof(CommandParsing).Assembly.DefinedTypes.Where(type => type.Namespace == "OoLunar.DSharpPlus.CommandAll.Tests.Commands"));
            _argumentConverterManager.AddArgumentConverters(typeof(CommandAllExtension).Assembly.DefinedTypes.Where(type => type.Namespace == "OoLunar.DSharpPlus.CommandAll.Converters"));
            _argumentConverterManager.TryAddParameters(_commandManager.CommandBuilders.Values.SelectMany(x => x.Overloads.SelectMany(y => y.Parameters)), out _);
            _commandManager.BuildCommands();
        }

        [TestMethod]
        public void TopLevelCommand()
        {
            Assert.IsTrue(_commandManager.TryFindCommand("ping", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("ping", command.Name);
        }

        [TestMethod]
        public void SubCommand()
        {
            Assert.IsTrue(_commandManager.TryFindCommand("command subcommand", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("subcommand", command.Name);
            Assert.AreEqual("command subcommand", command.FullName);
        }

        [TestMethod]
        public void GroupSubCommand()
        {
            Assert.IsTrue(_commandManager.TryFindCommand("command group subcommand", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("subcommand", command.Name);
            Assert.AreEqual("command group subcommand", command.FullName);
        }
    }
}
