using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class Int64ArgumentConverter : ArgumentConverter<long>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<long>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(long.TryParse(value, out long result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<long>());
    }
}
