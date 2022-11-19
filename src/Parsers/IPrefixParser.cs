using System.Diagnostics.CodeAnalysis;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    public interface IPrefixParser
    {
        bool TryRemovePrefix(string message, [NotNullWhen(true)] out string? messageWithoutPrefix);
    }
}
