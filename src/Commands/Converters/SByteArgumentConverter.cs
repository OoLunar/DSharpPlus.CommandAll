using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class SByteArgumentConverter : IArgumentConverter<sbyte>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(sbyte);

        /// <inheritdoc/>
        public Task<Optional<sbyte>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(sbyte.TryParse(value, out sbyte result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<sbyte>());
    }
}
