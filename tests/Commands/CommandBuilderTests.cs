using System;
using System.Linq;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.CommandAll.Processors.TextCommands.Attributes;
using DSharpPlus.CommandAll.Tests.Cases;
using DSharpPlus.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DSharpPlus.CommandAll.Tests.Commands
{
    [TestClass]
    public class CommandBuilderTests
    {
        [TestMethod]
        public void TopLevelEmptyCommand() => Assert.ThrowsException<ArgumentException>(() => CommandBuilder.From<TestSingleLevelSubCommands.EmptyCommand>());

        [TestMethod]
        public void TopLevelCommandMissingContext() => Assert.ThrowsException<ArgumentException>(() => CommandBuilder.From(TestTopLevelCommands.OopsAsync));

        [TestMethod]
        public void TopLevelCommandNoParameters()
        {
            CommandBuilder commandBuilder = CommandBuilder.From(TestTopLevelCommands.PingAsync);
            Command command = commandBuilder.Build();
            Assert.AreEqual("ping", command.Name);
            Assert.AreEqual("No description provided.", command.Description);
            Assert.IsNull(command.Parent);
            Assert.IsNull(command.Target);
            Assert.AreEqual(((Delegate)TestTopLevelCommands.PingAsync).Method, command.Method);
            Assert.AreEqual(0, command.Subcommands.Count);
            Assert.AreEqual(0, command.Arguments.Count);
        }

        [TestMethod]
        public void TopLevelCommandOneParameter()
        {
            CommandBuilder commandBuilder = CommandBuilder.From(TestTopLevelCommands.EchoAsync);
            Command command = commandBuilder.Build();
            Assert.AreEqual(1, command.Arguments.Count);
            Assert.AreEqual("message", command.Arguments[0].Name);
            Assert.AreEqual("No description provided.", command.Arguments[0].Description);
            Assert.AreEqual(typeof(string), command.Arguments[0].Type);
            Assert.AreEqual(false, command.Arguments[0].DefaultValue.HasValue);
            Assert.IsTrue(command.Arguments[0].Attributes.Count != 0);
            Assert.IsTrue(command.Arguments[0].Attributes.Any(attribute => attribute is RemainingTextAttribute));
        }

        [TestMethod]
        public void TopLevelCommandOneOptionalParameter()
        {
            CommandBuilder commandBuilder = CommandBuilder.From(TestTopLevelCommands.UserInfoAsync);
            Command command = commandBuilder.Build();
            Assert.AreEqual(1, command.Arguments.Count);
            Assert.AreEqual("user", command.Arguments[0].Name);
            Assert.AreEqual("No description provided.", command.Arguments[0].Description);
            Assert.AreEqual(typeof(DiscordUser), command.Arguments[0].Type);
            Assert.AreEqual(true, command.Arguments[0].DefaultValue.HasValue);
            Assert.AreEqual(null, command.Arguments[0].DefaultValue.Value);
        }

        [TestMethod]
        public void SingleLevelSubCommands()
        {
            CommandBuilder commandBuilder = CommandBuilder.From<TestSingleLevelSubCommands.TagCommand>();
            Assert.AreEqual(2, commandBuilder.Subcommands.Count);

            Command command = commandBuilder.Build();
            Assert.IsNull(command.Parent);
            Assert.AreEqual(2, command.Subcommands.Count);
            Assert.AreEqual("add", command.Subcommands[0].Name);
            Assert.AreEqual("get", command.Subcommands[1].Name);
            Assert.AreEqual(2, command.Subcommands[0].Arguments.Count);
            Assert.AreEqual(1, command.Subcommands[1].Arguments.Count);
        }

        [TestMethod]
        public void MultiLevelSubCommands()
        {
            CommandBuilder commandBuilder = CommandBuilder.From<TestMultiLevelSubCommands.InfoCommand>();
            Assert.AreEqual(2, commandBuilder.Subcommands.Count);

            Command command = commandBuilder.Build();
            Assert.IsNull(command.Parent);
            Assert.AreEqual(2, command.Subcommands.Count);
            Assert.AreEqual(command, command.Subcommands[0].Parent);
            Assert.AreEqual(command, command.Subcommands[1].Parent);
            Assert.AreEqual("user", command.Subcommands[0].Name);
            Assert.AreEqual("channel", command.Subcommands[1].Name);
            Assert.AreEqual(3, command.Subcommands[0].Subcommands.Count);
            Assert.AreEqual(2, command.Subcommands[1].Subcommands.Count);
            Assert.AreEqual(1, command.Subcommands[0].Subcommands[0].Arguments.Count);
            Assert.AreEqual(1, command.Subcommands[0].Subcommands[1].Arguments.Count);
            Assert.AreEqual(2, command.Subcommands[0].Subcommands[2].Arguments.Count);
            Assert.AreEqual(1, command.Subcommands[1].Subcommands[0].Arguments.Count);
            Assert.AreEqual(1, command.Subcommands[1].Subcommands[1].Arguments.Count);
        }
    }
}
