using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class UInt64ArgumentConverter : IArgumentConverter<ulong>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(ulong);

        /// <inheritdoc/>
        public Task<Optional<ulong>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(ulong.TryParse(value, out ulong result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<ulong>());
    }
}
