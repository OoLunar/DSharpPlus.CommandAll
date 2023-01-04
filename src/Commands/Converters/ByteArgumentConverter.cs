using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class ByteArgumentConverter : ArgumentConverter<byte>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<byte>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(byte.TryParse(value, out byte result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<byte>());
    }
}
