using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class DateTimeArgumentConverter : IArgumentConverter<DateTime>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.String;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(DateTime);

        /// <inheritdoc/>
        public Task<Optional<DateTime>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(DateTime.TryParse(value, out DateTime result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<DateTime>());
    }
}
