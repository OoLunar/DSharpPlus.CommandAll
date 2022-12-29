using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class Int64ArgumentConverter : IArgumentConverter<long>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(long);

        /// <inheritdoc/>
        public Task<Optional<long>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(long.TryParse(value, out long result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<long>());
    }
}
