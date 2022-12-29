using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class Int16ArgumentConverter : IArgumentConverter<short>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(short);

        /// <inheritdoc/>
        public Task<Optional<short>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(short.TryParse(value, out short result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<short>());
    }
}
