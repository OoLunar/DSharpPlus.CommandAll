using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class SByteArgumentConverter : ArgumentConverter<sbyte>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<sbyte>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(sbyte.TryParse(value, out sbyte result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<sbyte>());
    }
}
