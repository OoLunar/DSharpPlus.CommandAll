using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Converters
{
    /// <summary>
    /// Represents a converter that converts a string into an object.
    /// </summary>
    /// <remarks>
    /// This interface is used by the command system to convert strings into objects. If you are looking to create a custom converter, you should implement <see cref="IArgumentConverter{T}"/> instead.
    /// </remarks>
    public interface IArgumentConverter
    {
        /// <summary>
        /// The option type expected by this converter. Used to inform Discord what parameter type to send.
        /// </summary>
        ApplicationCommandOptionType OptionType { get; init; }

        /// <summary>
        /// Converts a string into an object.
        /// </summary>
        /// <param name="context">The context of the command.</param>
        /// <param name="parameter">The parameter that is being converted.</param>
        /// <param name="value">The string to convert.</param>
        /// <returns>The converted object, wrapped in an <see cref="IOptional"/> to indicate whether the conversion was successful or not.</returns>
        Task<IOptional> ConvertAsync(CommandContext context, CommandParameter parameter, string value);
    }
}
