using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class FloatArgumentConverter : ArgumentConverter<float>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Number;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<float>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(float.TryParse(value, out float result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<float>());
    }
}
