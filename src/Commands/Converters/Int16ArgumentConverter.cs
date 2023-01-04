using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class Int16ArgumentConverter : ArgumentConverter<short>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<short>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(short.TryParse(value, out short result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<short>());
    }
}
