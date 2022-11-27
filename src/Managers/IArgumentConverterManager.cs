using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OoLunar.DSharpPlus.CommandAll.Commands.Arguments;
using OoLunar.DSharpPlus.CommandAll.Commands.Builders;

namespace OoLunar.DSharpPlus.CommandAll.Managers
{
    /// <summary>
    /// Manages argument converters, allowing them to be registered and searched.
    /// </summary>
    public interface IArgumentConverterManager
    {
        /// <summary>
        /// The argument converters registered to this manager.
        /// </summary>
        IReadOnlyDictionary<Type, Type> TypeConverters { get; }

        /// <summary>
        /// Adds an argument converter to the <see cref="TypeConverters"/> dictionary.
        /// </summary>
        /// <typeparam name="T">The type to convert from. Must implement <see cref="IArgumentConverter{T}"/>.</typeparam>
        void AddArgumentConverter<T>() where T : IArgumentConverter;

        /// <inheritdoc cref="AddArgumentConverter{T}"/>
        /// <param name="type">The type to convert from. Must implement <see cref="IArgumentConverter{T}"/>.</param>
        void AddArgumentConverter(Type type);

        /// <summary>
        /// Searches through the assembly for argument converters and adds them to the <see cref="TypeConverters"/> dictionary.
        /// </summary>
        /// <param name="assembly">The assembly to search for argument converters.</param>
        void AddArgumentConverters(Assembly assembly);

        /// <summary>
        /// Searches through the collection of types for argument converters and adds them to the <see cref="TypeConverters"/> dictionary.
        /// </summary>
        void AddArgumentConverters(IEnumerable<Type> types);

        /// <summary>
        /// Iterates over <paramref name="parameters"/> and assigns the appropriate argument converter to each, if applicable.
        /// </summary>
        /// <param name="parameters">The parameters to assign argument converters to.</param>
        /// <param name="failedParameters">The parameters that failed to be assigned an argument converter, usually due to the lack of such argument converter not being registered.</param>
        /// <returns>Whether or not all parameters were assigned an argument converter.</returns>
        bool TrySaturateParameters(IEnumerable<CommandParameterBuilder> parameters, [NotNullWhen(false)] out IEnumerable<CommandParameterBuilder>? failedParameters);
    }
}
