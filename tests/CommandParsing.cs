using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.System.Commands;
using OoLunar.DSharpPlus.CommandAll.Tests.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class CommandParsing : BaseTestClass
    {
        public CommandParsing()
        {
            Extension.AddCommands(new[] { typeof(EchoCommand), typeof(PingCommand), typeof(MultiLevelCommand) });
            Extension.ArgumentConverterManager.TrySaturateParameters(Extension.CommandManager.CommandBuilders.Values.SelectMany(x => x.Overloads.SelectMany(y => y.Parameters)), out _);
            Extension.CommandManager.BuildCommands();
        }

        [TestMethod]
        public void TopLevelCommand()
        {
            Assert.IsTrue(Extension.CommandManager.TryFindCommand("ping", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("Ping", command.Name);
        }

        [TestMethod]
        public void SubCommand()
        {
            Assert.IsTrue(Extension.CommandManager.TryFindCommand("command subcommand", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("Subcommand", command.Name);
            Assert.AreEqual("Command Subcommand", command.FullName);
        }

        [TestMethod]
        public void GroupSubCommand()
        {
            Assert.IsTrue(Extension.CommandManager.TryFindCommand("command group subcommand", out _, out Command? command));
            Assert.IsNotNull(command);
            Assert.AreEqual("Subcommand", command.Name);
            Assert.AreEqual("Command Group Subcommand", command.FullName);
        }

        [TestMethod]
        public void InvalidMultiLevelCommand()
        {
            Assert.IsFalse(CommandBuilder.TryParse(Extension, typeof(InvalidMultiLevelCommand), out IEnumerable<CommandBuilder>? builders, out Exception? error));
            Assert.IsNull(builders);
            Assert.IsInstanceOfType(error, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void InvalidGroupCommand()
        {
            Assert.IsFalse(CommandBuilder.TryParse(Extension, typeof(InvalidGroupCommand), out IEnumerable<CommandBuilder>? builders, out Exception? error));
            Assert.IsNull(builders);
            Assert.IsInstanceOfType(error, typeof(ArgumentNullException));
        }
    }
}
