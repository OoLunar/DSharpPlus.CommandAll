using System;
using Emzi0767.Utilities;
using OoLunar.DSharpPlus.CommandAll.Managers;

namespace OoLunar.DSharpPlus.CommandAll.EventArgs
{
    public sealed class ConfigureCommandsEventArgs : AsyncEventArgs
    {
        public readonly CommandAllExtension Extension;
        public readonly ICommandManager CommandManager;

        public ConfigureCommandsEventArgs(CommandAllExtension extension, ICommandManager commandManager)
        {
            Extension = extension ?? throw new ArgumentNullException(nameof(extension));
            CommandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
        }
    }
}
