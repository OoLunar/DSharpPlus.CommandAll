using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <summary>
    /// Represents a converter that converts a string into an object.
    /// </summary>
    public abstract class ArgumentConverter<T> : IArgumentConverter
    {
        public abstract ApplicationCommandOptionType OptionType { get; }
        public abstract ArgumentParsingBehavior ParsingBehavior { get; }

        /// <inheritdoc cref="IArgumentConverter.ConvertAsync(CommandContext, string, CommandParameter?)"/>
        /// <returns>The converted object, wrapped in an <see cref="Optional{T}"/> to indicate whether the conversion was successful or not.</returns>
        /// <typeparam name="T">The type to convert to.</typeparam>
        public abstract Task<Optional<T>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null);

        /// <inheritdoc/>
        Task<IOptional> IArgumentConverter.ConvertAsync(CommandContext context, string value, CommandParameter? parameter) => Task.FromResult<IOptional>(ConvertAsync(context, value, parameter).GetAwaiter().GetResult());

        /// <inheritdoc/>
        public virtual bool CanConvert(Type type) => type == typeof(T);
    }
}
