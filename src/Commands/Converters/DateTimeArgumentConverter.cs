using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="ArgumentConverter{T}"/>
    public sealed class DateTimeArgumentConverter : ArgumentConverter<DateTime>
    {
        /// <inheritdoc/>
        public override ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;

        /// <inheritdoc/>
        public override ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public override Task<Optional<DateTime>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(DateTime.TryParse(value, out DateTime result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<DateTime>());
    }
}
