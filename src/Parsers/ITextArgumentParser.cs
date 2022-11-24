using System;
using System.Collections.Generic;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    public interface ITextArgumentParser
    {
        bool TryExtractArguments(string message, out IReadOnlyList<string> arguments);
    }

    [Flags]
    internal enum ArgumentState
    {
        None = 0,
        Quoted = 1 << 1,
        Escaped = 1 << 2,
        Backticked = 1 << 3,
        Whitespace = 1 << 4
    }
}
