using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class UInt16ArgumentConverter : ArgumentConverter<ushort>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<ushort>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(ushort.TryParse(value, out ushort result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<ushort>());
    }
}
