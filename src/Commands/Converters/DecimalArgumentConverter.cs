using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class DecimalArgumentConverter : ArgumentConverter<decimal>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Number;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<decimal>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(decimal.TryParse(value, out decimal result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<decimal>());
    }
}
