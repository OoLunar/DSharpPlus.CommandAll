using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class UInt32ArgumentConverter : ArgumentConverter<uint>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<uint>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(uint.TryParse(value, out uint result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<uint>());
    }
}
