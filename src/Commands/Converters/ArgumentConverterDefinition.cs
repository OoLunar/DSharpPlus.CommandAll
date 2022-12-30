using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <summary>
    /// The argument converter class exposed to the user.
    /// </summary>
    /// <param name="ArgumentConverterType">The type to convert to.</param>
    /// <param name="GenericArguments">The generic arguments that the type supports.</param>
    /// <param name="SingletonInstance">The type to convert from.</param>
    public sealed record ArgumentConverterDefinition(Type ArgumentConverterType, Type ConvertedType, IReadOnlyList<Type> GenericArguments, IArgumentConverter? SingletonInstance)
    {
        public IArgumentConverter GetOrCreateConverter(IServiceProvider serviceProvider) => SingletonInstance ?? (IArgumentConverter)ActivatorUtilities.CreateInstance(serviceProvider, ArgumentConverterType);
    }
}
