using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class FloatArgumentConverter : IArgumentConverter<float>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Number;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(float);

        /// <inheritdoc/>
        public Task<Optional<float>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(float.TryParse(value, out float result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<float>());
    }
}
