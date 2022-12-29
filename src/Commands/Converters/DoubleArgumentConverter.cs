using System;
using System.Threading.Tasks;
using DSharpPlus.CommandAll.Commands.Enums;
using DSharpPlus.Entities;

namespace DSharpPlus.CommandAll.Commands.Converters
{
    /// <inheritdoc cref="IArgumentConverter{T}"/>
    public sealed class DoubleArgumentConverter : IArgumentConverter<double>
    {
        /// <inheritdoc/>
        public ApplicationCommandOptionType OptionType => ApplicationCommandOptionType.Number;

        /// <inheritdoc/>
        public ArgumentParsingBehavior ParsingBehavior => ArgumentParsingBehavior.Static;

        /// <inheritdoc/>
        public bool CanConvert(Type type) => type == typeof(double);

        /// <inheritdoc/>
        public Task<Optional<double>> ConvertAsync(CommandContext context, string value, CommandParameter? parameter = null) => Task.FromResult(double.TryParse(value, out double result)
            ? Optional.FromValue(result)
            : Optional.FromNoValue<double>());
    }
}
