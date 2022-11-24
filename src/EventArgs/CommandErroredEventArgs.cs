using System;
using Emzi0767.Utilities;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.EventArgs
{
    public sealed class CommandErroredEventArgs : AsyncEventArgs
    {
        public readonly CommandContext Context;
        public readonly Exception Exception;

        public CommandErroredEventArgs(CommandContext context, Exception exception)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }
}
