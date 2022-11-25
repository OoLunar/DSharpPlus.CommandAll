using System.Diagnostics.CodeAnalysis;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    public interface IPrefixParser
    {
        bool TryRemovePrefix(CommandAllExtension extension, string message, [NotNullWhen(true)] out string? messageWithoutPrefix);
    }
}
