using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    public interface ICommandOverloadParser
    {
        bool TryParseOverload(Command command, IDictionary<string, object> arguments, [NotNullWhen(true)] out CommandOverload? overload);
        bool TryParseOverload(Command command, IEnumerable<string> arguments, [NotNullWhen(true)] out CommandOverload? overload);
    }
}
