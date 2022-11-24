using System;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Enums
{
    [Flags]
    public enum CommandFlags
    {
        None,
        Disabled = 1 << 1
    }
}
