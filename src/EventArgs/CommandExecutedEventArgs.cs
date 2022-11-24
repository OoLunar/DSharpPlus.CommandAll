using System;
using Emzi0767.Utilities;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.EventArgs
{
    public sealed class CommandExecutedEventArgs : AsyncEventArgs
    {
        public readonly CommandContext Context;
        public CommandExecutedEventArgs(CommandContext context) => Context = context ?? throw new ArgumentNullException(nameof(context));
    }
}
