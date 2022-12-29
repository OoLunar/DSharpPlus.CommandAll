using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class DateTimeOffsetArgumentConverter : IArgumentConverter<DateTimeOffset>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(DateTimeOffset);

        /// <inheritdoc/>
        public Task<Optional<DateTimeOffset>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(DateTimeOffset.TryParse(value, out DateTimeOffset result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<DateTimeOffset>());
    }
}
