using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class BooleanArgumentConverter : ArgumentConverter<bool>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Boolean;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<bool>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(bool.TryParse(value, out bool result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<bool>());
    }
}
