using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        /// <returns>All currently registered argument converters.</returns>
        IReadOnlyDictionary<Type, ArgumentConverterDefinition> GetTypeConverters();

        /// <summary>
        /// Attempts to get an argument converter for the specified type.
        /// </summary>
        /// <param name="type">The type to get an argument converter for.</param>
        /// <param name="converter">The argument converter for the specified type.</param>
        /// <returns>Whether or not an argument converter was found.</returns>
        bool TryGetConverter(Type type, [NotNullWhen(true)] out ArgumentConverterDefinition? converter);

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
