using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class UInt16ArgumentConverter : IArgumentConverter<ushort>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(ushort);

        /// <inheritdoc/>
        public Task<Optional<ushort>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(ushort.TryParse(value, out ushort result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<ushort>());
    }
}
