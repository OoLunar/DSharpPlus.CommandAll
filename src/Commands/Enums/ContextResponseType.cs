using System;

namespace DSharpPlus.CommandAll.Commands.Enums
{
    [Flags]
    public enum ContextResponseType
    {
        None = 0,
        Delayed,
        Created,
        Updated,
        Deleted,
        Prompt
    }
}
