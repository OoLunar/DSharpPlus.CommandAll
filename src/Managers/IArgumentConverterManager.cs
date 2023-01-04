using System;
using System.Collections.Generic;
using System.Reflection;
using DSharpPlus.CommandAll.Commands.Converters;

namespace DSharpPlus.CommandAll.Managers
{
    /// <summary>
    /// Manages argument converters, allowing them to be registered and searched.
    /// </summary>
    public interface IArgumentConverterManager
    {
        /// <summary>
        /// Returns all currently registered argument converters.
        /// </summary>
        /// <typeparam name="T">The type to search for argument converters.</typeparam>
        /// <returns>All currently registered argument converters.</returns>
        IReadOnlyList<ArgumentConverterDefinition> GetConverters<T>();

        /// <inheritdoc cref="GetConverters{T}"/>
        /// <param name="type">The type to search for argument converters. If none is provided then all registered converters are returned.</param>
        IReadOnlyList<ArgumentConverterDefinition> GetConverters(Type? type = null);

        /// <summary>
        /// Searches through the assembly for types that are assignable from <see cref="IArgumentConverter"/> and adds them to the <see cref="TypeConverters"/> dictionary.
        /// </summary>
        /// <param name="assembly">The assembly to search for argument converters.</param>
        /// <returns>The argument converters that were added.</returns>
        IReadOnlyList<ArgumentConverterDefinition> AddArgumentConverters(Assembly assembly);

        /// <summary>
        /// Searches through the collection of types that are assignable from <see cref="IArgumentConverter"/> and adds them to the <see cref="TypeConverters"/> dictionary.
        /// </summary>
        /// <param name="types">The types to search for argument converters.</param>
        /// <returns>The argument converters that were added.</returns>
        IReadOnlyList<ArgumentConverterDefinition> AddArgumentConverters(params Type[] types);
    }
}
