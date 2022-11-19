using System;
using System.Collections.Generic;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Commands;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    public interface IArgumentConverterManager
    {
        IReadOnlyList<CommandParameter> Parameters { get; }
        IReadOnlyDictionary<Type, Type> TypeConverters { get; }

        void AddArgumentConverter(Type type);
        void AddArgumentConverter<T>() where T : IArgumentConverter<T>;
        void AddArgumentConverters(Assembly assembly);
        void AddArgumentConverters(IEnumerable<Type> types);
        void AddParameters(IEnumerable<CommandParameter> parameters);
    }
}
