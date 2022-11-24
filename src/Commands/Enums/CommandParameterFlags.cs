using System;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Enums
{
    [Flags]
    public enum CommandParameterFlags
    {
        None = 0,
        Optional = 1 << 1,
        Params = 1 << 2,
    }
}
