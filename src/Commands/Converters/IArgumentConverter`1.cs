using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <summary>
    /// Represents a converter that converts a string into an object.
    /// </summary>
    public interface IArgumentConverter<T> : IArgumentConverter
    {
        static Type IArgumentConverter.Type { get; } = Type;
        static new Type Type => typeof(T);

        /// <inheritdoc cref="IArgumentConverter.ConvertAsync(CommandContext, string, CommandParameter?)"/>
        /// <returns>The converted object, wrapped in an <see cref="Optional{T}"/> to indicate whether the conversion was successful or not.</returns>
        /// <typeparam name="T">The type to convert to.</typeparam>
        new Task<Optional<T>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null);

        /// <inheritdoc/>
        Task<IOptional> IArgumentConverter.ConvertAsync(CommandContext context, string value, CommandParameter? parameter) => Task.FromResult<IOptional>(ConvertAsync(context, value, parameter).GetAwaiter().GetResult());
    }
}
