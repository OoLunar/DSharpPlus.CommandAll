using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class DoubleArgumentConverter : ArgumentConverter<double>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Number;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<double>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(double.TryParse(value, out double result)
           ? Optional.FromValue(result)
           : Optional.FromNoValue<double>());
    }
}
