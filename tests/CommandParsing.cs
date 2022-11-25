using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Managers;
using OoLunar.DSharpPlus.CommandAll.Tests.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class CommandParsing
    {
        private readonly CommandManager _commandManager = new();
        private readonly ArgumentConverterManager _argumentConverterManager = new();

        public CommandParsing()
        {
            _commandManager.AddCommands(new[] { typeof(EchoCommand), typeof(PingCommand), typeof(MultiLevelCommand) });
            _argumentConverterManager.AddArgumentConverters(typeof(CommandAllExtension).Assembly.DefinedTypes.Where(type => type.Namespace == "OoLunar.DSharpPlus.CommandAll.Converters"));
            _argumentConverterManager.TryAddParameters(_commandManager.CommandBuilders.Values.SelectMany(x => x.Overloads.SelectMany(y => y.Parameters)), out _);
            _commandManager.BuildCommands();
        }

        [TestMethod]
        public void TopLevelCommand()
        {
            Assert.IsTrue(_commandManager.TryFindCommand("ping", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("Ping", command.Name);
        }

        [TestMethod]
        public void SubCommand()
        {
            Assert.IsTrue(_commandManager.TryFindCommand("command subcommand", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("Subcommand", command.Name);
            Assert.AreEqual("Command Subcommand", command.FullName);
        }

        [TestMethod]
        public void GroupSubCommand()
        {
            Assert.IsTrue(_commandManager.TryFindCommand("command group subcommand", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("Subcommand", command.Name);
            Assert.AreEqual("Command Group Subcommand", command.FullName);
        }

        [TestMethod]
        public void InvalidMultiLevelCommand()
        {
            Assert.IsFalse(CommandBuilder.TryParse(typeof(InvalidMultiLevelCommand), out IEnumerable<CommandBuilder>? builders, out Exception? error));
            Assert.IsNull(builders);
            Assert.IsInstanceOfType(error, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void InvalidGroupCommand()
        {
            Assert.IsFalse(CommandBuilder.TryParse(typeof(InvalidGroupCommand), out IEnumerable<CommandBuilder>? builders, out Exception? error));
            Assert.IsNull(builders);
            Assert.IsInstanceOfType(error, typeof(ArgumentNullException));
        }
    }
}
