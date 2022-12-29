using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class UInt32ArgumentConverter : IArgumentConverter<uint>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Integer;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(uint);

        /// <inheritdoc/>
        public Task<Optional<uint>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(uint.TryParse(value, out uint result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<uint>());
    }
}
