using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <summary>
    /// Represents a converter that converts a string into an object.
    /// </summary>
    /// <remarks>
    /// This interface is used by the command system to convert strings into objects. Unless you're trying to implement an open generic converter, you should implement <see cref="IArgumentConverter{T}"/> instead.
    /// </remarks>
    public interface IArgumentConverter
    {
        /// <summary>
        /// The option type expected by this converter. Used to inform Discord what parameter type to send.
        /// </summary>
        ApplicationCommandOptionType OptionType { get; }

        /// <summary>
        /// Defines how the converter should be used and which behavior the invocator should expect.
        /// </summary>
        ArgumentParsingBehavior ParsingBehavior { get; }

        /// <summary>
        /// The type this converter converts to.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Converts a string into an object.
        /// </summary>
        /// <param name="context">The context of the command.</param>
        /// <param name="value">The string to convert.</param>
        /// <param name="parameter">The parameter that is being converted.</param>
        /// <returns>The converted object, wrapped in an <see cref="IOptional"/> to indicate whether the conversion was successful or not.</returns>
        Task<IOptional> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null);

        /// <summary>
        /// Checks whether this converter can convert the given type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>Whether this converter can convert the given type.</returns>
        bool CanConvert(Type type);
    }
}
