using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Parsers;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    public interface ICommandManager
    {
        IReadOnlyDictionary<string, Command> Commands { get; }
        IArgumentConverterManager ArgumentConverterManager { get; init; }
        ICommandOverloadParser CommandOverloadHandler { get; init; }

        void AddCommand<T>() where T : BaseCommand;
        void AddCommand(Type commandType);
        void AddCommands(Assembly assembly);
        void AddCommands(IEnumerable<Type> commandTypes);

        bool TryFindCommand(string commandString, [NotNullWhen(true)] out string? rawArguments, [NotNullWhen(true)] out Command? command);
    }
}
