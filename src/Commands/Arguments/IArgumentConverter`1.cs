using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace OoLunar.DSharpPlus.CommandAll.Commands.Arguments
{
    /// <summary>
    /// Represents a converter that converts a string into an object.
    /// </summary>
    public interface IArgumentConverter<T> : IArgumentConverter
    {
        /// <inheritdoc cref="IArgumentConverter.ConvertAsync(CommandContext, CommandParameter, string)"/>
        new Task<Optional<T>> ConvertAsync(CommandContext context, CommandParameter parameter, string value);

        /// <inheritdoc/>
        Task<IOptional> IArgumentConverter.ConvertAsync(CommandContext context, CommandParameter parameter, string value) => Task.FromResult<IOptional>(ConvertAsync(context, parameter, value).GetAwaiter().GetResult());
    }
}
