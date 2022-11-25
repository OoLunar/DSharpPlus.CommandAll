using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    public interface IArgumentConverterManager
    {
        IReadOnlyDictionary<Type, Type> TypeConverters { get; }

        void AddArgumentConverter(Type type);
        void AddArgumentConverter<T>() where T : IArgumentConverter;
        void AddArgumentConverters(Assembly assembly);
        void AddArgumentConverters(IEnumerable<Type> types);
        bool TrySaturateParameters(IEnumerable<CommandParameterBuilder> parameters, [NotNullWhen(false)] out IEnumerable<CommandParameterBuilder>? failedParameters);
    }
}
