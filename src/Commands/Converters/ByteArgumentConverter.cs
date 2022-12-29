using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class ByteArgumentConverter : IArgumentConverter<byte>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(byte);

        /// <inheritdoc/>
        public Task<Optional<byte>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(byte.TryParse(value, out byte result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<byte>());
    }
}
