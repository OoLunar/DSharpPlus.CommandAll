using System;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Enums
{
    [Flags]
    public enum CommandOverloadFlags
    {
        None = 0,
        Disabled = 1 << 1
    }
}
