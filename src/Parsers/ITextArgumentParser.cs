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
        Quoted,
        Escaped,
        Backticked
    }
}
