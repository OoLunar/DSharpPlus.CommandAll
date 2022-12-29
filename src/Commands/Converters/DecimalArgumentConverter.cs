using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class DecimalArgumentConverter : IArgumentConverter<decimal>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Number;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(decimal);

        /// <inheritdoc/>
        public Task<Optional<decimal>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(decimal.TryParse(value, out decimal result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<decimal>());
    }
}
