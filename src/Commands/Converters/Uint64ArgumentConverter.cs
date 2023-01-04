using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class UInt64ArgumentConverter : ArgumentConverter<ulong>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<ulong>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(ulong.TryParse(value, out ulong result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<ulong>());
    }
}
