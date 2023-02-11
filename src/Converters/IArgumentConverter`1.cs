using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    /// <summary>
    /// Represents a converter that converts a string into an object.
    /// </summary>
    public interface IArgumentConverter<T> : IArgumentConverter
    {
        /// <returns>The converted object, wrapped in an <see cref="Optional{T}"/> to indicate whether the conversion was successful or not.</returns>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <inheritdoc cref="IArgumentConverter.ConvertAsync(CommandContext, CommandParameter, string)"/>
        new Task<Optional<T>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null);

        /// <inheritdoc/>
        Task<IOptional> IArgumentConverter.ConvertAsync(CommandContext context, string value, CommandParameter? parameter) => Task.FromResult<IOptional>(ConvertAsync(context, value, parameter).GetAwaiter().GetResult());
    }
}
