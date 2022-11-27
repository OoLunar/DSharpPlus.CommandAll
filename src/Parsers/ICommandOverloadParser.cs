using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OoLunar.DSharpPlus.CommandAll.Commands;

namespace OoLunar.DSharpPlus.CommandAll.Parsers
{
    /// <summary>
    /// Determines which overload of a command should be used by the text command.
    /// </summary>
    public interface ICommandOverloadParser
    {
        /// <summary>
        /// Determines which overload of a command should be used by the text command.
        /// </summary>
        /// <param name="command">The command to determine the overload for.</param>
        /// <param name="arguments">The arguments provided to the command.</param>
        /// <param name="overload">The overload that should be used.</param>
        /// <returns>True if an overload was found, false otherwise.</returns>
        bool TryParseOverload(Command command, IEnumerable<string> arguments, [NotNullWhen(true)] out CommandOverload? overload);
    }
}
