using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Commands.Builders;
using DSharpPlus.CommandAll.Tests.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DSharpPlus.CommandAll.Tests
{
    [TestClass]
    public sealed class CommandParsing : BaseTestClass
    {
        public CommandParsing()
        {
            Extension.AddCommands(typeof(EchoCommand), typeof(PingCommand), typeof(MultiLevelCommand));
            Extension.ArgumentConverterManager.TrySaturateParameters(Extension.CommandManager.GetCommandBuilders().SelectMany(x => x.Overloads.SelectMany(y => y.Parameters)), out _);
            Extension.CommandManager.RegisterCommandsAsync(Extension).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TopLevelCommand()
        {
            Assert.IsTrue(Extension.CommandManager.TryFindCommand("ping", out Command? command, out _));
            Assert.IsNotNull(command);
            Assert.AreEqual("Ping", command.Name);
        }

        [TestMethod]
        public void SubCommand()
        {
            Assert.IsTrue(Extension.CommandManager.TryFindCommand("command subcommand", out Command? command, out _));
            Assert.IsNotNull(command);
            Assert.AreEqual("Subcommand", command.Name);
            Assert.AreEqual("Command Subcommand", command.FullName);
        }

        [TestMethod]
        public void GroupSubCommand()
        {
            Assert.IsTrue(Extension.CommandManager.TryFindCommand("command group subcommand", out Command? command, out _));
            Assert.IsNotNull(command);
            Assert.AreEqual("Subcommand", command.Name);
            Assert.AreEqual("Command Group Subcommand", command.FullName);
        }

        [TestMethod]
        public void InvalidMultiLevelCommand()
        {
            Assert.IsFalse(CommandBuilder.TryParse(Extension, typeof(InvalidMultiLevelCommand), out IReadOnlyList<CommandBuilder>? builders, out Exception? error));
            Assert.IsNull(builders);
            Assert.IsInstanceOfType(error, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void InvalidGroupCommand()
        {
            Assert.IsFalse(CommandBuilder.TryParse(Extension, typeof(InvalidGroupCommand), out IReadOnlyList<CommandBuilder>? builders, out Exception? error));
            Assert.IsNull(builders);
            Assert.IsInstanceOfType(error, typeof(ArgumentNullException));
        }
    }
}
